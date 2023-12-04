using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicFountain
{
    public class Connection_Hardware
    {
        public Connection.Connection_PLC connectionPLC;
        public Connection.Connection_PLC_Modbus connectionPLCModbus;
        public Connection.Connection_Artnet connectionArtNet;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Data
        private Data.Data_Config dataConfig;
        private Data.Data_Log dataLog;
        private Data.Data_Login dataLogin;

        // UI
        //private UI_Login uiLogin;
        private UI.UI_Wellcome uiWellCome;
        private UI.Hardware.UI_HardwareConfig uiHardwareConfig;
        private UI.Effect.UI_EffectEditor uiEffectEditor;
        private UI.Music.UI_MusicEditor uiMusicEditor;
        private UI.Playlist.UI_Playlist uiPlaylist;
        private UI.Log.UI_Log uiLog;

        // Connection
        private List<Connection_Hardware> listConnectionHardware;

        // Control algorithm
        private List<Control_Group> listControlGroup;
        private Control_Music controlMusic;

        public MainWindow()
        {
            InitializeComponent();

            // Creat all variables
            {
                // Data backends
                dataConfig = new Data.Data_Config();
                dataLog = new Data.Data_Log();
                dataLogin = new Data.Data_Login();

                // UIs
                uiWellCome = new UI.UI_Wellcome();
                uiHardwareConfig = new UI.Hardware.UI_HardwareConfig();
                uiEffectEditor = new UI.Effect.UI_EffectEditor();
                uiMusicEditor = new UI.Music.UI_MusicEditor();
                uiPlaylist = new UI.Playlist.UI_Playlist();
                uiLog = new UI.Log.UI_Log();

                // List connection
                listConnectionHardware = new List<Connection_Hardware>();

                // List control group
                listControlGroup = new List<Control_Group>();
                controlMusic = new Control_Music(null);
            }

            // Update config and log data
            {

            }

            // Creat UI
            {
                // Hardware config
                uiHardwareConfig.updateNewListHardwareEvent += UIConfigHardware_ListHardwareSaveButtonClick_EventHandle;
                uiHardwareConfig.reconnectHardwareEvent += UIConfigHardware_ReconnectHardwareButtonClick_EventHandle;
                uiHardwareConfig.groupTestClick += UIConfigHardware_GroupTestClick_EventHandle;
                uiHardwareConfig.groupSaveClick += UIConfigHardware_GroupSaveClick_EventHandle;
                uiHardwareConfig.updateHardwareOption += UIConfigHardware_HardwareOptionSaveClick_EventHandle;
                {// Update hardware config
                    uiHardwareConfig.Update_HardwareConfig(dataConfig.Get_DataConfigHardware());
                }

                // Effect editor
                uiEffectEditor.saveEffectListClick += UIEffectEditor_SaveEffectListClick_EventHandle;
                {// Update group name list and music config list for editor
                    List<Data.Data_Config_Hardware_Group> listGroupBufForEditor = new List<Data.Data_Config_Hardware_Group>();
                    listGroupBufForEditor.Clear();
                    for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                    {
                        for (int j = 0; j < dataConfig.Get_DataConfigHardware().listHardware[i].listGroup.Count; j++)
                        {
                            listGroupBufForEditor.Add(dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j]);
                        }
                    }
                    uiEffectEditor.UpdateListGroupConfig(listGroupBufForEditor);
                    // list effect
                    uiEffectEditor.UpdateListEffectConfig(dataConfig.Get_DataConfigPlaylist().listEffect);
                }

                // Music editor
                uiMusicEditor.playMusicClick += UIMusicEditor_PlayMusicClick_EventHandle;
                uiMusicEditor.stopMusicClick += UIMusicEditor_StopMusicClick_EventHandle;
                uiMusicEditor.playMusicTimeIntervalUpdate += UIMusicEditor_PlayMusicTimeUpdate_EventHandle;
                uiMusicEditor.saveMusicEffectClick += UIMusicEditor_SaveMusicEffectClick_EventHandle;
                uiMusicEditor.Update_MusicTime_ThreadStart();
                {// Update group name list and music config list for editor
                    List<Data.Data_Config_Hardware_Group> listGroupBufForEditor = new List<Data.Data_Config_Hardware_Group>();
                    listGroupBufForEditor.Clear();
                    for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                    {
                        for (int j = 0; j < dataConfig.Get_DataConfigHardware().listHardware[i].listGroup.Count; j++)
                        {
                            listGroupBufForEditor.Add(dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j]);
                        }
                    }
                    uiMusicEditor.UpdateListGroupConfig(listGroupBufForEditor);
                    // list effect
                    uiMusicEditor.UpdateListEffectConfig(dataConfig.Get_DataConfigPlaylist().listEffect);
                    // list music
                    uiMusicEditor.UpdateListMusicConfig(dataConfig.Get_DataConfigPlaylist().listMusic);
                }

                // Playlist
                uiPlaylist.playMusicClick += UIMusicEditor_PlayMusicClick_EventHandle;
                uiPlaylist.stopMusicClick += UIMusicEditor_StopMusicClick_EventHandle;
                uiPlaylist.playlistModeSaveClick += UIPlayList_PlaylistModeSave_EventHandle;
                uiPlaylist.playMusicTimeIntervalUpdate += UIMusicEditor_PlayMusicTimeUpdate_EventHandle;
                uiPlaylist.Update_MusicTime_ThreadStart();
                {// Update music config list and playlist mode

                    List<Data.Data_Config_Hardware_Group> listGroupBufForEditor = new List<Data.Data_Config_Hardware_Group>();
                    listGroupBufForEditor.Clear();
                    for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                    {
                        for (int j = 0; j < dataConfig.Get_DataConfigHardware().listHardware[i].listGroup.Count; j++)
                        {
                            listGroupBufForEditor.Add(dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j]);
                        }
                    }
                    uiPlaylist.UpdateListGroupConfig(listGroupBufForEditor);
                    // Update list music and mode
                    uiPlaylist.UpdateListMusicConfig(dataConfig.Get_DataConfigPlaylist().listMusic);
                    uiPlaylist.UpdatePlaylistMode(dataConfig.Get_DataConfigPlaylist().playlistMode);
                }

                // Log
                uiLog.clearLogClick += UILog_ClearLogClick_EventHandle;
                uiLog.chosenLogFileChanged += UILog_ChosenLogFileChanged_EventHandle;

                // Setup mainLayout
                mainLayout.Content = uiWellCome;
                mainLayout.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                mainLayout.NavigationService.RemoveBackEntry();
            }

            // Creat connection
            {
                listConnectionHardware.Clear();
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    dataConfig.Get_DataConfigHardware().listHardware[i].serverConnectionState = "Disconnected";
                    Connection_Hardware connectionHardware = new Connection_Hardware();
                    switch (dataConfig.Get_DataConfigHardware().listHardware[i].type)
                    {
                        case "Valve On/Off - PLC":
                        case "Pump A/D & Power - PLC":
                        case "LED 485 - PLC":
                        case "Valve Stepper - PLC":
                        case "Valve On/Off & Stepper - PLC":
                        case "Valve Stepper & LED 485 - PLC":
                            {
                                connectionHardware.connectionPLCModbus = new Connection.Connection_PLC_Modbus(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID, dataConfig.Get_DataConfigHardware().listHardware[i].modbusServerURL, 502, 1);
                                connectionHardware.connectionPLCModbus.plcModbusConnected += Connection_Connected_EventHandle;
                                connectionHardware.connectionPLCModbus.plcModbusConnecting += Connection_Connecting_EventHandle;
                                connectionHardware.connectionPLCModbus.plcModbusDisconnected += Connection_Disconnected_EventHandle;
                                //connectionHardware.connectionPLCModbus.Connect();
                                if (dataConfig.Get_DataConfigHardware().listHardware[i].optionInverter != null)
                                {
                                    connectionHardware.connectionPLCModbus.Change_PumpAnalog_PLC_Config(dataConfig.Get_DataConfigHardware().listHardware[i].optionInverter);
                                }
                                if (dataConfig.Get_DataConfigHardware().listHardware[i].optionVS_PLC != null)
                                {
                                    connectionHardware.connectionPLCModbus.Change_ValveStep_PLC_Config(dataConfig.Get_DataConfigHardware().listHardware[i].optionVS_PLC);
                                }
                                connectionHardware.connectionPLCModbus.Thread_Start();
                            }
                            break;
                        case "LED DMX - Artnet":
                            {
                                connectionHardware.connectionArtNet = new Connection.Connection_Artnet(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID, IPAddress.Parse(dataConfig.Get_DataConfigHardware().listHardware[i].artnetServerURL), IPAddress.Parse("255.255.255.0"));
                                connectionHardware.connectionArtNet.artnetConnected += Connection_Connected_EventHandle;
                                //connectionHardware.connectionArtNet.Connect2Artnet();
                            }
                            break;
                        default:
                            {
                                MessageBox.Show("Creat connection: Unknown hardwareType: " + dataConfig.Get_DataConfigHardware().listHardware[i].type + ", hardwareID: " + dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID.ToString());
                            }
                            break;
                    }
                    listConnectionHardware.Add(connectionHardware);
                }
            }

            // Creat control algorithm
            {
                listControlGroup.Clear();
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    for (int j = 0; j < dataConfig.Get_DataConfigHardware().listHardware[i].listGroup.Count; j++)
                    {
                        Control_Group newControlGroup = new Control_Group(dataConfig.Get_DataConfigHardware().listHardware[i], dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j]);
                        switch (dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].type)
                        {
                            case "Valve Stepper - PLC":
                            case "Valve On/Off - PLC":
                            case "Pump Digital - PLC":
                            case "Pump Analog - PLC":
                            case "Pump Analog Dual - PLC":
                            case "LED 485 - PLC":
                            case "LED DMX - Artnet":
                            case "System Power":
                                {
                                    newControlGroup.Thread_Start();
                                }
                                break;
                            default:
                                {
                                    MessageBox.Show("Creat control: Unknown groupType: " + dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].type + ", hardwareID: " + dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].hardwareID.ToString() + ", groupID: " + dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].groupID.ToString());
                                }
                                break;
                        }
                        newControlGroup.changeVOPLC_Data += ControlGroup_ChangeVOPLCData;
                        newControlGroup.changeVSPLC_Data += ControlGroup_ChangeVSPLCData;
                        newControlGroup.changePumpDigital_Data += ControlGroup_ChangePumpDigitalData;
                        newControlGroup.changePumpAnalog_Data += ControlGroup_ChangePumpAnalogData;
                        newControlGroup.changeLEDArtnet_Data += ControlGroup_ChangeLedArtnetData;
                        newControlGroup.changeLEDPLC_Data += ControlGroup_ChangeLEDPLCData;
                        newControlGroup.changeSystemPower_Data += ControlGroup_ChangeSystemPowerData;
                        listControlGroup.Add(newControlGroup);
                    }
                }

                controlMusic.groupChange += ControlMusic_GroupChange_EventHandle;
                controlMusic.Thread_Start();
            }

            // Others
            {
                dataLog.CreatLog(Data.GLOBAL_CONSTANT.LOG_TYPE_SYSTEM_START, "Khởi động phần mềm thành công");
            }
        }
        // Private methods -------------------------------------------------------------------------------
            // For control thread
        private void ControlGroup_ChangeVOPLCData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            for (int j = 0; j < listConnectionHardware.Count; j++)
            {
                if (listConnectionHardware[j].connectionPLCModbus != null)
                {
                    if (newConfig.hardwareID == listConnectionHardware[j].connectionPLCModbus.Get_HardwareID())
                    {// Send data
                        listConnectionHardware[j].connectionPLCModbus.Change_ValveOnOff_PLC_Data(newConfig.listVO_PLC);
                    }
                }
            }
        }
        private void ControlGroup_ChangeVSPLCData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            for (int j = 0; j < listConnectionHardware.Count; j++)
            {
                if (listConnectionHardware[j].connectionPLCModbus != null)
                {
                    if (newConfig.hardwareID == listConnectionHardware[j].connectionPLCModbus.Get_HardwareID())
                    {// Send data
                        listConnectionHardware[j].connectionPLCModbus.Change_ValveStep_PLC_Data(newConfig.listVS_PLC);
                    }
                }
            }
        }
        private void ControlGroup_ChangePumpDigitalData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            for (int j = 0; j < listConnectionHardware.Count; j++)
            {
                if (listConnectionHardware[j].connectionPLCModbus != null)
                {
                    if (newConfig.hardwareID == listConnectionHardware[j].connectionPLCModbus.Get_HardwareID())
                    {// Send data
                        listConnectionHardware[j].connectionPLCModbus.Change_PumpDigital_PLC_Data(newConfig.listInverter);
                    }
                }
            }
        }
        private void ControlGroup_ChangePumpAnalogData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            for (int j = 0; j < listConnectionHardware.Count; j++)
            {
                if (listConnectionHardware[j].connectionPLCModbus != null)
                {
                    if (newConfig.hardwareID == listConnectionHardware[j].connectionPLCModbus.Get_HardwareID())
                    {// Send data
                        listConnectionHardware[j].connectionPLCModbus.Change_PumpAnalog_PLC_Data(newConfig.listInverter);
                    }
                }
            }
        }
        private void ControlGroup_ChangeLedArtnetData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            //List<List<Data.Data_Config_Device_LED_ArtnetDMX>> listLEDChangeBuf = new List<List<Data.Data_Config_Device_LED_ArtnetDMX>>();
            //listLEDChangeBuf.Clear();
            //// Creat list hardware and device buffer
            //for (int i = 0; i < newConfig.listLED_ArtnetDMX.Count; i++)
            //{
            //    bool newConnectionCheck = true;
            //    for (int j = 0; j < listLEDChangeBuf.Count; j++)
            //    {
            //        if (listLEDChangeBuf[j].Count > 0)
            //        {
            //            if (newConfig.listLED_ArtnetDMX[i].hardwareID == listLEDChangeBuf[j][0].hardwareID)
            //            {
            //                newConnectionCheck = false;
            //                listLEDChangeBuf[j].Add(newConfig.listLED_ArtnetDMX[i]);
            //            }
            //        }
            //    }
            //    if (newConnectionCheck) // Creat new list device buffer
            //    {
            //        List<Data.Data_Config_Device_LED_ArtnetDMX> ledBuf = new List<Data.Data_Config_Device_LED_ArtnetDMX>();
            //        ledBuf.Clear();
            //        ledBuf.Add(newConfig.listLED_ArtnetDMX[i]);
            //        listLEDChangeBuf.Add(ledBuf);
            //    }
            //}
            // Send data to hardware
            //for (int i = 0; i < listLEDChangeBuf.Count; i++)
            {
                for (int j = 0; j < listConnectionHardware.Count; j++)
                {
                    if (listConnectionHardware[j].connectionArtNet != null)
                    {
                        if (newConfig.hardwareID == listConnectionHardware[j].connectionArtNet.Get_HardwareID())
                        {// Send data
                            listConnectionHardware[j].connectionArtNet.SendArtnetDmx(newConfig.listLED_ArtnetDMX);
                        }
                    }
                }
            }
        }
        private void ControlGroup_ChangeLEDPLCData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            //List<List<Data.Data_Config_Device_LED_PLC>> listLEDChangeBuf = new List<List<Data.Data_Config_Device_LED_PLC>>();
            //listLEDChangeBuf.Clear();
            //Creat list hardware and device buffer
            //for (int i = 0; i < newConfig.listLED_PLC.Count; i++)
            //{
            //    bool newConnectionCheck = true;
            //    for (int j = 0; j < listLEDChangeBuf.Count; j++)
            //    {
            //        if (listLEDChangeBuf[j].Count > 0)
            //        {
            //            if (newConfig.listLED_PLC[i].hardwareID == listLEDChangeBuf[j][0].hardwareID)
            //            {
            //                newConnectionCheck = false;
            //                listLEDChangeBuf[j].Add(newConfig.listLED_PLC[i]);
            //            }
            //        }
            //    }
            //    if (newConnectionCheck) // Creat new list device buffer
            //    {
            //        List<Data.Data_Config_Device_LED_PLC> ledBuf = new List<Data.Data_Config_Device_LED_PLC>();
            //        ledBuf.Clear();
            //        ledBuf.Add(newConfig.listLED_PLC[i]);
            //        listLEDChangeBuf.Add(ledBuf);
            //    }
            //}
            // Send data to hardware
            //for (int i = 0; i < listLEDChangeBuf.Count; i++)
            {
                for (int j = 0; j < listConnectionHardware.Count; j++)
                {
                    if (listConnectionHardware[j].connectionPLCModbus != null)
                    {
                        if (newConfig.hardwareID == listConnectionHardware[j].connectionPLCModbus.Get_HardwareID())
                        {// Send data
                            listConnectionHardware[j].connectionPLCModbus.Change_LED485_PLC_Data(newConfig.listLED_PLC, newConfig.red, newConfig.green, newConfig.blue);
                        }
                    }
                }
            }
        }
        private void ControlGroup_ChangeSystemPowerData(object sender,  Data.Data_Config_Hardware_Group newConfig)
        {
            for (int j = 0; j < listConnectionHardware.Count; j++)
            {
                if (listConnectionHardware[j].connectionPLCModbus != null)
                {
                    if (newConfig.hardwareID == listConnectionHardware[j].connectionPLCModbus.Get_HardwareID())
                    {// Send data
                        listConnectionHardware[j].connectionPLCModbus.Change_SystemPower_Data(newConfig.systemPower);
                    }
                }
            }
        }
        private void ControlMusic_GroupChange_EventHandle(object sender, Data.Data_Config_Hardware_Group groupTest)
        {
            for (int i = 0; i < listControlGroup.Count; i++)
            {
                if ((groupTest.hardwareID == listControlGroup[i].Get_HardwareID()) && (groupTest.groupID == listControlGroup[i].Get_GroupID()))
                {
                    groupTest.testMode = false;
                    listControlGroup[i].Set_NewGroupConfig(groupTest);
                }
            }
        }

        // For connection thread
        private void Connection_Connected_EventHandle(object sender, int hardwareIDBuf)
        {
            Connection_Connected_EventHandle_Support(hardwareIDBuf);
        }
        private void Connection_Connected_EventHandle_Support(int hardwareIDBuf)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => Connection_Connected_EventHandle_Support(hardwareIDBuf));
                return;
            }
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (hardwareIDBuf == dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID)
                {
                    dataConfig.Get_DataConfigHardware().listHardware[i].serverConnectionState = "Connected";
                }
            }
            uiHardwareConfig.Update_HardwareConnectionState(hardwareIDBuf, "Connected");
        }
        private void Connection_Connecting_EventHandle(object sender, int hardwareIDBuf)
        {
            //Connection_Connecting_EventHandle_Support(hardwareIDBuf);
        }
        private void Connection_Connecting_EventHandle_Support(int hardwareIDBuf)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => Connection_Connecting_EventHandle_Support(hardwareIDBuf));
                return;
            }
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (hardwareIDBuf == dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID)
                {
                    dataConfig.Get_DataConfigHardware().listHardware[i].serverConnectionState = "Connecting";
                }
            }
            uiHardwareConfig.Update_HardwareConnectionState(hardwareIDBuf, "Connecting");
        }
        private void Connection_Disconnected_EventHandle(object sender, int hardwareIDBuf)
        {
            Connection_Disconnected_EventHandle_Support(hardwareIDBuf);
        }
        private void Connection_Disconnected_EventHandle_Support(int hardwareIDBuf)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => Connection_Disconnected_EventHandle_Support(hardwareIDBuf));
                return;
            }
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (hardwareIDBuf == dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID)
                {
                    dataConfig.Get_DataConfigHardware().listHardware[i].serverConnectionState = "Disconnected";
                }
            }
            uiHardwareConfig.Update_HardwareConnectionState(hardwareIDBuf, "Disconnected");
        }

            // For sub UI
        private void UIConfigHardware_ListHardwareSaveButtonClick_EventHandle(object sender, List<Data.Data_Config_Hardware> newListHardware)
        {
            {
                // Delete hardware is not in newList
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    bool needDeleteCheck = true;
                    for (int j = 0; j < newListHardware.Count; j++)
                    {
                        if ((dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == newListHardware[j].hardwareID) && (dataConfig.Get_DataConfigHardware().listHardware[i].type == newListHardware[j].type))
                        {
                            needDeleteCheck = false;
                            j = newListHardware.Count;
                        }
                    }
                    if (needDeleteCheck)
                    {
                        dataConfig.DeleteHardwareByID(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID);
                        i--;
                    }
                }
                // Edit hardware is change data in newList
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    for (int j = 0; j < newListHardware.Count; j++)
                    {
                        if ((dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == newListHardware[j].hardwareID) && (dataConfig.Get_DataConfigHardware().listHardware[i].type == newListHardware[j].type))
                        {
                            dataConfig.Get_DataConfigHardware().listHardware[i].name = newListHardware[j].name;
                            dataConfig.Get_DataConfigHardware().listHardware[i].opcServerURL = newListHardware[j].opcServerURL;
                            dataConfig.Get_DataConfigHardware().listHardware[i].artnetServerURL = newListHardware[j].artnetServerURL;
                            dataConfig.Get_DataConfigHardware().listHardware[i].modbusServerURL = newListHardware[j].modbusServerURL;
                            dataConfig.EditHardware(dataConfig.Get_DataConfigHardware().listHardware[i]);
                        }
                    }
                }
                // Add new hardware is not in Get_DataConfigHardware
                for (int j = 0; j < newListHardware.Count; j++)
                {
                    bool needAddCheck = true;
                    for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                    {
                        if (dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == newListHardware[j].hardwareID)
                        {
                            needAddCheck = false;
                            i = dataConfig.Get_DataConfigHardware().listHardware.Count;
                        }
                    }
                    if (needAddCheck)
                    {
                         Data.Data_Config_Hardware newHardwareBuf = new  Data.Data_Config_Hardware();
                        newHardwareBuf.hardwareID = newListHardware[j].hardwareID;
                        newHardwareBuf.type = newListHardware[j].type;
                        newHardwareBuf.name = newListHardware[j].name;
                        newHardwareBuf.opcServerURL = newListHardware[j].opcServerURL;
                        newHardwareBuf.artnetServerURL = newListHardware[j].artnetServerURL;
                        newHardwareBuf.modbusServerURL = newListHardware[j].modbusServerURL;
                        {// Creat default option
                            newHardwareBuf.optionVO_PLC = new Data.Data_Config_Hardware_Option_VO_PLC();
                            newHardwareBuf.optionVS_PLC = new Data.Data_Config_Hardware_Option_VS_PLC();
                            newHardwareBuf.optionInverter = new Data.Data_Config_Hardware_Option_Inverter();
                            newHardwareBuf.optionLED_ArtnetDMX = new Data.Data_Config_Hardware_Option_LED_ArtnetDMX();
                            newHardwareBuf.optionLED_PLC = new Data.Data_Config_Hardware_Option_LED_PLC();
                            // Default option for valve stepper
                            newHardwareBuf.optionVS_PLC.coordAngle = Data.GLOBAL_CONSTANT.HARDWARE_OPTION_VALVESTEPPER_COORDANGLE_DEFAULT;
                            newHardwareBuf.optionVS_PLC.maxAngle = Data.GLOBAL_CONSTANT.HARDWARE_OPTION_VALVESTEPPER_MAXANGLE_DEFAULT;
                            newHardwareBuf.optionVS_PLC.maxSpeed = Data.GLOBAL_CONSTANT.HARDWARE_OPTION_VALVESTEPPER_MAXSPEED_DEFAULT;
                            newHardwareBuf.optionVS_PLC.ratio = Data.GLOBAL_CONSTANT.HARDWARE_OPTION_VALVESTEPPER_RATIO_DEFAULT;
                            // Default option for inverter
                            newHardwareBuf.optionInverter.freqMax = Data.GLOBAL_CONSTANT.HARDWARE_OPTION_PUMPANALOG_MAXFREQ_DEFAULT;
                            newHardwareBuf.optionInverter.freqMin = Data.GLOBAL_CONSTANT.HARDWARE_OPTION_PUMPANALOG_MINFREQ_DEFAULT;
                        }
                        {// Creat default list devices
                            newHardwareBuf.listVO_PLC = new List<Data.Data_Config_Device_VO_PLC>();
                            newHardwareBuf.listVS_PLC = new List<Data.Data_Config_Device_VS_PLC>();
                            newHardwareBuf.listInverter = new List<Data.Data_Config_Device_Inverter>();
                            newHardwareBuf.listLED_ArtnetDMX = new List<Data.Data_Config_Device_LED_ArtnetDMX>();
                            newHardwareBuf.listLED_PLC = new List<Data.Data_Config_Device_LED_PLC>();
                            newHardwareBuf.listGroup = new List<Data.Data_Config_Hardware_Group>();
                        }
                        dataConfig.AddHardware(newHardwareBuf);
                    }
                }
                MessageBox.Show("Lưu thông tin cấu hình PLC thành công .");
                uiHardwareConfig.Update_HardwareConfig(dataConfig.Get_DataConfigHardware());
            }
            {// Recreat all connection
                for (int i = 0; i < listConnectionHardware.Count; i++)
                {// Disconnect all old connections
                    if (listConnectionHardware[i].connectionPLC != null)
                    {
                        listConnectionHardware[i].connectionPLC.Disconnect2OPCUAServer();
                    }
                    if (listConnectionHardware[i].connectionPLCModbus != null)
                    {
                        listConnectionHardware[i].connectionPLCModbus.Disconnect();
                        listConnectionHardware[i].connectionPLCModbus.Thread_Stop();
                    }
                    if (listConnectionHardware[i].connectionArtNet != null)
                    {
                        listConnectionHardware[i].connectionArtNet.Disconnect2Artnet();
                    }
                }
                listConnectionHardware.Clear();
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    Connection_Hardware connectionHardware = new Connection_Hardware();
                    switch (dataConfig.Get_DataConfigHardware().listHardware[i].type)
                    {
                        case "Valve On/Off - PLC":
                        case "Pump A/D & Power - PLC":
                        case "LED 485 - PLC":
                        case "Valve Stepper - PLC":
                        case "Valve On/Off & Stepper - PLC":
                        case "Valve Stepper & LED 485 - PLC":
                            {
                                connectionHardware.connectionPLCModbus = new Connection.Connection_PLC_Modbus(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID, dataConfig.Get_DataConfigHardware().listHardware[i].modbusServerURL, 502, 1);
                                connectionHardware.connectionPLCModbus.plcModbusConnected += Connection_Connected_EventHandle;
                                connectionHardware.connectionPLCModbus.plcModbusConnecting += Connection_Connecting_EventHandle;
                                connectionHardware.connectionPLCModbus.plcModbusDisconnected += Connection_Disconnected_EventHandle;
                                //connectionHardware.connectionPLCModbus.Connect();
                                if (dataConfig.Get_DataConfigHardware().listHardware[i].optionInverter != null)
                                {
                                    connectionHardware.connectionPLCModbus.Change_PumpAnalog_PLC_Config(dataConfig.Get_DataConfigHardware().listHardware[i].optionInverter);
                                }
                                if (dataConfig.Get_DataConfigHardware().listHardware[i].optionVS_PLC != null)
                                {
                                    connectionHardware.connectionPLCModbus.Change_ValveStep_PLC_Config(dataConfig.Get_DataConfigHardware().listHardware[i].optionVS_PLC);
                                }
                                connectionHardware.connectionPLCModbus.Thread_Start();
                            }
                            break;
                        case "LED DMX - Artnet":
                            {
                                connectionHardware.connectionArtNet = new Connection.Connection_Artnet(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID, IPAddress.Parse(dataConfig.Get_DataConfigHardware().listHardware[i].artnetServerURL), IPAddress.Parse("255.255.255.0"));
                                connectionHardware.connectionArtNet.artnetConnected += Connection_Connected_EventHandle;
                                //connectionHardware.connectionArtNet.Connect2Artnet();
                            }
                            break;
                        default:
                            {
                                MessageBox.Show("Creat connection: Unknown hardwareType: " + dataConfig.Get_DataConfigHardware().listHardware[i].type + ", hardwareID: " + dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID.ToString());
                            }
                            break;
                    }
                    listConnectionHardware.Add(connectionHardware);
                }

            }
        }
        private void UIConfigHardware_HardwareOptionSaveClick_EventHandle(object sender, Data.Data_Config_Hardware newHardwareOption)
        {
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == newHardwareOption.hardwareID)
                {// Update dataConfig
                    dataConfig.Get_DataConfigHardware().listHardware[i].optionInverter = newHardwareOption.optionInverter;
                    dataConfig.Get_DataConfigHardware().listHardware[i].optionLED_ArtnetDMX = newHardwareOption.optionLED_ArtnetDMX;
                    dataConfig.Get_DataConfigHardware().listHardware[i].optionLED_PLC = newHardwareOption.optionLED_PLC;
                    dataConfig.Get_DataConfigHardware().listHardware[i].optionVO_PLC = newHardwareOption.optionVO_PLC;
                    dataConfig.Get_DataConfigHardware().listHardware[i].optionVS_PLC = newHardwareOption.optionVS_PLC;
                    dataConfig.EditHardware(dataConfig.Get_DataConfigHardware().listHardware[i]);
                    for (int j = 0; j < listConnectionHardware.Count; j++)
                    {// Send new config to hardware
                        if (listConnectionHardware[j].connectionPLCModbus != null)
                        {
                            if (listConnectionHardware[j].connectionPLCModbus.Get_HardwareID() == newHardwareOption.hardwareID)
                            {
                                switch (newHardwareOption.type)
                                {
                                    case "Valve Stepper - PLC":
                                        {
                                            listConnectionHardware[j].connectionPLCModbus.Change_ValveStep_PLC_Config(newHardwareOption.optionVS_PLC);
                                        }
                                        break;
                                    case "Pump A/D & Power - PLC":
                                        {
                                            listConnectionHardware[j].connectionPLCModbus.Change_PumpAnalog_PLC_Config(newHardwareOption.optionInverter);
                                        }
                                        break;
                                    default:
                                        {

                                        }
                                        break;
                                }
                            }
                        }
                    }
                    MessageBox.Show("Cập nhật cấu hình tuỳ chọn cho " + dataConfig.Get_DataConfigHardware().listHardware[i].name + " thành công");
                }
            }
        }
        private void UIConfigHardware_ReconnectHardwareButtonClick_EventHandle(object sender, int hardwareIDBuf)
        {
            for (int i = 0; i < listConnectionHardware.Count; i++)
            {// Disconnect all old connections
                if (listConnectionHardware[i].connectionPLC != null)
                {
                    if (listConnectionHardware[i].connectionPLC.Get_PLCID() == hardwareIDBuf)
                    {
                        listConnectionHardware[i].connectionPLC.Connect2OPCUAServer();
                    }
                }
                if (listConnectionHardware[i].connectionPLCModbus != null)
                {
                    if (listConnectionHardware[i].connectionPLCModbus.Get_HardwareID() == hardwareIDBuf)
                    {
                        listConnectionHardware[i].connectionPLCModbus.Connect();
                    }
                }
                if (listConnectionHardware[i].connectionArtNet != null)
                {
                    if (listConnectionHardware[i].connectionArtNet.Get_HardwareID() == hardwareIDBuf)
                    {
                        listConnectionHardware[i].connectionArtNet.Connect2Artnet();
                    }
                }
            }
        }
        private void UIConfigHardware_GroupTestClick_EventHandle(object sender,  Data.Data_Config_Hardware_Group groupTest)
        {
            for (int i = 0; i < listControlGroup.Count; i++)
            {
                if ((groupTest.hardwareID == listControlGroup[i].Get_HardwareID()) && (groupTest.groupID == listControlGroup[i].Get_GroupID()))
                {
                    groupTest.testMode = true;
                    listControlGroup[i].Set_NewGroupConfig(groupTest);
                }
            }
        }
        private void UIConfigHardware_GroupSaveClick_EventHandle(object sender,  Data.Data_Config_Hardware newHardwareGroupBuf)
        {
            {// Update groupList
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    if (dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == newHardwareGroupBuf.hardwareID)
                    {
                        // Delete all old group
                        dataConfig.DeleteAllGroupInHardware(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID);
                        // Recreat all new group
                        for (int j = 0; j < newHardwareGroupBuf.listGroup.Count; j++)
                        {
                            newHardwareGroupBuf.listGroup[j].hardwareID = newHardwareGroupBuf.hardwareID;
                            dataConfig.AddHardwareGroup(newHardwareGroupBuf.listGroup[j]);
                        }
                        dataConfig.EditHardware(dataConfig.Get_DataConfigHardware().listHardware[i]);
                        MessageBox.Show("Cập nhật cấu hình nhóm cho " + dataConfig.Get_DataConfigHardware().listHardware[i].name + " thành công");
                    }
                }
                //uiHardwareConfig.Update_HardwareConfig(dataConfig.Get_DataConfigHardware());
            }
            {// Recreat all control group thread
                controlMusic.Set_NewMusicConfig(null); // Reset config of music run now
                for (int i = 0; i < listControlGroup.Count; i++)
                {// Stop all old thread
                    listControlGroup[i].Thread_Stop();
                }
                listControlGroup.Clear();
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {// Recreat new control group
                    for (int j = 0; j < dataConfig.Get_DataConfigHardware().listHardware[i].listGroup.Count; j++)
                    {
                        Control_Group newControlGroup = new Control_Group(dataConfig.Get_DataConfigHardware().listHardware[i], dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j]);
                        switch (dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].type)
                        {
                            case "Valve Stepper - PLC":
                            case "Valve On/Off - PLC":
                            case "Pump Digital - PLC":
                            case "Pump Analog - PLC":
                            case "Pump Analog Dual - PLC":
                            case "LED 485 - PLC":
                            case "LED DMX - Artnet":
                            case "System Power":
                                {
                                    newControlGroup.Thread_Start();
                                }
                                break;
                            default:
                                {
                                    MessageBox.Show("Creat control: Unknown groupType: " + dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].type + ", hardwareID: " + dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].hardwareID.ToString() + ", groupID: " + dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j].groupID.ToString());
                                }
                                break;
                        }
                        newControlGroup.changeVOPLC_Data += ControlGroup_ChangeVOPLCData;
                        newControlGroup.changeVSPLC_Data += ControlGroup_ChangeVSPLCData;
                        newControlGroup.changePumpDigital_Data += ControlGroup_ChangePumpDigitalData;
                        newControlGroup.changePumpAnalog_Data += ControlGroup_ChangePumpAnalogData;
                        newControlGroup.changeLEDArtnet_Data += ControlGroup_ChangeLedArtnetData;
                        newControlGroup.changeLEDPLC_Data += ControlGroup_ChangeLEDPLCData;
                        newControlGroup.changeSystemPower_Data += ControlGroup_ChangeSystemPowerData;
                        listControlGroup.Add(newControlGroup);
                    }
                }
            }
            {// Update group name list for editor
                List<Data.Data_Config_Hardware_Group> listGroupBufForEditor = new List<Data.Data_Config_Hardware_Group>();
                listGroupBufForEditor.Clear();
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    for (int j = 0; j < dataConfig.Get_DataConfigHardware().listHardware[i].listGroup.Count; j++)
                    {
                        listGroupBufForEditor.Add(dataConfig.Get_DataConfigHardware().listHardware[i].listGroup[j]);
                    }
                }
                uiEffectEditor.UpdateListGroupConfig(listGroupBufForEditor);
                uiMusicEditor.UpdateListGroupConfig(listGroupBufForEditor);
                uiPlaylist.UpdateListGroupConfig(listGroupBufForEditor);
            }
        }
        private void UIEffectEditor_SaveEffectListClick_EventHandle(object sender, List<Data.Data_Config_Effect> newListEffect)
        {
            try
            {
                dataConfig.DeleteAllEffect();
                for (int i = 0; i < newListEffect.Count; i++)
                {
                    dataConfig.AddEffect(newListEffect[i]);
                }
                uiEffectEditor.UpdateListEffectConfig(dataConfig.Get_DataConfigPlaylist().listEffect);
                uiMusicEditor.UpdateListEffectConfig(dataConfig.Get_DataConfigPlaylist().listEffect);
                MessageBox.Show("Lưu thông tin hiệu ứng thành công");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void UIMusicEditor_PlayMusicClick_EventHandle(object sender, Data.Data_Config_Music newMusicRunning)
        {
            try
            {
                // Stop other music
                if (sender == uiMusicEditor)
                {
                    uiPlaylist.StopPlay();
                }
                if (sender == uiPlaylist)
                {
                    uiMusicEditor.StopPlay();
                }
                // Set new config for controlMusic thread
                if (controlMusic != null)
                {
                    controlMusic.Set_NewMusicConfig(newMusicRunning);
                }
                // Creat log
                dataLog.CreatLog(Data.GLOBAL_CONSTANT.LOG_TYPE_MUSIC_START, "Bắt đầu phát bản nhạc: " + newMusicRunning.musicPath);
            }
            catch
            {

            }
        }
        private void UIMusicEditor_StopMusicClick_EventHandle(object sender, int noUse)
        {
            try
            {
                if (controlMusic != null)
                {
                    controlMusic.Set_NewMusicConfig(null);
                }
                for (int i = 0; i < listControlGroup.Count; i++)
                {
                    listControlGroup[i].SetEffectToSpecialStop();
                }
                // Creat log
                dataLog.CreatLog(Data.GLOBAL_CONSTANT.LOG_TYPE_MUSIC_STOP, "Đã dừng phát nhạc");
            }
            catch
            {

            }
        }
        private void UIMusicEditor_PlayMusicTimeUpdate_EventHandle(object sender, double newMusicTime)
        {
            UIMusicEditor_PlayMusicTimeUpdate_EventHandle_Support(newMusicTime);
        }
        private void UIMusicEditor_PlayMusicTimeUpdate_EventHandle_Support(double newMusicTime)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => UIMusicEditor_PlayMusicTimeUpdate_EventHandle_Support(newMusicTime));
                return;
            }
            try
            {
                if (controlMusic != null)
                {
                    controlMusic.Set_NewMusicTime(newMusicTime);
                }
                for (int i = 0; i < listControlGroup.Count; i++)
                {
                    listControlGroup[i].Set_NewMusicTime(newMusicTime);
                }
            }
            catch
            {
                MessageBox.Show("Check it now");
            }
        }
        private void UIMusicEditor_SaveMusicEffectClick_EventHandle(object sender,  Data.Data_Config_Music newMusicRunning)
        {
            dataConfig.EditDataConfigMusic(newMusicRunning);
            uiMusicEditor.UpdateListMusicConfig(dataConfig.Get_DataConfigPlaylist().listMusic);
            uiPlaylist.UpdateListMusicConfig(dataConfig.Get_DataConfigPlaylist().listMusic);
            MessageBox.Show("Lưu thông tin hiệu ứng thành công .");
        }
        private void UIPlayList_PlaylistModeSave_EventHandle(object sender, Data.Data_Config_Playlist_Mode newPlaylistMode)
        {
            try
            {
                dataConfig.EditPlaylistMode(newPlaylistMode);
                MessageBox.Show("Lưu chế độ phát nhạc thành công");
            }
            catch
            {

            }
        }
        private void UILog_ClearLogClick_EventHandle(object sender, int noUse)
        {
            try
            {
                dataLog.DeleteAllLog();
                uiLog.RefreshListLogFiles();
                uiLog.RefreshListLogs();
            }
            catch
            {

            }
        }
        private void UILog_ChosenLogFileChanged_EventHandle(object sender, string newLogFilePath)
        {
            try
            {
                List<Data.Data_Log_Info> newListLog = dataLog.GetLogByPath(newLogFilePath);
                if (newListLog != null)
                {
                    uiLog.UpdateListLogs(newListLog);
                }
                else
                {
                    MessageBox.Show("Không đọc được dữ liệu từ file: " + newLogFilePath);
                }
            }
            catch
            {

            }
        }



        // For Main UI        
        private void HardwareConfig_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            uiHardwareConfig.Update_HardwareConfig(dataConfig.Get_DataConfigHardware());
            mainLayout.Content = uiHardwareConfig;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void EffectEditor_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            mainLayout.Content = uiEffectEditor;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void MusicEditor_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            //uiPlaylist.StopPlay();
            mainLayout.Content = uiMusicEditor;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void Playlist_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            //uiMusicEditor.StopPlay();
            mainLayout.Content = uiPlaylist;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void History_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            mainLayout.Content = uiLog;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Sub UI exit
            {
                uiHardwareConfig.ApplicationExit();
                uiMusicEditor.ApplicationExit();
                uiPlaylist.ApplicationExit();
                uiLog.ApplicationExit();
            }
            // Disconnect all connection and stop sub thread of them
            {
                for (int i = 0; i < listConnectionHardware.Count; i++)
                {
                    if (listConnectionHardware[i].connectionPLC != null)
                    {
                        listConnectionHardware[i].connectionPLC.Disconnect2OPCUAServer();
                    }
                    if (listConnectionHardware[i].connectionPLCModbus != null)
                    {
                        listConnectionHardware[i].connectionPLCModbus.Disconnect();
                        listConnectionHardware[i].connectionPLCModbus.Thread_Stop();
                    }
                    if (listConnectionHardware[i].connectionArtNet != null)
                    {
                        listConnectionHardware[i].connectionArtNet.Disconnect2Artnet();
                    }
                }
                // Close all control threads
                for (int i = 0; i < listControlGroup.Count; i++)
                {
                    listControlGroup[i].Thread_Stop();
                }
                controlMusic.Thread_Stop();
            }
            // Others
            {
                dataLog.CreatLog(Data.GLOBAL_CONSTANT.LOG_TYPE_SYSTEM_STOP, "Đã đóng phần mềm");
            }
        }
        
        // Public methods -------------------------------------------------------------------------------

    }
}
