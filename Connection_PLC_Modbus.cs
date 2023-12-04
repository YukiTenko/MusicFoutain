using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NModbus;
using NModbus.Extensions.Enron;
using NModbus.Utility;

namespace MusicFountain.Connection
{
    public partial class Connection_PLC_Modbus
    {
        // Constants
        const ushort VALVE_STEP_CONFIG_MODBUS_START_ADDRESS = 116;
        const int VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE = 8;
        const int VALVE_STEP_CONFIG_MODBUS_NUMBER_OF_VALVE = 4;
        const int VALVE_STEP_CONFIG_MODBUS_COORD_ANGLE = 0;
        const int VALVE_STEP_CONFIG_MODBUS_MAX_ANGLE = 2;
        const int VALVE_STEP_CONFIG_MODBUS_MAX_SPEED = 4;
        const int VALVE_STEP_CONFIG_MODBUS_RATIO = 6;

        const ushort VALVE_STEP_DATA_MODBUS_START_ADDRESS = 148;
        const int VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE = 12;
        const int VALVE_STEP_DATA_MODBUS_NUMBER_OF_VALVE = 4;
        const int VALVE_STEP_DATA_MODBUS_ENABLE = 0;
        const int VALVE_STEP_DATA_MODBUS_RESET = 2;
        const int VALVE_STEP_DATA_MODBUS_SET_SPEED = 4;
        const int VALVE_STEP_DATA_MODBUS_SET_ANGLE = 8;

        const ushort VALVE_ONOFF_MODBUS_START_ADDRESS = 196;
        const int VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE = 6;
        const int VALVE_ONOFF_NUMBER_OF_VALVE = 16;
        const int VALVE_ONOFF_MODBUS_ENABLE = 0;
        const int VALVE_ONOFF_MODBUS_RESET = 2;
        const int VALVE_ONOFF_MODBUS_SENDFLAG = 4;
        const int VALVE_ONOFF_MODBUS_DATA = 5;

        const ushort LED_485_MODBUS_ADDRESS = 211;
        const int LED_485_MODBUS_LENGTH = 71;
        const int LED_485_MODBUS_ENABLE = 0;
        //const int LED_485_MODBUS_READY = 1;
        const int LED_485_MODBUS_RESET = 2;
        //const int LED_485_MODBUS_ERROR = 3;
        const int LED_485_MODBUS_REQUEST_FLAG = 4;
        const int LED_485_MODBUS_REQUEST_DATA = 5;
        const int LED_485_MODBUS_REQUEST_DATA_LENGTH = LED_485_MODBUS_LENGTH - LED_485_MODBUS_REQUEST_DATA;
        const int LED_485_MODBUS_LED_ID_MAX = 500;


        const ushort PUMP_DIGITAL_MODBUS_ADDRESS = 0;
        const int PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP = 6;
        const int PUMP_DIGITAL_NUMBER_OF_PUMP = 5;
        const int PUMP_DIGITAL_MODBUS_ENABLE = 0;
        const int PUMP_DIGITAL_MODBUS_RESET = 2;
        const int PUMP_DIGITAL_MODBUS_SETSTATUS = 4;
        const int PUMP_DIGITAL_MODBUS_CURRENTSTATUS = 5;

        const ushort PUMP_ANALOG_CONFIG_MODBUS_ADDRESS = 30;
        const int PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP = 4;
        const int PUMP_ANALOG_CONFIG_MODBUS_NUMBER_OF_PUMP = 4;
        const int PUMP_ANALOG_CONFIG_MODBUS_MIN_FREQ = 0;
        const int PUMP_ANALOG_CONFIG_MODBUS_MAX_FREQ = 2;

        const ushort PUMP_ANALOG_DATA_MODBUS_ADDRESS = 46;
        const int PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP = 16;
        const int PUMP_ANALOG_DATA_MODBUS_NUMBER_OF_PUMP = 4;
        const int PUMP_ANALOG_DATA_MODBUS_ENABLE = 0;
        const int PUMP_ANALOG_DATA_MODBUS_RESET = 2;
        const int PUMP_ANALOG_DATA_MODBUS_SETMODE = 4;
        const int PUMP_ANALOG_DATA_MODBUS_MANUAL_FREQ = 5;
        const int PUMP_ANALOG_DATA_MODBUS_MANUAL_CURRENTFREQ = 7;
        const int PUMP_ANALOG_DATA_MODBUS_SIN_MINFREQ = 9;
        const int PUMP_ANALOG_DATA_MODBUS_SIN_MAXFREQ = 11;
        const int PUMP_ANALOG_DATA_MODBUS_SIN_CYCLE = 13;
        const int PUMP_ANALOG_DATA_MODBUS_SIN_PHASE = 14;

        const ushort SYSTEMPOWER_MODBUS_ADDRESS = 110;
        const int SYSTEMPOWER_MODBUS_LENGTH = 6;
        const ushort SYSTEMPOWER_MODBUS_ENABLE = 0;
        const ushort SYSTEMPOWER_MODBUS_RESET = 2;
        const ushort SYSTEMPOWER_MODBUS_SETSTATUS = 4;
        const ushort SYSTEMPOWER_MODBUS_CURRENTSTATUS = 5;

        const int MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT = 20;

        const int MODBUS_ALL_DATA_LENGTH = 282;
        const int MODBUS_MAX_DATA_IN_A_MESSAGE = 120;

        // Variables
        //UdpClient udpClient;
        TcpClient tcpClient;
        ModbusFactory modbusFactory;
        IModbusMaster iModbusMaster;

        ushort[] allData;

        private Thread connectionThread;
        private bool running;
        private bool modbusNeedSendData;

        int hardwareID;
        string serverURL;
        int serverPort;
        int slaveID;

        bool modbusReady2Send;
        int connectedCount;
        int disconnectedCount;

        // Events
        public event EventHandler<int> plcModbusConnected;
        public event EventHandler<int> plcModbusConnecting;
        public event EventHandler<int> plcModbusDisconnected;

        public Connection_PLC_Modbus(int newHardwareID, string newServerUrl, int newServerPort, int newSlaveID){
            {// Creat variables
                //udpClient = null;
                tcpClient = null;
                modbusFactory = null;
                iModbusMaster = null;

                allData = new ushort[MODBUS_ALL_DATA_LENGTH];

                hardwareID = newHardwareID;
                serverURL = newServerUrl;
                serverPort = newServerPort;
                slaveID = newSlaveID;

                modbusReady2Send = true;
                connectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                disconnectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
            }
            {
                connectionThread = new Thread(Connection_Thread);
                running = true;
                modbusNeedSendData = false;
            }
        }

        // Public methods
        public void Connect()
        {
            Disconnect();
            plcModbusConnecting.Invoke(this, hardwareID);
            try
            {
                tcpClient = new TcpClient(serverURL, serverPort);
                //udpClient = new UdpClient(serverURL, serverPort);
                if (tcpClient != null)
                {
                    modbusFactory = new ModbusFactory();
                    if (modbusFactory != null)
                    {
                        iModbusMaster = modbusFactory.CreateMaster(tcpClient);
                        iModbusMaster.Transport.Retries = 0;
                        iModbusMaster.Transport.ReadTimeout = 50;
                        iModbusMaster.Transport.WriteTimeout = 50;
                        if (iModbusMaster != null)
                        {
                            plcModbusConnected.Invoke(this, hardwareID);
                            try
                            {
                                // Now read all data for first time
                                for (int i = 0; (i * MODBUS_MAX_DATA_IN_A_MESSAGE) < MODBUS_ALL_DATA_LENGTH; i++)
                                {
                                    int endByteCount = (i + 1) * MODBUS_MAX_DATA_IN_A_MESSAGE;
                                    if (endByteCount > MODBUS_ALL_DATA_LENGTH)
                                    {
                                        endByteCount = MODBUS_ALL_DATA_LENGTH;
                                    }

                                    int startAddressBuf = (i * MODBUS_MAX_DATA_IN_A_MESSAGE);
                                    if (startAddressBuf < 0)
                                    {
                                        startAddressBuf = 0;
                                    }

                                    ushort[] readMessageBuf = Modbus_ReadMultipleRegisters((byte)slaveID, (ushort)startAddressBuf, (ushort)(endByteCount - startAddressBuf));
                                    if (readMessageBuf != null)
                                    {
                                        for (int j = 0; j < endByteCount - (i * MODBUS_MAX_DATA_IN_A_MESSAGE); j++)
                                        {
                                            allData[startAddressBuf + j] = readMessageBuf[j];
                                        }
                                    }
                                    Thread.Sleep(10);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể kết nối đến Modbus TCP Server theo địa chỉ: " + serverURL + ":" + serverPort.ToString());
            }
        }
        public void Disconnect()
        {
            try
            {
                //udpClient.Close();
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
                iModbusMaster = null;
                modbusFactory = null;
                //udpClient = null;
                tcpClient = null;
                plcModbusDisconnected.Invoke(this, hardwareID);
            }
            catch (Exception)
            {
                MessageBox.Show("Xảy ra lỗi khi đóng kết nối MODBUS TCP/IP đến địa chỉ: " + serverURL);
            }
        }

        public void Thread_Start()
        {
            running = true;
            connectionThread.Start();
        }
        public void Thread_Stop()
        {
            running = false;
        }

        public int Get_HardwareID()
        {
            return hardwareID;
        }

        public void Change_ValveStep_PLC_Config(Data.Data_Config_Hardware_Option_VS_PLC newOptionVSConfig)
        {
            ushort[] vsPLCConfig = new ushort[VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE * VALVE_STEP_CONFIG_MODBUS_NUMBER_OF_VALVE];
            //ushort[] vsPLCConfig = Modbus_ReadMultipleRegisters((byte)slaveID, VALVE_STEP_CONFIG_MODBUS_START_ADDRESS, VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE * VALVE_STEP_CONFIG_MODBUS_NUMBER_OF_VALVE);
            if (vsPLCConfig != null) {
                for (int i = 0; i < VALVE_STEP_CONFIG_MODBUS_NUMBER_OF_VALVE; i++)
                {
                    byte[] coordAngleBuf = BitConverter.GetBytes(newOptionVSConfig.coordAngle);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_COORD_ANGLE + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE] = BitConverter.ToUInt16(coordAngleBuf, 2);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_COORD_ANGLE + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE + 1] = BitConverter.ToUInt16(coordAngleBuf, 0);

                    byte[] maxAngleBuf = BitConverter.GetBytes(newOptionVSConfig.maxAngle);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_MAX_ANGLE + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE] = BitConverter.ToUInt16(maxAngleBuf, 2);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_MAX_ANGLE + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE + 1] = BitConverter.ToUInt16(maxAngleBuf, 0);

                    byte[] maxSpeedBuf = BitConverter.GetBytes(newOptionVSConfig.maxSpeed);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_MAX_SPEED + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE] = BitConverter.ToUInt16(maxSpeedBuf, 2);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_MAX_SPEED + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE + 1] = BitConverter.ToUInt16(maxSpeedBuf, 0);

                    byte[] ratioBuf = BitConverter.GetBytes(newOptionVSConfig.ratio);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_RATIO + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE] = BitConverter.ToUInt16(ratioBuf, 2);
                    vsPLCConfig[VALVE_STEP_CONFIG_MODBUS_RATIO + i * VALVE_STEP_CONFIG_MODBUS_LENGTH_OF_ONE_VALVE + 1] = BitConverter.ToUInt16(ratioBuf, 0);
                }
                // Send data
                for (int i = 0; i < vsPLCConfig.Length; i++)
                {
                    allData[VALVE_STEP_CONFIG_MODBUS_START_ADDRESS + i] = vsPLCConfig[i];
                }
                modbusNeedSendData = true;
                //Modbus_WriteMultipleRegisters((byte)slaveID, VALVE_STEP_CONFIG_MODBUS_START_ADDRESS, vsPLCConfig);
            }
        }
        public void Change_ValveStep_PLC_Data(List<Data.Data_Config_Device_VS_PLC> newListValve)
        {
            int numberOfModuleBuf = 0;
            for (int i = 0; i < newListValve.Count; i++)
            {
                if (numberOfModuleBuf < newListValve[i].devID)
                {
                    numberOfModuleBuf = newListValve[i].devID;
                }
            }
            if (numberOfModuleBuf > VALVE_STEP_DATA_MODBUS_NUMBER_OF_VALVE)
            {
                MessageBox.Show("Số lượng van trong cấu hình là: " + numberOfModuleBuf + " đang lớn hơn số van tối đa trên 1 PLC là: " + VALVE_STEP_DATA_MODBUS_NUMBER_OF_VALVE);
            }
            else
            {
                ushort[] vsPLCData = new ushort[VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE * VALVE_STEP_DATA_MODBUS_NUMBER_OF_VALVE];
                //ushort[] vsPLCData = Modbus_ReadMultipleRegisters((byte)slaveID, VALVE_STEP_DATA_MODBUS_START_ADDRESS, VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE * VALVE_STEP_DATA_MODBUS_NUMBER_OF_VALVE);
                if (vsPLCData != null)
                {
                    for (int i = 0; i < newListValve.Count; i++)
                    {// 16 valve/register
                        vsPLCData[VALVE_STEP_DATA_MODBUS_ENABLE + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE] = Convert.ToUInt16(newListValve[i].enable);
                        vsPLCData[VALVE_STEP_DATA_MODBUS_RESET + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE] = Convert.ToUInt16(newListValve[i].reset);

                        byte[] setSpeedBuf = BitConverter.GetBytes(newListValve[i].setSpeed);
                        vsPLCData[VALVE_STEP_DATA_MODBUS_SET_SPEED + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE] = BitConverter.ToUInt16(setSpeedBuf, 2);
                        vsPLCData[VALVE_STEP_DATA_MODBUS_SET_SPEED + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE + 1] = BitConverter.ToUInt16(setSpeedBuf, 0);

                        byte[] setAngleBuf = BitConverter.GetBytes(newListValve[i].setAngle);
                        vsPLCData[VALVE_STEP_DATA_MODBUS_SET_ANGLE + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE] = BitConverter.ToUInt16(setAngleBuf, 2);
                        vsPLCData[VALVE_STEP_DATA_MODBUS_SET_ANGLE + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE + 1] = BitConverter.ToUInt16(setAngleBuf, 0);

                        // Update data to buffer
                        for (int j = 0; j < VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE; j++)
                        {
                            allData[VALVE_STEP_DATA_MODBUS_START_ADDRESS + (newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE + j] = vsPLCData[(newListValve[i].devID - 1) * VALVE_STEP_DATA_MODBUS_LENGTH_OF_ONE_VALVE + j];
                        }
                    }
                    // Send data
                    modbusNeedSendData = true;
                    //Modbus_WriteMultipleRegisters((byte)slaveID, VALVE_STEP_DATA_MODBUS_START_ADDRESS, vsPLCData);
                }
            }
        }
        public void Change_ValveOnOff_PLC_Data(List<Data.Data_Config_Device_VO_PLC> newListValve)
        {
            int numberOfModuleBuf = 0;
            for (int i = 0; i < newListValve.Count; i++)
            {
                if (numberOfModuleBuf < (((newListValve[i].devID - 1) / VALVE_ONOFF_NUMBER_OF_VALVE) + 1))
                {
                    numberOfModuleBuf = (newListValve[i].devID - 1) / VALVE_ONOFF_NUMBER_OF_VALVE + 1;
                }
            }
            ushort[] voPLCData = new ushort[VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE * numberOfModuleBuf];
            //ushort[] voPLCData = Modbus_ReadMultipleRegisters((byte)slaveID, VALVE_ONOFF_MODBUS_START_ADDRESS, (ushort)(VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE * numberOfModuleBuf));
            if (voPLCData != null)
            {
                for (int i = 0; i < newListValve.Count; i++)
                {// 16 valve/register
                    voPLCData[VALVE_ONOFF_MODBUS_ENABLE + (newListValve[i].devID - 1) / VALVE_ONOFF_NUMBER_OF_VALVE * VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE] = 1;
                    voPLCData[VALVE_ONOFF_MODBUS_RESET + (newListValve[i].devID - 1) / VALVE_ONOFF_NUMBER_OF_VALVE * VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE] = 0;
                    voPLCData[VALVE_ONOFF_MODBUS_SENDFLAG + (newListValve[i].devID - 1) / VALVE_ONOFF_NUMBER_OF_VALVE * VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE] = 1;

                    int registerDataBuf = allData[VALVE_ONOFF_MODBUS_START_ADDRESS + VALVE_ONOFF_MODBUS_DATA]; // Get old data of 16 valve
                    if (newListValve[i].setOnOff)
                    {
                        registerDataBuf = registerDataBuf | (int)(Math.Pow(2, (newListValve[i].devID - 1) % VALVE_ONOFF_NUMBER_OF_VALVE) + 0);
                    }
                    else
                    {
                        registerDataBuf = registerDataBuf & (int)(65535 - Math.Pow(2, (newListValve[i].devID - 1) % VALVE_ONOFF_NUMBER_OF_VALVE));
                    }
                    voPLCData[VALVE_ONOFF_MODBUS_DATA] = (ushort)registerDataBuf;

                    // Update data to buffer
                    for (int j = 0; j < VALVE_ONOFF_MODBUS_LENGTH_OF_ONE_VALVE; j++)
                    {
                        allData[VALVE_ONOFF_MODBUS_START_ADDRESS + j] = voPLCData[j];
                    }
                }
                // Send data
                modbusNeedSendData = true; 
                //Modbus_WriteMultipleRegisters((byte)slaveID, VALVE_ONOFF_MODBUS_START_ADDRESS, voPLCData);
            }
        }
        public void Change_LED485_PLC_Data(List<Data.Data_Config_Device_LED_PLC> newListLED, int red, int green, int blue)
        {// Creat message
            ushort[] ledPLCData = new ushort[LED_485_MODBUS_LENGTH];
            //ushort[] ledPLCData = Modbus_ReadMultipleRegisters((byte)slaveID, LED_485_MODBUS_ADDRESS, LED_485_MODBUS_LENGTH);
            if (ledPLCData != null)
            {
                ledPLCData[LED_485_MODBUS_ENABLE] = 1;
                ledPLCData[LED_485_MODBUS_RESET] = 0;
                ledPLCData[LED_485_MODBUS_REQUEST_FLAG] = 1;
                {// Creat message
                    List<int> messageBuf = new List<int>();
                    {// Creat message header
                        messageBuf.Add(64); // @
                        messageBuf.Add(48); // 0
                        messageBuf.Add(56); // 8 // Ma lenh 08
                        messageBuf.Add(red + green * 2 + blue * 4 + 48); // Tinh ma mau, gia tri toi da la 7 nen khong can check 0-9 hhoac A-F
                    }
                    {// Fill data of LED to message
                        for (int i = 0; i < (LED_485_MODBUS_LED_ID_MAX/4); i++)
                        {
                            messageBuf.Add(0);
                        }
                        for (int i = 0; i < newListLED.Count; i++)
                        {
                            if ((newListLED[i].state == 1) && (newListLED[i].devID < LED_485_MODBUS_LED_ID_MAX))
                            {
                                int ledDataPosition = ((newListLED[i].devID - 1) / 8) * 2 + 1 - (((newListLED[i].devID - 1) % 8) / 4);
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
                    {// Copy message to modbus data buf
                        if (messageBuf.Count > (LED_485_MODBUS_REQUEST_DATA_LENGTH * 2))
                        {
                            MessageBox.Show("Length of control LED message is longer than max MODBUS buffer");
                        }
                        else
                        {
                            for (int i = LED_485_MODBUS_REQUEST_DATA; i < LED_485_MODBUS_LENGTH; i++)
                            {
                                ledPLCData[i] = 0;
                            }
                            for (int i = 0; i < messageBuf.Count; i += 2)
                            {
                                ledPLCData[LED_485_MODBUS_REQUEST_DATA + (i / 2)] = (ushort)(messageBuf[i + 1] + messageBuf[i] * 256);
                            }
                        }
                    }
                }
                // Send data
                for (int i = 0; i < ledPLCData.Length; i++)
                {
                    allData[LED_485_MODBUS_ADDRESS + i] = ledPLCData[i];
                }
                modbusNeedSendData = true;
                //Modbus_WriteMultipleRegisters((byte)slaveID, LED_485_MODBUS_ADDRESS, ledPLCData);
            }
        }
        public void Change_PumpDigital_PLC_Data(List<Data.Data_Config_Device_Inverter> newListPump)
        {
            ushort[] pumpDigitalPLCData = new ushort[PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP * PUMP_DIGITAL_NUMBER_OF_PUMP];
            //ushort[] pumpDigitalPLCData = Modbus_ReadMultipleRegisters((byte)slaveID, PUMP_DIGITAL_MODBUS_ADDRESS, PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP * PUMP_DIGITAL_NUMBER_OF_PUMP);
            if (pumpDigitalPLCData != null)
            {
                for (int i = 0; i < newListPump.Count; i++)
                {
                    pumpDigitalPLCData[PUMP_DIGITAL_MODBUS_ENABLE + (newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP] = Convert.ToUInt16(newListPump[i].enable);
                    pumpDigitalPLCData[PUMP_DIGITAL_MODBUS_RESET + (newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP] = Convert.ToUInt16(newListPump[i].reset);
                    pumpDigitalPLCData[PUMP_DIGITAL_MODBUS_SETSTATUS + (newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP] = Convert.ToUInt16(newListPump[i].setStatus);
                    pumpDigitalPLCData[PUMP_DIGITAL_MODBUS_CURRENTSTATUS + (newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP] = (ushort)(1 - pumpDigitalPLCData[PUMP_DIGITAL_MODBUS_SETSTATUS + (newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP]);

                    // Update data to buffer
                    for (int j = 0; j < PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP; j++)
                    {
                        allData[PUMP_DIGITAL_MODBUS_ADDRESS + (newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP + j] = pumpDigitalPLCData[(newListPump[i].devID - 1) * PUMP_DIGITAL_MODBUS_LENGTH_OF_ONE_PUMP + j];
                    }
                }
                // Send data
                modbusNeedSendData = true;
                //Modbus_WriteMultipleRegisters((byte)slaveID, PUMP_DIGITAL_MODBUS_ADDRESS, pumpDigitalPLCData);
            }
        }
        public void Change_PumpAnalog_PLC_Config(Data.Data_Config_Hardware_Option_Inverter newOptionInverter)
        {
            ushort[] pumpAnalogPLCConfig = new ushort[PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP * PUMP_ANALOG_CONFIG_MODBUS_NUMBER_OF_PUMP];
            //ushort[] pumpAnalogPLCConfig = Modbus_ReadMultipleRegisters((byte)slaveID, PUMP_ANALOG_CONFIG_MODBUS_ADDRESS, PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP * PUMP_ANALOG_CONFIG_MODBUS_NUMBER_OF_PUMP);
            if (pumpAnalogPLCConfig != null)
            {
                for (int i = 0; i < PUMP_ANALOG_CONFIG_MODBUS_NUMBER_OF_PUMP; i++)
                {
                    byte[] minFreqBuf = BitConverter.GetBytes(newOptionInverter.freqMin);
                    pumpAnalogPLCConfig[PUMP_ANALOG_CONFIG_MODBUS_MIN_FREQ + i * PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(minFreqBuf, 2);
                    pumpAnalogPLCConfig[PUMP_ANALOG_CONFIG_MODBUS_MIN_FREQ + i * PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(minFreqBuf, 0);

                    byte[] maxFreqBuf = BitConverter.GetBytes(newOptionInverter.freqMax);
                    pumpAnalogPLCConfig[PUMP_ANALOG_CONFIG_MODBUS_MAX_FREQ + i * PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(maxFreqBuf, 2);
                    pumpAnalogPLCConfig[PUMP_ANALOG_CONFIG_MODBUS_MAX_FREQ + i * PUMP_ANALOG_CONFIG_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(maxFreqBuf, 0);
                }
                // Send data
                for (int i = 0; i < pumpAnalogPLCConfig.Length; i++)
                {
                    allData[PUMP_ANALOG_CONFIG_MODBUS_ADDRESS + i] = pumpAnalogPLCConfig[i];
                }
                modbusNeedSendData = true;
                //Modbus_WriteMultipleRegisters((byte)slaveID, PUMP_ANALOG_CONFIG_MODBUS_ADDRESS, pumpAnalogPLCConfig);
            }
        }
        public void Change_PumpAnalog_PLC_Data(List<Data.Data_Config_Device_Inverter> newListPump)
        {
            ushort[] pumpAnalogPLCData = new ushort[PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP * PUMP_ANALOG_DATA_MODBUS_NUMBER_OF_PUMP];
            //ushort[] pumpAnalogPLCData = Modbus_ReadMultipleRegisters((byte)slaveID, PUMP_ANALOG_DATA_MODBUS_ADDRESS, PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP * PUMP_ANALOG_DATA_MODBUS_NUMBER_OF_PUMP);
            if (pumpAnalogPLCData != null)
            {
                for (int i = 0; i < newListPump.Count; i++)
                {
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_ENABLE + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = Convert.ToUInt16(newListPump[i].enable);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_RESET + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = Convert.ToUInt16(newListPump[i].reset);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SETMODE + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = Convert.ToUInt16(newListPump[i].setMode);

                    byte[] manFreqBuf = BitConverter.GetBytes(newListPump[i].manFreq);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_MANUAL_FREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(manFreqBuf, 2);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_MANUAL_FREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(manFreqBuf, 0);

                    byte[] manCurrentFreqBuf = BitConverter.GetBytes((float)-1);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_MANUAL_CURRENTFREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(manCurrentFreqBuf, 2);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_MANUAL_CURRENTFREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(manCurrentFreqBuf, 0);

                    byte[] sinMinFreqBuf = BitConverter.GetBytes(newListPump[i].sinAmplMin);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_MINFREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(sinMinFreqBuf, 2);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_MINFREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(sinMinFreqBuf, 0);

                    byte[] sinMaxFreqBuf = BitConverter.GetBytes(newListPump[i].sinAmplMax);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_MAXFREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(sinMaxFreqBuf, 2);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_MAXFREQ + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(sinMaxFreqBuf, 0);

                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_CYCLE + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = (ushort)(newListPump[i].sinFreqCycle);

                    byte[] sinPhaseBuf = BitConverter.GetBytes(newListPump[i].sinPhaseDiff);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_PHASE + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP] = BitConverter.ToUInt16(sinPhaseBuf, 2);
                    pumpAnalogPLCData[PUMP_ANALOG_DATA_MODBUS_SIN_PHASE + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + 1] = BitConverter.ToUInt16(sinPhaseBuf, 0);

                    // Update data to buffer
                    for (int j = 0; j < PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP; j++)
                    {
                        allData[PUMP_ANALOG_DATA_MODBUS_ADDRESS + (newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + j] = pumpAnalogPLCData[(newListPump[i].devID - 1) * PUMP_ANALOG_DATA_MODBUS_LENGTH_OF_ONE_PUMP + j];
                    }
                }
                // Send data
                modbusNeedSendData = true;
                //Modbus_WriteMultipleRegisters((byte)slaveID, PUMP_ANALOG_DATA_MODBUS_ADDRESS, pumpAnalogPLCData);
            }
        }
        public void Change_SystemPower_Data(bool newSystemPowerState)
        {// Creat message
            ushort[] systemPowerBuf = new ushort[SYSTEMPOWER_MODBUS_LENGTH];
            //ushort[] systemPowerBuf = Modbus_ReadMultipleRegisters((byte)slaveID, SYSTEMPOWER_MODBUS_ADDRESS, SYSTEMPOWER_MODBUS_LENGTH);
            if (systemPowerBuf != null)
            {
                systemPowerBuf[SYSTEMPOWER_MODBUS_ENABLE] = 1;
                systemPowerBuf[SYSTEMPOWER_MODBUS_RESET] = 0;
                systemPowerBuf[SYSTEMPOWER_MODBUS_SETSTATUS] = Convert.ToUInt16(newSystemPowerState);
                systemPowerBuf[SYSTEMPOWER_MODBUS_CURRENTSTATUS] = (ushort)(1 - systemPowerBuf[SYSTEMPOWER_MODBUS_SETSTATUS]);
                // Send data
                for (int i = 0; i < systemPowerBuf.Length; i++)
                {
                    allData[SYSTEMPOWER_MODBUS_ADDRESS + i] = systemPowerBuf[i];
                }
                modbusNeedSendData = true;
                //Modbus_WriteMultipleRegisters((byte)slaveID, SYSTEMPOWER_MODBUS_ADDRESS, systemPowerBuf);
            }
        }

        // Private methods
        private void Connection_Thread()
        {
            while (running)
            {
                try
                {
                    if (modbusNeedSendData)
                    {
                        modbusNeedSendData = false;
                        for (int i = 0; (i * MODBUS_MAX_DATA_IN_A_MESSAGE) < MODBUS_ALL_DATA_LENGTH; i++)
                        {
                            int endByteCount = (i + 1) * MODBUS_MAX_DATA_IN_A_MESSAGE;
                            if (endByteCount > MODBUS_ALL_DATA_LENGTH)
                            {
                                endByteCount = MODBUS_ALL_DATA_LENGTH;
                            }

                            int startAddressBuf = MODBUS_ALL_DATA_LENGTH - ((i + 1) * MODBUS_MAX_DATA_IN_A_MESSAGE);// For copy last block first
                            if (startAddressBuf < 0)
                            {
                                startAddressBuf = 0;
                            }

                            ushort[] sendMessageBuf = new ushort[endByteCount - (i * MODBUS_MAX_DATA_IN_A_MESSAGE)];
                            for (int j = 0; j < endByteCount - (i * MODBUS_MAX_DATA_IN_A_MESSAGE); j++)
                            {
                                sendMessageBuf[j] = allData[startAddressBuf + j];// Copy last block first
                            }

                            Modbus_WriteMultipleRegisters((byte)slaveID, (ushort)startAddressBuf, sendMessageBuf);
                            Thread.Sleep(1);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                }
            } 
        }

        private ushort[] Modbus_ReadMultipleRegisters(byte slaveID, ushort address, ushort quantity)
        {
            try
            {
                if (iModbusMaster != null)
                {
                    int timeOutCountMaxBuf = iModbusMaster.Transport.WriteTimeout * 3;
                    for (int i = 0; i < timeOutCountMaxBuf; i++) // Wait for max 3 message
                    {
                        if (modbusReady2Send)
                        {
                            i = timeOutCountMaxBuf;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    if (modbusReady2Send)
                    {
                        modbusReady2Send = false;
                        try
                        {
                            ushort[] returnBuf = iModbusMaster.ReadHoldingRegisters((byte)slaveID, address, quantity);
                            disconnectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                            connectedCount++;
                            if (connectedCount > MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT)
                            {
                                connectedCount = 0;
                                plcModbusConnected.Invoke(this, hardwareID);
                            }
                            modbusReady2Send = true;
                            return returnBuf;
                        }
                        catch (Exception e)
                        {
                            if (e.ToString().Contains("Unexpected start address in response") || e.ToString().Contains("Unable to read data from the transport connection") || e.ToString().Contains("Exception of type 'NModbus.SlaveException' was thrown") || e.ToString().Contains("Response was not of excepted transaction ID"))
                            {
                                // Ingore this
                            }
                            else
                            {
                                MessageBox.Show(e.ToString());
                                connectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                                disconnectedCount++;
                                if (disconnectedCount > MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT)
                                {
                                    disconnectedCount = 0;
                                    plcModbusDisconnected.Invoke(this, hardwareID);
                                }
                                modbusReady2Send = true;
                            }
                            return null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Đang gửi quá nhiều dữ liệu tới Modbus TCP Server theo địa chỉ: " + serverURL + ":" + serverPort.ToString() + " . Một số bức điện có thể sẽ bị bỏ qua .");
                    }
                }
            }
            catch (Exception)
            {
                connectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                disconnectedCount++;
                if (disconnectedCount > MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT)
                {
                    disconnectedCount = 0;
                    plcModbusDisconnected.Invoke(this, hardwareID);
                }
            }
            return null;
        }
        private void Modbus_WriteMultipleRegisters(byte slaveID, ushort address, ushort[] data)
        {
            try
            {
                if (iModbusMaster != null)
                {
                    int timeOutCountMaxBuf = iModbusMaster.Transport.WriteTimeout * 3;
                    for (int i = 0; i < timeOutCountMaxBuf; i++) // Wait for max 3 message
                    {
                        if (modbusReady2Send)
                        {
                            i = timeOutCountMaxBuf;
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                    if (modbusReady2Send)
                    {
                        modbusReady2Send = false;
                        try
                        {
                            iModbusMaster.WriteMultipleRegisters((byte)slaveID, address, data);
                            disconnectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                            connectedCount++;
                            if (connectedCount > MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT)
                            {
                                connectedCount = 0;
                                plcModbusConnected.Invoke(this, hardwareID);
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.ToString().Contains("Unexpected start address in response") || e.ToString().Contains("Unable to read data from the transport connection") || e.ToString().Contains("Exception of type 'NModbus.SlaveException' was thrown") || e.ToString().Contains("Response was not of expected transaction ID"))
                            {
                                // Ingore this
                            }
                            else
                            {
                                MessageBox.Show(e.ToString());
                                connectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                                disconnectedCount++;
                                if (disconnectedCount > MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT)
                                {
                                    disconnectedCount = 0;
                                    plcModbusDisconnected.Invoke(this, hardwareID);
                                }
                            }
                        }
                        modbusReady2Send = true; ;
                    }
                    else
                    {
                        MessageBox.Show("Đang gửi quá nhiều dữ liệu tới Modbus TCP Server theo địa chỉ: " + serverURL + ":" + serverPort.ToString() + " . Một số bức điện có thể sẽ bị bỏ qua .");
                    }
                }
            }
            catch (Exception)
            {
                connectedCount = MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT;
                disconnectedCount++;
                if (disconnectedCount > MODBUS_CONNECTION_STATE_INVOKE_EVENT_COUNT)
                {
                    disconnectedCount = 0;
                    plcModbusDisconnected.Invoke(this, hardwareID);
                }
            }
        }
    }
}
