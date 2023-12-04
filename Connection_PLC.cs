using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Opc.Ua;
using Opc.Ua.Client;
using Siemens.UAClientHelper;

namespace MusicFountain.Connection
{
    public partial class Connection_PLC
    {
        // Constant
        private string VALVE_STEPPER_CONFIG_ADDRESS = "ns=4;i=5";
        private string VALVE_STEPPER_DATA_ADDRESS = "ns=4;i=24";

        private string VALVE_ONOFF_CONFIG_ADDRESS = "ns=4;i=59";
        private string VALVE_ONOFF_DATA_ADDRESS = "ns=4;i=63";

        //private string LED_CONFIG_ADDRESS = "ns=4;i=78";
        private string LED_DATA_ADDRESS = "ns=4;i=85";

        //private string PUMP_DIGITAL_CONFIG_ADDRESS = "ns=4;i=1160";
        private string PUMP_DIGITAL_DATA_ADDRESS = "ns=4;i=1164";

        private string PUMP_ANALOG_CONFIG_ADDRESS = "ns=4;i=1197";
        private string PUMP_ANALOG_DATA_ADDRESS = "ns=4;i=1208";

        //private string SYSTEM_POWER_CONFIG_ADDRESS = "ns=4;i=1255";
        private string SYSTEM_POWER_DATA_ADDRESS = "ns=4;i=1259";

        // Variables
        private int plcID;
        private string serverURL;
        private string serverConnectionState;

        private OPC_UAClientHelperAPI opcUAClient;
        private EndpointDescription opcUAEndpoint;
        private Session opcUASession;

        private List<string[]> vsPLCConfig;
        private List<string[]> vsPLCData;

        private List<string[]> voPLCConfig;
        private List<string[]> voPLCData;

        //private List<string[]> pdPLCConfig;
        private List<string[]> pdPLCData;

        private List<string[]> paPLCConfig;
        private List<string[]> paPLCData;

        //private List<string[]> ledPLCConfig;
        private List<string[]> ledPLCData;

        //private List<string[]> systemPoweConfig;
        private List<string[]> systemPoweData;

        //private Thread connectThread;

        // Events
        public event EventHandler<int> plcConnected;
        public event EventHandler<int> plcConnecting;
        public event EventHandler<int> plcDisconnected;

        public Connection_PLC(int newID, string newServerURL)
        {
            {// Creat variables
                plcID = newID;
                serverURL = newServerURL;
                serverConnectionState = "Disconnected";

                opcUAClient = new OPC_UAClientHelperAPI();
                opcUAEndpoint = null;
                opcUASession = null;

                {// Read/Write buffer
                    vsPLCConfig = null;
                    vsPLCData = null;

                    voPLCConfig = null;
                    voPLCData = null;

                    pdPLCData = null;

                    paPLCConfig = null;
                    paPLCData = null;

                    ledPLCData = null;

                    systemPoweData = null;
                }

                //connectThread = null;
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
                    serverConnectionState = "Disconnected";
                    plcDisconnected.Invoke(this, plcID);
                    // try reconnecting using the existing session state
                    opcUASession.Reconnect();
                }
                else
                {
                    serverConnectionState = "Connected";
                    plcConnected.Invoke(this, plcID);
                }
            }
            catch (Exception)
            {
                serverConnectionState = "Disconnected";
                plcDisconnected.Invoke(this, plcID);
                //MessageBox.Show("Mất kết nối tới server OPC UA .\nCó thể do server OPC UA đã khởi động lại .\nVui lòng khởi động lại phần mềm .");
            }
        }
        private void Notification_ServerCertificate(CertificateValidator cert, CertificateValidationEventArgs e)
        {
            try
            {
                //Search for the server's certificate in store;if found -> accept
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
        public string Get_ConnectionState()
        {
            return serverConnectionState;
        }
        public void Connect2OPCUAServer()
        {
            try
            {
                serverConnectionState = "Connecting";
                plcConnecting.Invoke(this, plcID);
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
                                            serverConnectionState = "Connected";
                                            plcConnected.Invoke(this, plcID);
                                            {// Read data struct first time
                                                ReadStructUdt_VS_PLC_Data();
                                                ReadStructUdt_VO_PLC_Data();
                                                ReadStructUdt_LED_PLC_Data();
                                                ReadStructUdt_PD_PLC_Data();
                                                ReadStructUdt_PA_PLC_Data();
                                                ReadStructUdt_SystemPower_Data();
                                            }
                                        }
                                        else
                                        {
                                            serverConnectionState = "Disconnected";
                                            plcDisconnected.Invoke(this, plcID);
                                            MessageBox.Show("Endpoint không xác định .");
                                            return;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        serverConnectionState = "Disconnected";
                                        plcDisconnected.Invoke(this, plcID);
                                        MessageBox.Show("Không thể kết nối đến server OPC UA theo địa chỉ: " + serverURL + " .");
                                    }
                                    i = endpoints.Count;
                                }
                            }
                            if (endpoints.Count <= 0)
                            {
                                serverConnectionState = "Disconnected";
                                plcDisconnected.Invoke(this, plcID);
                                MessageBox.Show("Không thể kết nối đến server OPC UA theo địa chỉ: " + serverURL + " .");
                            }
                        }
                        catch (ServiceResultException sre)
                        {
                            serverConnectionState = "Disconnected";
                            plcDisconnected.Invoke(this, plcID);
                            //If an url in ad.DiscoveryUrls can not be reached, myClientHelperAPI will throw an Exception
                            MessageBox.Show(sre.Message, "Error");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                serverConnectionState = "Disconnected";
                plcDisconnected.Invoke(this, plcID);
                MessageBox.Show(ex.Message, "Error");
            }
            //connectThread = null;
        }
        public void Disconnect2OPCUAServer()
        {
            opcUAClient.Disconnect();
        }





        public List<string[]> ReadStructUdt_VS_PLC_Config()
        {
            Console.WriteLine("ReadStructUdt_VS_PLC");
            if (opcUAClient.Session != null)
            {
                vsPLCConfig = opcUAClient.ReadStructUdt(VALVE_STEPPER_CONFIG_ADDRESS);
                vsPLCConfig.RemoveAt(vsPLCConfig.Count - 1);
                return vsPLCConfig;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_VS_PLC_Config()
        {
            Console.WriteLine("WriteStructUdt_VS_PLC");
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(VALVE_STEPPER_CONFIG_ADDRESS, vsPLCConfig);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_VS_PLC_Config( Data.Data_Config_Hardware_Option_VS_PLC newOptionVSConfig)
        {
            if (ReadStructUdt_VS_PLC_Config() != null)
            {
                vsPLCConfig[0][2] = newOptionVSConfig.coordAngle.ToString();
                vsPLCConfig[1][2] = newOptionVSConfig.maxAngle.ToString();
                vsPLCConfig[2][2] = newOptionVSConfig.maxSpeed.ToString();
                vsPLCConfig[3][2] = newOptionVSConfig.ratio.ToString();
                vsPLCConfig[4][2] = newOptionVSConfig.coordAngle.ToString();
                vsPLCConfig[5][2] = newOptionVSConfig.maxAngle.ToString();
                vsPLCConfig[6][2] = newOptionVSConfig.maxSpeed.ToString();
                vsPLCConfig[7][2] = newOptionVSConfig.ratio.ToString();
                vsPLCConfig[8][2] = newOptionVSConfig.coordAngle.ToString();
                vsPLCConfig[9][2] = newOptionVSConfig.maxAngle.ToString();
                vsPLCConfig[10][2] = newOptionVSConfig.maxSpeed.ToString();
                vsPLCConfig[11][2] = newOptionVSConfig.ratio.ToString();
                vsPLCConfig[12][2] = newOptionVSConfig.coordAngle.ToString();
                vsPLCConfig[13][2] = newOptionVSConfig.maxAngle.ToString();
                vsPLCConfig[14][2] = newOptionVSConfig.maxSpeed.ToString();
                vsPLCConfig[15][2] = newOptionVSConfig.ratio.ToString();
                WriteStructUdt_VS_PLC_Config();
            }
        }
        public List<string[]> ReadStructUdt_VS_PLC_Data()
        {
            Console.WriteLine("ReadStructUdt_VS_PLC");
            if (opcUAClient.Session != null)
            {
                vsPLCData = opcUAClient.ReadStructUdt(VALVE_STEPPER_DATA_ADDRESS);
                vsPLCData.RemoveAt(vsPLCData.Count - 1);
                return vsPLCData;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_VS_PLC_Data()
        {
            Console.WriteLine("WriteStructUdt_VS_PLC");
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(VALVE_STEPPER_DATA_ADDRESS, vsPLCData);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_VS_PLC_Data(List<Data.Data_Config_Device_VS_PLC> newListValve)
        {
            if (vsPLCData != null)
            {
                for (int i = 0; i < newListValve.Count; i++)
                {
                    vsPLCData[0 + (newListValve[i].devID - 1) * 8][2] = newListValve[i].enable.ToString();
                    vsPLCData[2 + (newListValve[i].devID - 1) * 8][2] = newListValve[i].reset.ToString();
                    vsPLCData[4 + (newListValve[i].devID - 1) * 8][2] = newListValve[i].setSpeed.ToString();
                    vsPLCData[6 + (newListValve[i].devID - 1) * 8][2] = newListValve[i].setAngle.ToString();
                }
                WriteStructUdt_VS_PLC_Data();
            }
        }



        public List<string[]> ReadStructUdt_VO_PLC_Config()
        {
            if (opcUAClient.Session != null)
            {
                voPLCConfig = opcUAClient.ReadStructUdt(VALVE_ONOFF_CONFIG_ADDRESS);
                voPLCConfig.RemoveAt(voPLCConfig.Count - 1);
                return voPLCConfig;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_VO_PLC_Config()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(VALVE_ONOFF_CONFIG_ADDRESS, voPLCConfig);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_VO_PLC_Config(List<Data.Data_Config_Device_VO_PLC> newListValve)
        {
            if (voPLCConfig != null)
            {// Creat message
                for (int i = 0; i < newListValve.Count; i++)
                {// 16 valve/register
                }
                WriteStructUdt_VO_PLC_Config();
            }
        }
        public List<string[]> ReadStructUdt_VO_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                voPLCData = opcUAClient.ReadStructUdt(VALVE_ONOFF_DATA_ADDRESS);
                voPLCData.RemoveAt(voPLCData.Count - 1);
                return voPLCData;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_VO_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(VALVE_ONOFF_DATA_ADDRESS, voPLCData);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_VO_PLC_Data(List<Data.Data_Config_Device_VO_PLC> newListValve)
        {
            if (voPLCData != null)
            {// Creat message
                for (int i = 0; i < newListValve.Count; i++)
                {// 16 valve/register
                    voPLCData[0 + (newListValve[i].devID / 16) * 6][2] = "true";
                    voPLCData[2 + (newListValve[i].devID / 16) * 6][2] = "false";
                    voPLCData[4 + (newListValve[i].devID / 16) * 6][2] = "true";
                    int opcRegisterDataBuf = int.Parse(voPLCData[5 + (newListValve[i].devID / 16) * 6][2]);
                    if (newListValve[i].setOnOff)
                    {
                        opcRegisterDataBuf = opcRegisterDataBuf | (int)(Math.Pow(2, (newListValve[i].devID % 16) - 1) + 0);
                    }
                    else
                    {
                        opcRegisterDataBuf = opcRegisterDataBuf & (int)(65535 - Math.Pow(2, (newListValve[i].devID % 16) - 1));
                    }
                    voPLCData[5 + (newListValve[i].devID / 16) * 6][2] = opcRegisterDataBuf.ToString();
                }
                WriteStructUdt_VO_PLC_Data();
            }
        }



        public List<string[]> ReadStructUdt_PD_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                pdPLCData = opcUAClient.ReadStructUdt(PUMP_DIGITAL_DATA_ADDRESS);
                pdPLCData.RemoveAt(pdPLCData.Count - 1);
                return pdPLCData;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_PD_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(PUMP_DIGITAL_DATA_ADDRESS, pdPLCData);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_PD_PLC_Data(List<Data.Data_Config_Device_Inverter> newListPump)
        {
            if (pdPLCData != null)
            {
                for (int i = 0; i < newListPump.Count; i++)
                {
                    pdPLCData[0 + (newListPump[i].devID - 1) * 6][2] = newListPump[i].enable.ToString();
                    pdPLCData[2 + (newListPump[i].devID - 1) * 6][2] = newListPump[i].reset.ToString();
                    pdPLCData[4 + (newListPump[i].devID - 1) * 6][2] = newListPump[i].setStatus.ToString();
                }
                WriteStructUdt_PD_PLC_Data();
            }
        }



        public List<string[]> ReadStructUdt_PA_PLC_Config()
        {
            if (opcUAClient.Session != null)
            {
                paPLCConfig = opcUAClient.ReadStructUdt(PUMP_ANALOG_CONFIG_ADDRESS);
                paPLCConfig.RemoveAt(paPLCConfig.Count - 1);
                return paPLCConfig;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_PA_PLC_Config()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(PUMP_ANALOG_CONFIG_ADDRESS, paPLCConfig);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_PA_PLC_Config( Data.Data_Config_Hardware_Option_Inverter newInverterption)
        {
            if (ReadStructUdt_PA_PLC_Config() != null)
            {
                paPLCConfig[0][2] = newInverterption.freqMin.ToString();
                paPLCConfig[1][2] = newInverterption.freqMax.ToString();
                paPLCConfig[2][2] = newInverterption.freqMin.ToString();
                paPLCConfig[3][2] = newInverterption.freqMax.ToString();
                paPLCConfig[4][2] = newInverterption.freqMin.ToString();
                paPLCConfig[5][2] = newInverterption.freqMax.ToString();
                paPLCConfig[6][2] = newInverterption.freqMin.ToString();
                paPLCConfig[7][2] = newInverterption.freqMax.ToString();
                WriteStructUdt_PA_PLC_Config();
            }
        }
        public List<string[]> ReadStructUdt_PA_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                paPLCData = opcUAClient.ReadStructUdt(PUMP_ANALOG_DATA_ADDRESS);
                paPLCData.RemoveAt(paPLCData.Count - 1);
                return paPLCData;
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_PA_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(PUMP_ANALOG_DATA_ADDRESS, paPLCData);
            }
            else
            {
                //MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeValveData_PA_PLC_Data(List<Data.Data_Config_Device_Inverter> newListPump)
        {
            if (paPLCData != null)
            {
                for (int i = 0; i < newListPump.Count; i++)
                {
                    paPLCData[0 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].enable.ToString();
                    paPLCData[2 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].reset.ToString();
                    paPLCData[4 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].setMode.ToString();
                    paPLCData[5 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].manFreq.ToString();
                    paPLCData[7 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].sinAmplMin.ToString();
                    paPLCData[8 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].sinAmplMax.ToString();
                    paPLCData[9 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].sinFreqCycle.ToString();
                    paPLCData[10 + (newListPump[i].devID - 1) * 11][2] = newListPump[i].sinPhaseDiff.ToString();
                }
                WriteStructUdt_PA_PLC_Data();
            }
        }



        public List<string[]> ReadStructUdt_LED_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                ledPLCData = opcUAClient.ReadStructUdt(LED_DATA_ADDRESS);
                ledPLCData.RemoveAt(ledPLCData.Count - 1);
                return ledPLCData;
            }
            else
            {
                ////MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_LED_PLC_Data()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(LED_DATA_ADDRESS, ledPLCData);
            }
            else
            {
                ////MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeLEDData_LED_PLC_Data(List<Data.Data_Config_Device_LED_PLC> newListLED, int red, int green, int blue)
        {
            if (ledPLCData != null)
            {
                {// Creat message
                    List<int> messageBuf = new List<int>();
                    {// Creat message header
                        messageBuf.Add(64); // @
                        messageBuf.Add(48); // 0
                        messageBuf.Add(56); // 8 // Ma lenh 08
                        messageBuf.Add(red + green * 2 + blue * 4 + 48); // Tinh ma mau, gia tri toi da la 7 nen khong can check 0-9 hhoac A-F
                    }
                    {// Find data of LED to message
                        for (int i = 0; i < 128; i++)
                        {
                            messageBuf.Add(0);
                        }
                        for (int i = 0; i < newListLED.Count; i++)
                        {
                            if (newListLED[i].state == 1)
                            {
                                int ledDataPosition = ((newListLED[i].devID - 1)/8)*2 + 1 - (((newListLED[i].devID - 1)%8)/4);
                                int ledDataScale = 3; // Highest scale for MSB bit
                                if ((newListLED[i].devID % 4) != 0)
                                {
                                    ledDataScale = (newListLED[i].devID % 4) - 1;
                                }
                                messageBuf[4 + ledDataPosition] += (int)(Math.Pow(2, ledDataScale)); // 4 is number of header bytes
                            }
                        }
                        for (int i = 4; i < messageBuf.Count; i++) // ingore 4 bytes header
                        {
                            if (messageBuf[i] > 9)
                            {
                                messageBuf[i] += 55; // A-F
                            }
                            else
                            {
                                messageBuf[i] += 48; // 0-9
                            }
                        }
                    }
                    {// Calculator CRC
                        int crcBuf = 0;
                        for (int i = 0; i < messageBuf.Count; i++)
                        {
                            crcBuf += messageBuf[i];
                        }
                        int crcBuf1 = (crcBuf % 256) / 16;
                        if (crcBuf1 > 9)
                        {
                            crcBuf1 += 55; // A-F
                        }
                        else
                        {
                            crcBuf1 += 48; // 0-9
                        }
                        int crcBuf2 = crcBuf % 16;
                        if (crcBuf2 > 9)
                        {
                            crcBuf2 += 55; // A-F
                        }
                        else
                        {
                            crcBuf2 += 48; // 0-9
                        }
                        messageBuf.Add(crcBuf1);
                        messageBuf.Add(crcBuf2);
                    }
                    messageBuf.Add(10); // End byte
                    // Send data
                    ledPLCData[0][2] = "True";
                    ledPLCData[4][2] = "True";
                    ledPLCData[5][2] = "";
                    for (int i = 0; i < messageBuf.Count; i++)
                    {
                        ledPLCData[5][2] += messageBuf[i].ToString();
                        ledPLCData[5][2] += ";";
                    }
                    //MessageBox.Show(ledPLCData[5][2]);
                }
                WriteStructUdt_LED_PLC_Data();
            }
        }
        public void Test_LEDData_LED_PLC()
        {
            ReadStructUdt_LED_PLC_Data();
            ledPLCData[0][2] = "True";
            ledPLCData[4][2] = "True";
            ledPLCData[5][2] = "64;48;56;50;48;48;48;48;48;48;48;50;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;48;68;67;10;";
            WriteStructUdt_LED_PLC_Data();
        }



        public List<string[]> ReadStructUdt_SystemPower_Data()
        {
            if (opcUAClient.Session != null)
            {
                systemPoweData = opcUAClient.ReadStructUdt(SYSTEM_POWER_DATA_ADDRESS);
                systemPoweData.RemoveAt(systemPoweData.Count - 1);
                return systemPoweData;
            }
            else
            {
                ////MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
                return null;
            }
        }
        public void WriteStructUdt_SystemPower_Data()
        {
            if (opcUAClient.Session != null)
            {
                opcUAClient.WriteStructUdt(SYSTEM_POWER_DATA_ADDRESS, systemPoweData);
            }
            else
            {
                ////MessageBox.Show("Chưa kết nối được tới server OPC UA theo địa chỉ: " + serverURL + " .");
            }
        }
        public void ChangeSystemPower_Data(bool newSystemPowerState)
        {
            if (systemPoweData != null)
            {
                // Send data
                systemPoweData[0][2] = "True";
                systemPoweData[4][2] = newSystemPowerState.ToString();
                WriteStructUdt_SystemPower_Data();
            }
        }


    }
}
