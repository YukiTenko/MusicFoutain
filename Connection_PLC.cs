using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Opc.Ua;
using Opc.Ua.Client;
using Siemens.UAClientHelper;

namespace MusicFountain
{
    public partial class Connection_PLC
    {
        // Variables
        private int plcID;
        private string serverURL;

        private OPC_UAClientHelperAPI opcUAClient;
        private EndpointDescription opcUAEndpoint;
        private Session opcUASession;

        private List<string[]> vsaData;

        // Events

        public Connection_PLC(int newID)
        {
            {// Creat variables
                plcID = newID;
                serverURL = "";
                vsaData = new List<string[]>();
                opcUAClient = new OPC_UAClientHelperAPI();
            }
        }
        // Private methods
        private void Notification_KeepAlive(ISession sender, KeepAliveEventArgs e)
        {
            try
            {
                // check for events from discarded sessions.
                if (!Object.ReferenceEquals(sender, opcUASession))
                {
                    return;
                }
                // check for disconnected session.
                if (!ServiceResult.IsGood(e.Status))
                {
                    // try reconnecting using the existing session state
                    opcUASession.Reconnect();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Mất kết nối tới server OPC UA .\nCó thể do server OPC UA đã khởi động lại .\nVui lòng khởi động lại phần mềm .");
            }
        }
        private void Notification_ServerCertificate(CertificateValidator cert, CertificateValidationEventArgs e)
        {
            try
            {
                //Search for the server's certificate in store; if found -> accept
                X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509CertificateCollection certCol = store.Certificates.Find(X509FindType.FindByThumbprint, e.Certificate.Thumbprint, true);
                store.Close();
                if (certCol.Capacity > 0)
                {
                    e.Accept = true;
                }

                //Show cert dialog if cert hasn't been accepted yet
                else
                {
                    MessageBox.Show("Notification_ServerCertificate: Cert hasn't been accepted yet");
                }
            }
            catch
            {
                ;
            }
        }
        // Public methods -------------------------------------------------------------------------------
        public int Get_PLCID()
        {
            return plcID;
        }
        public void Connect2OPCUAServer(string newServerURL)
        {
            try
            {
                serverURL = newServerURL;
                ApplicationDescriptionCollection servers = opcUAClient.FindServers("opc.tcp://" + serverURL);
                foreach (ApplicationDescription ad in servers)
                {
                    foreach (string url in ad.DiscoveryUrls)
                    {
                        try
                        {
                            EndpointDescriptionCollection endpoints = opcUAClient.GetEndpoints(url);
                            for (int i = 0; i < endpoints.Count; i++)
                            {
                                string securityPolicy = endpoints[i].SecurityPolicyUri.Remove(0, 42);
                                if (securityPolicy == "#None")
                                {
                                    opcUAEndpoint = endpoints[i];
                                    try
                                    {
                                        //Register mandatory events (cert and keep alive)
                                        opcUAClient.KeepAliveNotification += new KeepAliveEventHandler(Notification_KeepAlive);
                                        opcUAClient.CertificateValidationNotification += new CertificateValidationEventHandler(Notification_ServerCertificate);

                                        //Check for a selected endpoint
                                        if (opcUAEndpoint != null)
                                        {
                                            //Call connect
                                            opcUAClient.Connect(opcUAEndpoint, false, "", "").Wait();
                                            //Extract the session object for further direct session interactions
                                            opcUASession = opcUAClient.Session;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Endpoint không xác định .");
                                            return;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("Có lỗi xảy ra khi chạy phần test OPC");
                                    }
                                    i = endpoints.Count;
                                }
                            }
                            if (endpoints.Count <= 0)
                            {
                                MessageBox.Show("Không thể kết nối đến server OPC UA theo địa chỉ: " + serverURL + " .");
                            }
                        }
                        catch (ServiceResultException sre)
                        {
                            //If an url in ad.DiscoveryUrls can not be reached, myClientHelperAPI will throw an Exception
                            MessageBox.Show(sre.Message, "Error");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        public List<string[]> ReadStructUdt_VOA()
        {
            vsaData = opcUAClient.ReadStructUdt("ns=4;i=5");
            vsaData.RemoveAt(vsaData.Count - 1);
            //vsaData.RemoveAt(12);
            return vsaData;
        }
        public void WriteStructUdt_VOA()
        {
            opcUAClient.WriteStructUdt("ns=4;i=5", vsaData);
        }
        public void ChangeValveInfo_VOA(Data_Config_Device_VOA valveConfig)
        {
            ReadStructUdt_VOA();
            WriteStructUdt_VOA();
        }
        public List<string[]> ReadStructUdt_VSA()
        {
            vsaData = opcUAClient.ReadStructUdt("ns=4;i=5");
            vsaData.RemoveAt(vsaData.Count - 1);
            //vsaData.RemoveAt(12);
            return vsaData;
        }
        public void WriteStructUdt_VSA()
        {
            opcUAClient.WriteStructUdt("ns=4;i=5", vsaData);
        }
        public void ChangeValveInfo_VSA(Data_Config_Device_VSA valveConfig)
        {
            ReadStructUdt_VSA();
            vsaData[1 + (valveConfig.devID - 1) * 6][2] = valveConfig.enable.ToString();
            vsaData[2 + (valveConfig.devID - 1) * 6][2] = valveConfig.setAngle.ToString();
            vsaData[4 + (valveConfig.devID - 1) * 6][2] = valveConfig.setSpeed.ToString();
            WriteStructUdt_VSA();
        }
    }
}
