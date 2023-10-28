using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MusicFountain
{

    public class Data_Config_Device_VSA
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
        public float setAngle { get; set; }
        public float currentAngle { get; set; }
        public float setSpeed { get; set; }
        public float currentSpeed { get; set; }
    }
    public class Data_Config_Device_VOA
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
    public class Data_Config_Device_Inverter
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public int groupID { get; set; }
    }
    public class Data_Config_Device_LED
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public int groupID { get; set; }
    }
    public class Data_Config_Hardware_Config
    {
        public int hardwareID { get; set; }
        public string name { get; set; }
        public string opcServerURL { get; set; }
        public int opcServerPort { get; set; }
        public string artnetServerURL { get; set; }
        public string type { get; set; }
        public List<Data_Config_Device_VSA> listVSA { get; set; }
        public List<Data_Config_Device_VOA> listVOA { get; set; }
        public List<Data_Config_Device_Inverter> listInverter { get; set; }
        public List<Data_Config_Device_LED> listLED { get; set; }

    }
    public class Data_Config_Hardware_Option_VSA
    {
        public float maxAngle { get; set; }
        public float maxSpeed { get; set; }
        public float coordAngle { get; set; }
        public float ratio { get; set; }

    }
    public class Data_Config_Hardware_Option
    {
        public Data_Config_Hardware_Option_VSA vsaOption { get; set; }
    }
    public class Data_Config_Hardware_Group
    {
        public int groupID { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<Data_Config_Device_VSA> listVSA { get; set; }
        public List<Data_Config_Device_VOA> listVOA { get; set; }
        public List<Data_Config_Device_Inverter> listInverter { get; set; }
        public List<Data_Config_Device_LED> listLED { get; set; }

    }
    public class Data_Config_Hardware
    {
        public Data_Config_Hardware_Option hardwareOption { get; set; }
        public List<Data_Config_Hardware_Config> listHardware { get; set; }
        public List<Data_Config_Hardware_Group> listGroup { get; set; }
    }

    public class Data_Config_Playlist
    {

    }

    public partial class Data_Config
    {
        // Variables
        string hardwareConfigFilePath;
        string playlistConfigFilePath;

        Data_Config_Hardware dataConfigHardware;
        Data_Config_Playlist dataConfigPlaylist;

        public Data_Config()
        {
            {// Creat variables
                hardwareConfigFilePath = "../Data/HardwareConfig.json";
                playlistConfigFilePath = "../Data/PlaylistConfig.json";

                dataConfigHardware = new Data_Config_Hardware();
                dataConfigPlaylist = new Data_Config_Playlist();
            }

            {// Read hardware config
                if (File.Exists(hardwareConfigFilePath))
                {// Read data and deserialize
                    string jsonStrBuf = File.ReadAllText(hardwareConfigFilePath);
                    try
                    {
                        dataConfigHardware = JsonSerializer.Deserialize<Data_Config_Hardware>(jsonStrBuf);
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

        public Data_Config_Hardware Get_DataConfigHardware()
        {
            return dataConfigHardware;
        }
        public Data_Config_Playlist Get_DataConfigPlaylist()
        {
            return dataConfigPlaylist;
        }

        public int AddPLC(Data_Config_Hardware_Config newPLC)
        {
            newPLC.hardwareID = dataConfigHardware.listHardware.Count + 1;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Creat new ID
                if (newPLC.hardwareID <= dataConfigHardware.listHardware[i].hardwareID)
                {
                    newPLC.hardwareID = dataConfigHardware.listHardware[i].hardwareID + 1;
                }
            }
            dataConfigHardware.listHardware.Add(newPLC);
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
            return newPLC.hardwareID;
        }
        public Data_Config_Hardware_Config DeletePLCByGID(int plcGID)
        {
            Data_Config_Hardware_Config deletedPLC = null;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Delete PLC
                if (dataConfigHardware.listHardware[i].hardwareID == plcGID)
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
        public Data_Config_Hardware_Config EditPLC(Data_Config_Hardware_Config plcBuf)
        {
            Data_Config_Hardware_Config editPLC = null;
            for (int i = 0; i < dataConfigHardware.listHardware.Count; i++)
            {// Edit PLC
                if (dataConfigHardware.listHardware[i].hardwareID == plcBuf.hardwareID)
                {
                    dataConfigHardware.listHardware[i] = plcBuf;
                    editPLC = dataConfigHardware.listHardware[i];
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
            return editPLC;
        }

        public int AddGroup(Data_Config_Hardware_Group newGroup)
        {
            newGroup.groupID = dataConfigHardware.listGroup.Count + 1;
            for (int i = 0; i < dataConfigHardware.listGroup.Count; i++)
            {// Creat new ID
                if (newGroup.groupID <= dataConfigHardware.listHardware[i].hardwareID)
                {
                    newGroup.groupID = dataConfigHardware.listHardware[i].hardwareID + 1;
                }
            }
            dataConfigHardware.listGroup.Add(newGroup);
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
            return newGroup.groupID;
        }
        public Data_Config_Hardware_Group DeleteGroupByGID(int groupID)
        {
            Data_Config_Hardware_Group deletedGroup = null;
            for (int i = 0; i < dataConfigHardware.listGroup.Count; i++)
            {// Delete PLC
                if (dataConfigHardware.listGroup[i].groupID == groupID)
                {
                    deletedGroup = dataConfigHardware.listGroup[i];
                    dataConfigHardware.listGroup.RemoveAt(i);
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
            return deletedGroup;
        }
        public Data_Config_Hardware_Group EditGroup(Data_Config_Hardware_Group groupBuf)
        {
            Data_Config_Hardware_Group editGroup = null;
            for (int i = 0; i < dataConfigHardware.listGroup.Count; i++)
            {// Edit PLC
                if (dataConfigHardware.listGroup[i].groupID == groupBuf.groupID)
                {
                    dataConfigHardware.listGroup[i] = groupBuf;
                    editGroup = dataConfigHardware.listGroup[i];
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
            return editGroup;
        }
    }
}
