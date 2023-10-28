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

namespace MusicFountain
{
    public partial class Connection_Artnet : IDisposable
    {
        // Variables
        private IPAddress localIP;
        private IPAddress subnetMask;

        private ArtNetSocket artnetSocket;

        private byte artnetPacketSequence;
        private byte artnetPacketPhysical;
        private byte artnetPacketUniverse;

        // Events

        public Connection_Artnet(IPAddress newIP, IPAddress newSubnetMark)
        {
            {// Creat variale
                localIP = newIP;
                subnetMask = newSubnetMark;

                artnetSocket = new ArtNetSocket
                {
                    EnableBroadcast = true
                };

                artnetPacketSequence = 0;
                artnetPacketPhysical = 1;
                artnetPacketUniverse = 0;
            }

            {// Artnet socket connect
                artnetSocket.NewPacket += Socket_NewPacket;
                artnetSocket.Open(localIP, subnetMask);
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
        public byte[] SendArtnetDmx(byte[] dmxData)
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
