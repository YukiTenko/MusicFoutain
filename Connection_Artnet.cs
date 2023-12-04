using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Sockets;
using Haukcode.Sockets;
using Haukcode.ArtNet.IO;
using Haukcode.Rdm;
using Haukcode.ArtNet;
using System.Windows;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MusicFountain.Connection
{
    public partial class Connection_Artnet : IDisposable
    {
        // Variables
        private int hardwareID;

        private IPAddress localIP;
        private IPAddress subnetMask;

        private ArtNetSocket artnetSocket;

        private byte artnetPacketSequence;
        private byte artnetPacketPhysical;
        private byte artnetPacketUniverse;

        // Events
        public event EventHandler<int> artnetConnected;

        public Connection_Artnet(int newHardwareID, IPAddress newIP, IPAddress newSubnetMark)
        {
            {// Creat variale
                hardwareID = newHardwareID;
                localIP = newIP;
                subnetMask = newSubnetMark;

                artnetSocket = new ArtNetSocket
                {
                    EnableBroadcast = true
                };

                artnetPacketSequence = 0;
                artnetPacketPhysical = 1;
                artnetPacketUniverse = 0;

                artnetSocket.NewPacket += Socket_NewPacket;
            }
        }
        public void Connect2Artnet()
        {
            string myLocalAddress = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((item.OperationalStatus == OperationalStatus.Up) && (item.Name == "Ethernet"))
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && (ip.Address.AddressFamily == AddressFamily.InterNetwork))
                        {
                            myLocalAddress = ip.Address.ToString();
                        }
                    }
                }
            }
            artnetSocket.Open(localIP, subnetMask); // localIP // IPAddress.Parse(myLocalAddress)
            artnetConnected.Invoke(this, hardwareID);
        }
        public void Disconnect2Artnet()
        {
            try
            {
                artnetSocket.Disconnect(false);
            }
            catch (Exception)
            {

            }
        }

        // Private methods
        private void Socket_NewPacket(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            switch (e.Packet.OpCode)
            {
                case ArtNetOpCodes.ArtTrigger:
                    {

                    }
                    break;
                case ArtNetOpCodes.PollReply:
                    {

                    }
                    break;
                default:
                    {
                        MessageBox.Show("Received ArtNet packet with OpCode: " + e.Packet.OpCode.ToString() + " from " + e.Source.ToString());
                    }
                    break;
            }
        }

        // Public methods
        public int Get_HardwareID()
        {
            return hardwareID;
        }

        public bool Get_ConnectionStatus()
        {
            return artnetSocket.Connected;
        }
        public bool Get_PortOpenStatus()
        {
            return artnetSocket.PortOpen;
        }
        public void Dispose()
        {
            artnetSocket.Close();
            artnetSocket.Dispose();
        }

        public byte[] SendArtTrigger(short oemCode, byte key, byte subKey)
        {
            ArtTriggerPacket packetBuf = new ArtTriggerPacket
            {
                OemCode = oemCode,
                Key = key,
                SubKey = subKey,
                Data = new byte[512]
            };
            artnetSocket.Send(packetBuf);

            return packetBuf.ToArray();
        }
        public byte[] SendArtnetPoll(byte _talkToMe)
        {
            ArtPollPacket packetBuf = new ArtPollPacket
            {
                TalkToMe = _talkToMe
            };
            artnetSocket.Send(packetBuf);

            return packetBuf.ToArray();
        }
        public byte[] SendArtnetDmx(List<Data.Data_Config_Device_LED_ArtnetDMX> newListLED)
        {
            // Creat LED data buffer
            byte[] dmxData = new byte[512];
            for (int i = 0; i < newListLED.Count; i++)
            {
                dmxData[((newListLED[i].devID - 1) * 3)] = (byte)newListLED[i].red;
                dmxData[((newListLED[i].devID - 1) * 3) + 1] = (byte)newListLED[i].green;
                dmxData[((newListLED[i].devID - 1) * 3) + 2] = (byte)newListLED[i].blue;
            }
            artnetPacketSequence += 1;
            // Creat packet buf
            ArtNetDmxPacket packetBuf = new ArtNetDmxPacket
            {
                Sequence = artnetPacketSequence,
                Physical = artnetPacketPhysical,
                Universe = artnetPacketUniverse,
                DmxData = dmxData
            };
            // Send data
            artnetSocket.Send(packetBuf);
            return packetBuf.ToArray();
        }
        public byte[] TestArtnetDmx(byte[] dmxData)
        {
            artnetPacketSequence += 1;

            ArtNetDmxPacket packetBuf = new ArtNetDmxPacket
            {
                Sequence = artnetPacketSequence,
                Physical = artnetPacketPhysical,
                Universe = artnetPacketUniverse,
                DmxData = dmxData
            };
            artnetSocket.Send(packetBuf);

            return packetBuf.ToArray();
        }
    }
}
