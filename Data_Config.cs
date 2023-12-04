using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MusicFountain.Data
{
    // For hardware
    public class Data_Config_Hardware_Option_VO_PLC
    {

    }
    public class Data_Config_Device_VO_PLC
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public int groupID { get; set; }
        public bool enable { get; set; }
        public bool ready { get; set; }
        public bool reset { get; set; }
        public bool error { get; set; }
        public bool setOnOff { get; set; }
        public bool currentOnOff { get; set; }
    }
    public class Data_Config_Hardware_Option_VS_PLC
    {
        public float maxAngle { get; set; }
        public float maxSpeed { get; set; }
        public float coordAngle { get; set; }
        public float ratio { get; set; }

    }
    public class Data_Config_Device_VS_PLC
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public int groupID { get; set; }
        public bool enable { get; set; }
        public bool ready { get; set; }
        public bool reset { get; set; }
        public bool error { get; set; }
        public float setCoordAngle { get; set; }
        public float setMaxAngle { get; set; }
        public float setMaxSpeed { get; set; }
        public float setRatio { get; set; }
        public float setSpeed { get; set; }
        public float currentSpeed { get; set; }
        public float setAngle { get; set; }
        public float currentAngle { get; set; }
    }
    public class Data_Config_Hardware_Option_Inverter
    {
        public float freqMax { get; set; }
        public float freqMin { get; set; }
    }
    public class Data_Config_Device_Inverter
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public int groupID { get; set; }
        public bool enable { get; set; }
        public bool ready { get; set; }
        public bool reset { get; set; }
        public bool error { get; set; }
        public bool setStatus { get; set; }
        public int setMode { get; set; }
        public int setFreqMin { get; set; }
        public int setFreqMax { get; set; }
        public float manFreq { get; set; }
        public float sinAmplMin { get; set; }
        public float sinAmplMax { get; set; }
        public float sinFreqCycle { get; set; }
        public float sinPhaseDiff { get; set; }
    }
    public class Data_Config_Hardware_Option_LED_ArtnetDMX
    {
    }
    public class Data_Config_Device_LED_ArtnetDMX
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public int groupID { get; set; }
        public int state { get; set; }
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }
    public class Data_Config_Hardware_Option_LED_PLC
    {
    }
    public class Data_Config_Device_LED_PLC
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public int groupID { get; set; }
        public int state { get; set; }
    }
    public class Data_Config_Hardware_Group_Data
    {
    }
    public class Data_Config_Hardware_Group
    {
        public int hardwareID { get; set; }
        public int groupID { get; set; }
        public string name { get; set; }
        public int startDevID { get; set; }
        public int endDevID { get; set; }
        public string type { get; set; }
        public string effectSave { get; set; }
        public string effectRunning { get; set; }
        public bool testMode { get; set; }  // Manual test 
        public bool systemPower { get; set; }
        public float speed { get; set; }    // tốc độ van step
        public float timeGoOn { get; set; }   // thời gian đạt công suất max
        public float timeHoldOn { get; set; }   // thời gian giữ mở, chu kỳ lặp (set chung cho cycle), giữ góc vẫy - công suất max - công suất đặt
        public float timeGoOff { get; set; }  // thời gian đạt công suất min
        public float timeHoldOff { get; set; }  // thời gian giữ tắt, giữ góc vẫy - công suất min
        public float startPosition { get; set; }    // góc bắt đầu vẫy hoặc giữ góc vẫy cố định
        public float endPosition { get; set; }      // góc kết thúc vẫy
        public float freqMax { get; set; }      // tần số - công suất biến tần max, công suất đặt
        public float freqMin { get; set; }      // tần số - công suất biến tần min
        public float cycle { get; set; }        // chu kỳ thay đổi của vòi cụp xoè
        public int red { get; set; }        // LED
        public int green { get; set; }      // LED
        public int blue { get; set; }       // LED
        public List<Data_Config_Device_VS_PLC> listVS_PLC { get; set; }
        public List<Data_Config_Device_VO_PLC> listVO_PLC { get; set; }
        public List<Data_Config_Device_Inverter> listInverter { get; set; }
        public List<Data_Config_Device_LED_ArtnetDMX> listLED_ArtnetDMX { get; set; }
        public List<Data_Config_Device_LED_PLC> listLED_PLC { get; set; }

    }
    public class Data_Config_Hardware
    {
        public int hardwareID { get; set; }
        public string name { get; set; }
        public string opcServerURL { get; set; }
        public int opcServerPort { get; set; }
        public string modbusServerURL { get; set; }
        public int modbusServerPort { get; set; }
        public string artnetServerURL { get; set; }
        public string type { get; set; }
        public string serverConnectionState { get; set; }
        public Data_Config_Hardware_Option_VO_PLC optionVO_PLC { get; set; }
        public List<Data_Config_Device_VO_PLC> listVO_PLC { get; set; }
        public Data_Config_Hardware_Option_VS_PLC optionVS_PLC { get; set; }
        public List<Data_Config_Device_VS_PLC> listVS_PLC { get; set; }
        public Data_Config_Hardware_Option_Inverter optionInverter { get; set; }
        public List<Data_Config_Device_Inverter> listInverter { get; set; }
        public Data_Config_Hardware_Option_LED_ArtnetDMX optionLED_ArtnetDMX { get; set; }
        public List<Data_Config_Device_LED_ArtnetDMX> listLED_ArtnetDMX { get; set; }
        public Data_Config_Hardware_Option_LED_PLC optionLED_PLC { get; set; }
        public List<Data_Config_Device_LED_PLC> listLED_PLC { get; set; }
        public List<Data_Config_Hardware_Group> listGroup { get; set; }

    }
    public class Data_Config_List_Hardware
    {
        public List<Data_Config_Hardware> listHardware { get; set; }
    }

    // For music effect and playlist
    public class Data_Config_Effect
    {
        public int effectID { get; set; }
        public string effectName { get; set; }
        public int hardwareID { get; set; }
        public string hardwarename { get; set; }
        public int groupID { get; set; }
        public string groupName { get; set; }
        public string groupType { get; set; }
        public string groupEffect { get; set; }
        public string para1Name { get; set; }
        public string para1 { get; set; }
        public string para2Name { get; set; }
        public string para2 { get; set; }
        public string para3Name { get; set; }
        public string para3 { get; set; }
        public string para4Name { get; set; }
        public string para4 { get; set; }
        public string para5Name { get; set; }
        public string para5 { get; set; }
        public string para6Name { get; set; }
        public string para6 { get; set; }
        public List<Data_Config_Device_VS_PLC> listVS_PLC { get; set; }
        public List<Data_Config_Device_VO_PLC> listVO_PLC { get; set; }
        public List<Data_Config_Device_Inverter> listInverter { get; set; }
        public List<Data_Config_Device_LED_ArtnetDMX> listLED_ArtnetDMX { get; set; }
        public List<Data_Config_Device_LED_PLC> listLED_PLC { get; set; }
    }
    public class Data_Config_Effect_With_Time
    {
        public int musicTimeID { get; set; }
        public double musicTimeLength { get; set; }
        public double musicTimeStart { get; set; }
        public double musicTimeStop { get; set; }
        public List<Data_Config_Effect> listEffectData { get; set; }
    }
    public class Data_Config_Music
    {
        public string musicPath { get; set; }
        public List<Data_Config_Effect_With_Time> listEffect { get; set; }
    }
    public class Data_Config_Playlist_Mode
    {
        public bool isRepeatOne { get; set; }
        public bool isAutoStart { get; set; }
        public int hourAutoStart { get; set; }
        public int minAutoStart { get; set; }
        public bool isAutoStop { get; set; }
        public int hourAutoStop { get; set; }
        public int minAutoStop { get; set; }
    }
    public class Data_Config_Playlist
    {
        public Data_Config_Playlist_Mode playlistMode { get; set; }
        public List<Data_Config_Effect> listEffect { get; set; }
        public List<Data_Config_Music> listMusic { get; set; }
    }

    // Interface class 
    public partial class Data_Config
    {
        // Variables
        private string hardwareConfigFilePath;
        private string playlistConfigFilePath;

        private Data_Config_List_Hardware dataConfigHardware;
        private Data_Config_Playlist dataConfigPlaylist;

        public Data_Config()
        {
            {// Creat variables
                hardwareConfigFilePath = "../Data/HardwareConfig.json";
                playlistConfigFilePath = "../Data/PlaylistConfig.json";

                dataConfigHardware = new Data_Config_List_Hardware();
                dataConfigPlaylist = new Data_Config_Playlist();
            }

            {// Read hardware config
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    string jsonStrBuf = File.ReadAllText(hardwareConfigFilePath);
                    try
                    {
                        dataConfigHardware = JsonSerializer.Deserialize<Data_Config_List_Hardware>(jsonStrBuf);
                        for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
                        {
                            for (int j = 0; j < dataConfigHardware.listHardware[i].listGroup.Count; j++)
                            {
                                dataConfigHardware.listHardware[i].listGroup[j].effectRunning = "";
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Data_Config: Can not deserialize data: \n" + jsonStrBuf + "\n to JSON");
                    }
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
                }
            }
            {// Read playlist config
                if (File.Exists(playlistConfigFilePath))
                {// Read data and deserialize
                    string jsonStrBuf = File.ReadAllText(playlistConfigFilePath);
                    try
                    {
                        dataConfigPlaylist = JsonSerializer.Deserialize<Data_Config_Playlist>(jsonStrBuf);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Data_Config: Can not deserialize data: \n" + jsonStrBuf + "\n to JSON");
                    }
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + playlistConfigFilePath);
                }
            }

        }

        // Get data
        public Data_Config_List_Hardware Get_DataConfigHardware()
        {
            return dataConfigHardware;
        }
        public Data_Config_Playlist Get_DataConfigPlaylist()
        {
            return dataConfigPlaylist;
        }

        // Edit hardware
        public int AddHardware(Data_Config_Hardware newHardware)
        {
            if (newHardware.hardwareID <= 0)
            {// Hardware no have ID now
                newHardware.hardwareID = dataConfigHardware.listHardware.Count + 1;
                for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
                {// Creat new ID
                    if (newHardware.hardwareID <= dataConfigHardware.listHardware[i].hardwareID)
                    {
                        newHardware.hardwareID = dataConfigHardware.listHardware[i].hardwareID + 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Bạn đã tự điền hardwareID\nVui lòng chắc chắn rằng hardwareID bạn điền là chính xác, nếu không sẽ gây ra lỗi hệ thống");
            }
            dataConfigHardware.listHardware.Add(newHardware);
            {// Write hardware config to file
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(hardwareConfigFilePath, JsonSerializer.Serialize(dataConfigHardware));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
                }
            }
            return newHardware.hardwareID;
        }
        public Data_Config_Hardware DeleteHardwareByID(int hardwareID)
        {
            Data_Config_Hardware deletedPLC = null;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Delete PLC
                if (dataConfigHardware.listHardware[i].hardwareID == hardwareID)
                {
                    deletedPLC = dataConfigHardware.listHardware[i];
                    dataConfigHardware.listHardware.RemoveAt(i);
                    i = dataConfigHardware.listHardware.Count;
                }
            }
            {// Write hardware config to file
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(hardwareConfigFilePath, JsonSerializer.Serialize(dataConfigHardware));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
                }
            }
            return deletedPLC;
        }
        public Data_Config_Hardware EditHardware(Data_Config_Hardware hardwareBuf)
        {
            Data_Config_Hardware editHardware = null;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Edit PLC
                if (dataConfigHardware.listHardware[i].hardwareID == hardwareBuf.hardwareID)
                {
                    dataConfigHardware.listHardware[i] = hardwareBuf;
                    editHardware = dataConfigHardware.listHardware[i];
                    i = dataConfigHardware.listHardware.Count;
                }
            }
            {// Write hardware config to file
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(hardwareConfigFilePath, JsonSerializer.Serialize(dataConfigHardware));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
                }
            }
            return editHardware;
        }

        // Edit group
        public int AddHardwareGroup(Data_Config_Hardware_Group newHardwareGroup)
        {
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {
                if (dataConfigHardware.listHardware[i].hardwareID == newHardwareGroup.hardwareID)
                {
                    // Cal groupID
                    newHardwareGroup.groupID = dataConfigHardware.listHardware[i].listGroup.Count + 1;
                    for (int j = 0; j < dataConfigHardware.listHardware[i].listGroup.Count; j++)
                    {
                        if (newHardwareGroup.groupID == dataConfigHardware.listHardware[i].listGroup[j].groupID)
                        {
                            newHardwareGroup.groupID++;
                        }
                    }
                    // Creat list devices of group
                    switch (newHardwareGroup.type)
                    {
                        case "Valve On/Off - PLC":
                            {
                                newHardwareGroup.listVO_PLC = new List<Data_Config_Device_VO_PLC>();
                                newHardwareGroup.listVO_PLC.Clear();
                                for (int j = newHardwareGroup.startDevID; j <= newHardwareGroup.endDevID; j++)
                                {
                                    Data_Config_Device_VO_PLC newDev = new Data_Config_Device_VO_PLC();
                                    newDev.hardwareID = newHardwareGroup.hardwareID;
                                    newDev.groupID = newHardwareGroup.groupID;
                                    newDev.devID = j;
                                    newDev.type = newHardwareGroup.type;
                                    newDev.name = "";
                                    newDev.enable = true;
                                    newDev.reset = false;
                                    newHardwareGroup.listVO_PLC.Add(newDev);
                                }
                            }
                            break;
                        case "Valve Stepper - PLC":
                            {
                                newHardwareGroup.listVS_PLC = new List<Data_Config_Device_VS_PLC>();
                                newHardwareGroup.listVS_PLC.Clear();
                                for (int j = newHardwareGroup.startDevID; j <= newHardwareGroup.endDevID; j++)
                                {
                                    Data_Config_Device_VS_PLC newDev = new Data_Config_Device_VS_PLC();
                                    newDev.hardwareID = newHardwareGroup.hardwareID;
                                    newDev.groupID = newHardwareGroup.groupID;
                                    newDev.devID = j;
                                    newDev.type = newHardwareGroup.type;
                                    newDev.name = "";
                                    newDev.enable = true;
                                    newDev.reset = false;
                                    newHardwareGroup.listVS_PLC.Add(newDev);
                                }
                            }
                            break;
                        case "Pump Digital - PLC":
                        case "Pump Analog - PLC":
                        case "Pump Analog Dual - PLC":
                            {
                                newHardwareGroup.listInverter = new List<Data_Config_Device_Inverter>();
                                newHardwareGroup.listInverter.Clear();
                                for (int j = newHardwareGroup.startDevID; j <= newHardwareGroup.endDevID; j++)
                                {
                                    Data_Config_Device_Inverter newDev = new Data_Config_Device_Inverter();
                                    newDev.hardwareID = newHardwareGroup.hardwareID;
                                    newDev.groupID = newHardwareGroup.groupID;
                                    newDev.devID = j;
                                    newDev.type = newHardwareGroup.type;
                                    newDev.name = "";
                                    newDev.enable = true;
                                    newDev.reset = false;
                                    newHardwareGroup.listInverter.Add(newDev);
                                }
                            }
                            break;
                        case "LED DMX - Artnet":
                            {
                                newHardwareGroup.listLED_ArtnetDMX = new List<Data_Config_Device_LED_ArtnetDMX>();
                                newHardwareGroup.listLED_ArtnetDMX.Clear();
                                for (int j = newHardwareGroup.startDevID; j <= newHardwareGroup.endDevID; j++)
                                {
                                    Data_Config_Device_LED_ArtnetDMX newDev = new Data_Config_Device_LED_ArtnetDMX();
                                    newDev.hardwareID = newHardwareGroup.hardwareID;
                                    newDev.groupID = newHardwareGroup.groupID;
                                    newDev.devID = j;
                                    newDev.type = newHardwareGroup.type;
                                    newDev.name = "";
                                    newHardwareGroup.listLED_ArtnetDMX.Add(newDev);
                                }
                            }
                            break;
                        case "LED 485 - PLC":
                            {
                                newHardwareGroup.listLED_PLC = new List<Data_Config_Device_LED_PLC>();
                                newHardwareGroup.listLED_PLC.Clear();
                                for (int j = newHardwareGroup.startDevID; j <= newHardwareGroup.endDevID; j++)
                                {
                                    Data_Config_Device_LED_PLC newDev = new Data_Config_Device_LED_PLC();
                                    newDev.hardwareID = newHardwareGroup.hardwareID;
                                    newDev.groupID = newHardwareGroup.groupID;
                                    newDev.devID = j;
                                    newDev.type = newHardwareGroup.type;
                                    newDev.name = "";
                                    newHardwareGroup.listLED_PLC.Add(newDev);
                                }
                            }
                            break;
                        default:
                            {

                            }
                            break;
                    }
                    // Add group
                    dataConfigHardware.listHardware[i].listGroup.Add(newHardwareGroup);
                    i = dataConfigHardware.listHardware.Count;
                }
            }
            //{// Write hardware config to file
            //    if (File.Exists(hardwareConfigFilePath))
            //    {// Read data and deserialize
            //        File.WriteAllText(hardwareConfigFilePath, JsonSerializer.Serialize(dataConfigHardware));
            //    }
            //    else
            //    {
            //        MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
            //    }
            //}
            return newHardwareGroup.groupID;
        }
        public Data_Config_Hardware DeleteAllGroupInHardware(int hardwareID)
        {
            Data_Config_Hardware deleteHardwareGroup = null;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Find hardware
                if (dataConfigHardware.listHardware[i].hardwareID == hardwareID)
                {
                    dataConfigHardware.listHardware[i].listGroup.Clear();
                    i = dataConfigHardware.listHardware.Count;
                }
            }
            {// Write hardware config to file
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(hardwareConfigFilePath, JsonSerializer.Serialize(dataConfigHardware));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
                }
            }
            return deleteHardwareGroup;

        }
        public Data_Config_Hardware_Group DeleteHardwareGroupByID(int hardwareID, int groupID)
        {
            Data_Config_Hardware_Group deleteGroup = null;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Find hardware
                if (dataConfigHardware.listHardware[i].hardwareID == hardwareID)
                {
                    for (int j = 0; j < dataConfigHardware.listHardware[i].listGroup.Count; i++)
                    {// Find group and delete
                        if (dataConfigHardware.listHardware[i].listGroup[j].groupID == groupID)
                        {
                            deleteGroup = dataConfigHardware.listHardware[i].listGroup[j];
                            dataConfigHardware.listHardware[i].listGroup.RemoveAt(j);
                            j = dataConfigHardware.listHardware[i].listGroup.Count;
                        }
                    }
                    i = dataConfigHardware.listHardware.Count;
                }
            }
            {// Write hardware config to file
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(hardwareConfigFilePath, JsonSerializer.Serialize(dataConfigHardware));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + hardwareConfigFilePath);
                }
            }
            return deleteGroup;
        }
        public Data_Config_Hardware_Group EditHardwareGroup(Data_Config_Hardware_Group hardwareGroupBuf)
        {
            return null;
        }    
    
        // Edit effect
        public int AddEffect(Data_Config_Effect newEffect)
        {
            if (newEffect.effectID <= 0)
            {// Hardware no have ID now
                newEffect.effectID = dataConfigPlaylist.listEffect.Count + 1;
                for (int i = 0; i < dataConfigPlaylist.listEffect.Count; i++)
                {// Creat new ID
                    if (newEffect.effectID <= dataConfigPlaylist.listEffect[i].effectID)
                    {
                        newEffect.effectID = dataConfigPlaylist.listEffect[i].effectID + 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Bạn đã tự điền effectID\nVui lòng chắc chắn rằng effectID bạn điền là chính xác, nếu không sẽ gây ra lỗi hệ thống");
            }
            dataConfigPlaylist.listEffect.Add(newEffect);
            {// Write hardware config to file
                if (File.Exists(playlistConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(playlistConfigFilePath, JsonSerializer.Serialize(dataConfigPlaylist));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + playlistConfigFilePath);
                }
            }
            return newEffect.effectID;
        }
        public void DeleteAllEffect()
        {
            dataConfigPlaylist.listEffect.Clear();
            {// Write hardware config to file
                if (File.Exists(playlistConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(playlistConfigFilePath, JsonSerializer.Serialize(dataConfigPlaylist));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + playlistConfigFilePath);
                }
            }
        }

        // Edit music
        public Data_Config_Music EditDataConfigMusic(Data_Config_Music newMusicConfig)
        {
            {// Update new playlist
                bool checkNewMusic = true;
                Data_Config_Music newMusicBuf = new Data_Config_Music();
                {
                    newMusicBuf.musicPath = newMusicConfig.musicPath;
                    newMusicBuf.listEffect = new List<Data_Config_Effect_With_Time>();
                    // Order list time by timeID
                    for (int i = 0; i < newMusicConfig.listEffect.Count; i++)
                    {
                        bool effectAddCheckBuf = true;
                        for (int j = 0; j < newMusicBuf.listEffect.Count; j++)
                        {
                            if (newMusicConfig.listEffect[i].musicTimeID < newMusicBuf.listEffect[j].musicTimeID)
                            {
                                effectAddCheckBuf = false;
                                newMusicBuf.listEffect.Insert(j, newMusicConfig.listEffect[i]);
                                j = newMusicBuf.listEffect.Count;
                            }
                        }
                        if (effectAddCheckBuf)
                        {
                            newMusicBuf.listEffect.Add(newMusicConfig.listEffect[i]);
                        }
                    }
                }
                for (int i = 0; i < dataConfigPlaylist.listMusic.Count; i++)
                {// Edit music config in data json
                    if (dataConfigPlaylist.listMusic[i].musicPath == newMusicBuf.musicPath)
                    {
                        checkNewMusic = false;
                        dataConfigPlaylist.listMusic[i] = newMusicBuf;
                    }
                }
                if (checkNewMusic)
                {
                    dataConfigPlaylist.listMusic.Add(newMusicBuf);
                }
            }

            {// Write hardware config to file
                if (File.Exists(playlistConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(playlistConfigFilePath, JsonSerializer.Serialize(dataConfigPlaylist));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + playlistConfigFilePath);
                }
            }
            return newMusicConfig;
        }
        public void EditPlaylistMode(Data_Config_Playlist_Mode newPlayListMode)
        {
            dataConfigPlaylist.playlistMode = newPlayListMode;
            {// Write hardware config to file
                if (File.Exists(playlistConfigFilePath))
                {// Read data and deserialize
                    File.WriteAllText(playlistConfigFilePath, JsonSerializer.Serialize(dataConfigPlaylist));
                }
                else
                {
                    MessageBox.Show("Data_Config: Can not find file with path: " + playlistConfigFilePath);
                }
            }
        }
    }
}
