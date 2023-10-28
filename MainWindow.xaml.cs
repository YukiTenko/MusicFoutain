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
        public Connection_PLC connectionPLC;
        public Connection_Artnet connectionArtNet;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Data
        private Data_Config dataConfig;
        private Data_Log dataLog;
        private Data_Login dataLogin;

        // UI
        private UI_ConfigGroup uiConfigGroup;
        private UI_ConfigHardwareOption uiConfigHardwareOption;
        private UI_ConfigHardware_VO_PLC uiConfigHardware_VO_PLC;
        private UI_ConfigHardware_VS_PLC uiConfigHardware_VS_PLC;
        private UI_ConfigHardware uiConfigHardware;
        private UI_EditMusicEffect uiMusicEffect;
        private UI_EditPlaylist uiEditPlaylist;
        private UI_GroupEdit_VO_PLC uiGroupEdit_VO_PLC;
        private UI_GroupEdit_VS_PLC uiGroupEdit_VS_PLC;
        private UI_Log uiLog;
        private UI_Login uiLogin;
        private UI_Running uiRunning;

        private Test testUI;

        // Connection
        private List<Connection_Hardware> listConnectionHardware;

        // Control algorithm
        private Control_Algorithm controlAlgorithm;

public MainWindow()
        {
            InitializeComponent();

            // Creat data struct + Update config and log data
            {
                dataConfig = new Data_Config();
                dataLog = new Data_Log();
                dataLogin = new Data_Login();
            }

            // Creat UI
            {
                uiConfigGroup = new UI_ConfigGroup("../UI/ConfigGroup.xml");
                uiConfigGroup.backClick += UIConfigGroup_BackButtonClick_EventHandle;
                uiConfigGroup.saveClick += UIConfigGroup_SaveButtonClick_EventHandle;
                uiConfigGroup.editClick += UIConfigGroup_EditButtonClick_EventHandle;
                uiConfigGroup.deleteClick += UIConfigGroup_DeleteButtonClick_EventHandle;
                uiConfigGroup.addClick += UIConfigGroup_AddButtonClick_EventHandle;
                uiConfigGroup.Refresh();

                uiConfigHardwareOption = new UI_ConfigHardwareOption("../UI/ConfigHardwareOption.xml");
                uiConfigHardwareOption.backClick += UIConfigHardwareOption_BackButtonClick_EventHandle;
                uiConfigHardwareOption.saveClick += UIConfigHardwareOption_SaveButtonClick_EventHandle;
                uiConfigHardwareOption.Refresh();

                uiConfigHardware = new UI_ConfigHardware("../UI/ConfigHardware.xml");
                uiConfigHardware.backClick += UIConfigHardware_BackButtonClick_EventHandle;
                uiConfigHardware.optionClick += UIConfigHardware_OptionButtonClick_EventHandle;
                uiConfigHardware.saveClick += UIConfigHardware_SaveButtonClick_EventHandle;
                uiConfigHardware.editClick += UIConfigHardware_EditButtonClick_EventHandle;
                uiConfigHardware.deleteClick += UIConfigHardware_DeleteButtonClick_EventHandle;
                uiConfigHardware.addClick += UIConfigHardware_AddButtonClick_EventHandle;
                uiConfigHardware.Refresh();

                uiConfigHardware_VO_PLC = new UI_ConfigHardware_VO_PLC("../UI/ConfigHardware_VO_PLC.xml");
                uiConfigHardware_VO_PLC.backClick += UIConfigHardware_VO_PLC_BackButtonClick_EventHandle;
                uiConfigHardware_VO_PLC.saveClick += UIConfigHardware_VO_PLC_SaveButtonClick_EventHandle;
                uiConfigHardware_VO_PLC.onOffTestClick += UIConfigHardware_VO_PLC_OnOffTestButtonClick_EventHandle;
                uiConfigHardware_VO_PLC.Refresh();

                uiConfigHardware_VS_PLC = new UI_ConfigHardware_VS_PLC("../UI/ConfigHardware_VS_PLC.xml");
                uiConfigHardware_VS_PLC.backClick += UIConfigHardware_VS_PLC_BackButtonClick_EventHandle;
                uiConfigHardware_VS_PLC.saveClick += UIConfigHardware_VS_PLC_SaveButtonClick_EventHandle;
                uiConfigHardware_VS_PLC.moveTestClick += UIConfigHardware_VS_PLC_MoveTestButtonClick_EventHandle;
                uiConfigHardware_VS_PLC.goHomeClick += UIConfigHardware_VS_PLC_GoHomeButtonClick_EventHandle;
                uiConfigHardware_VS_PLC.Refresh();

                uiMusicEffect = new UI_EditMusicEffect("../UI/EditMusicEffect.xml");
                uiMusicEffect.backClick += UIEditMusicEffect_BackButtonClick_EventHandle;
                uiMusicEffect.Refresh();

                uiEditPlaylist = new UI_EditPlaylist("../UI/EditPlaylist.xml");
                uiEditPlaylist.backClick += UIEditPlaylist_BackButtonClick_EventHandle;
                uiEditPlaylist.addClick += UIEditPlaylist_AddButtonClick_EventHandle;
                uiEditPlaylist.editClick += UIEditPlaylist_EditButtonClick_EventHandle;
                uiEditPlaylist.Refresh();

                uiGroupEdit_VO_PLC = new UI_GroupEdit_VO_PLC("../UI/GroupEdit_VO_PLC.xml");
                uiGroupEdit_VO_PLC.backClick += uiGroupEdit_VO_PLC_BackButtonClick_EventHandle;
                uiGroupEdit_VO_PLC.Refresh();

                uiGroupEdit_VS_PLC = new UI_GroupEdit_VS_PLC("../UI/GroupEdit_VS_PLC.xml");
                uiGroupEdit_VS_PLC.backClick += uiGroupEdit_VS_PLC_BackButtonClick_EventHandle;
                uiGroupEdit_VS_PLC.Refresh();

                uiLog = new UI_Log("../UI/Log.xml");
                uiLog.backClick += UILog_BackButtonClick_EventHandle;
                uiLog.prevClick += UILog_PrevButtonClick_EventHandle;
                uiLog.nextClick += UILog_NextButtonClick_EventHandle;
                uiLog.Refresh();

                uiLogin = new UI_Login("../UI/Login.xml");
                uiLogin.loginClick += UILogin_LoginButtonClick_EventHandle;
                uiLogin.Refresh();

                uiRunning = new UI_Running("../UI/Running.xml");
                uiRunning.backClick += UIRunning_BackButtonClick_EventHandle;
                uiRunning.configGroupClick += UIRunning_ConfigGroupButtonClick_EventHandle;
                uiRunning.configHardwareClick += UIRunning_ConfigHardwareButtonClick_EventHandle;
                uiRunning.editPlaylistClick += UIRunning_EditPlaylistButtonClick_EventHandle;
                uiRunning.logClick += UIRunning_LogButtonClick_EventHandle;
                uiRunning.Refresh();

                testUI = new Test();

                mainLayout.Content = uiRunning;
                mainLayout.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                mainLayout.NavigationService.RemoveBackEntry();
            }

            // Creat connection
            {
                listConnectionHardware = new List<Connection_Hardware>();
                listConnectionHardware.Clear();
                for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
                {
                    Connection_Hardware connectionHardware = new Connection_Hardware();
                    switch (dataConfig.Get_DataConfigHardware().listHardware[i].type)
                    {
                        case "Valve Stepper - PLC":
                        case "Valve On/Off - PLC":
                            {
                                connectionHardware.connectionPLC = new Connection_PLC(dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID);
                                connectionHardware.connectionPLC.Connect2OPCUAServer(dataConfig.Get_DataConfigHardware().listHardware[i].opcServerURL);
                            }
                            break;
                        case "LED - ArtnetDMX":
                            {
                                connectionHardware.connectionArtNet = new Connection_Artnet(IPAddress.Parse("192.168.0.22"), IPAddress.Parse("255.255.255.0"));
                            }
                            break;
                        default:
                            {
                                MessageBox.Show("Creat connection: Unknown hardware type: " + dataConfig.Get_DataConfigHardware().listHardware[i].type + ", hardwareID: " + dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID.ToString());
                            }
                            break;
                    }
                    listConnectionHardware.Add(connectionHardware);
                }
            }

            // Creat control algorithm
            {
                controlAlgorithm = new Control_Algorithm();
            }
        }
        // Private methods -------------------------------------------------------------------------------
        private void UIConfigGroup_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiRunning.Refresh();
            mainLayout.Content = uiRunning;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIConfigGroup_SaveButtonClick_EventHandle(object sender, List<Data_Config_Hardware_Group> listGroupBuf)
        {

        }
        private void UIConfigGroup_EditButtonClick_EventHandle(object sender, Data_Config_Hardware_Group groupBuf)
        {
            switch (groupBuf.type)
            {
                case "Valve Stepper - PLC":
                    {
                        uiGroupEdit_VS_PLC.Refresh();
                        mainLayout.Content = uiGroupEdit_VS_PLC;
                        mainLayout.NavigationService.RemoveBackEntry();
                    }
                    break;
                case "Valve On/Off - PLC":
                    {
                        uiGroupEdit_VO_PLC.Refresh();
                        mainLayout.Content = uiGroupEdit_VO_PLC;
                        mainLayout.NavigationService.RemoveBackEntry();
                    }
                    break;
                case "Inverter - A":
                    {
                    }
                    break;
                case "LED - ArtnetDMX":
                    {
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }
        private void UIConfigGroup_DeleteButtonClick_EventHandle(object sender, int groupID)
        {
            Data_Config_Hardware_Group deleteGroup = dataConfig.DeleteGroupByGID(groupID);
            if (deleteGroup == null)
            {
                MessageBox.Show("Xoá Group thất bại .\nKhông tìm được Group với groupID: " + groupID + " .");
            }
            else
            {
                MessageBox.Show("Xoá " + deleteGroup.name + " thành công.");
            }
            // Update new list PLC
            uiConfigGroup.Update_ListGroup(dataConfig.Get_DataConfigHardware().listGroup);
        }
        private void UIConfigGroup_AddButtonClick_EventHandle(object sender, Data_Config_Hardware_Group groupBuf)
        {
            int newGroupID = dataConfig.AddGroup(groupBuf);
            if (newGroupID > 0)
            {
                MessageBox.Show(groupBuf.name + "đã được thêm với groupID: " + newGroupID.ToString() + ".");
            }
            else
            {
                MessageBox.Show("Thêm group thất bại .");
            }
            // Update new list PLC
            uiConfigGroup.Update_ListGroup(dataConfig.Get_DataConfigHardware().listGroup);
        }
        private void UIConfigHardwareOption_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiConfigHardware.Refresh();
            mainLayout.Content = uiConfigHardware;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIConfigHardwareOption_SaveButtonClick_EventHandle(object sender, int noUse)
        {

        }
        private void UIConfigHardware_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiRunning.Refresh();
            mainLayout.Content = uiRunning;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIConfigHardware_OptionButtonClick_EventHandle(object sender, int noUse)
        {
            uiConfigHardwareOption.Refresh();
            mainLayout.Content = uiConfigHardwareOption;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIConfigHardware_SaveButtonClick_EventHandle(object sender, List<Data_Config_Hardware_Config> newListPLC)
        {
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                for (int j = 0; j < newListPLC.Count; j++)
                {
                    if (dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == newListPLC[j].hardwareID)
                    {
                        dataConfig.Get_DataConfigHardware().listHardware[i].name = newListPLC[j].name;
                        dataConfig.Get_DataConfigHardware().listHardware[i].type = newListPLC[j].type;
                        dataConfig.Get_DataConfigHardware().listHardware[i].opcServerURL = newListPLC[j].opcServerURL;
                    }
                }
            }
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                dataConfig.EditPLC(dataConfig.Get_DataConfigHardware().listHardware[i]);
            }
            MessageBox.Show("Lưu thông tin cấu hình PLC thành công .");
        }
        private void UIConfigHardware_EditButtonClick_EventHandle(object sender, int plcGID)
        {
            bool plcGIDCheck = false;
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID == plcGID)
                {
                    switch (dataConfig.Get_DataConfigHardware().listHardware[i].type)
                    {
                        case "Valve Stepper - PLC":
                            {
                                uiConfigHardware_VS_PLC.Refresh();
                                uiConfigHardware_VS_PLC.Update_PLCInfo(dataConfig.Get_DataConfigHardware().listHardware[i]);
                                mainLayout.Content = uiConfigHardware_VS_PLC;
                                mainLayout.NavigationService.RemoveBackEntry();
                            }
                            break;
                        case "Valve On/Off - PLC":
                            {
                                uiConfigHardware_VO_PLC.Refresh();
                                uiConfigHardware_VO_PLC.Update_PLCInfo(dataConfig.Get_DataConfigHardware().listHardware[i]);
                                mainLayout.Content = uiConfigHardware_VO_PLC;
                                mainLayout.NavigationService.RemoveBackEntry();
                            }
                            break;
                        case "LED - ArtnetDMX":
                            {
                                uiConfigHardware_VO_PLC.Refresh();
                                uiConfigHardware_VO_PLC.Update_PLCInfo(dataConfig.Get_DataConfigHardware().listHardware[i]);
                                mainLayout.Content = uiConfigHardware_VO_PLC;
                                mainLayout.NavigationService.RemoveBackEntry();
                            }
                            break;
                        default:
                            {

                            }
                            break;
                    }
                    plcGIDCheck = true;
                }
            }
            if (plcGIDCheck == false)
            {
                MessageBox.Show("UIConfigHardware_EditButtonClick_EventHandle: Cannot find PLC with hardwareID: " + plcGID.ToString());
            }
        }
        private void UIConfigHardware_DeleteButtonClick_EventHandle(object sender, int plcGID)
        {
            Data_Config_Hardware_Config deletedPLC = dataConfig.DeletePLCByGID(plcGID);
            if (deletedPLC == null)
            {
                MessageBox.Show("Xoá PLC thất bại .\nKhông tìm được PLC với hardwareID: " + plcGID + " .");
            } 
            else
            {
                MessageBox.Show("Xoá " + deletedPLC.name + " thành công.");
            }
            // Update new list PLC
            uiConfigHardware.Update_ListHardware(dataConfig.Get_DataConfigHardware().listHardware);
        }
        private void UIConfigHardware_AddButtonClick_EventHandle(object sender, Data_Config_Hardware_Config newPLC)
        {
            int newPLCID = dataConfig.AddPLC(newPLC);
            if (newPLCID > 0)
            {
                MessageBox.Show(newPLC.name + "đã được thêm với hardwareID: " + newPLCID.ToString() + ".");
                for (int i = 0; i < newPLC.listVSA.Count; i++)
                {
                    newPLC.listVSA[i].hardwareID = newPLCID;
                }
                for (int i = 0; i < newPLC.listInverter.Count; i++)
                {
                    newPLC.listInverter[i].hardwareID = newPLCID;
                }
                for (int i = 0; i < newPLC.listLED.Count; i++)
                {
                    newPLC.listLED[i].hardwareID = newPLCID;
                }
            } 
            else
            {
                MessageBox.Show("Thêm PLC thất bại .");
            }
            // Update new list PLC
            uiConfigHardware.Update_ListHardware(dataConfig.Get_DataConfigHardware().listHardware);
        }
        private void UIConfigHardware_VO_PLC_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiConfigHardware.Refresh();
            mainLayout.Content = uiConfigHardware;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIConfigHardware_VO_PLC_SaveButtonClick_EventHandle(object sender, Data_Config_Hardware_Config hardwareConfig)
        {
            bool hardwareIDCheck = false;
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (hardwareConfig.hardwareID == dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID)
                {
                    hardwareIDCheck = true;
                    dataConfig.Get_DataConfigHardware().listHardware[i].listVOA.Clear();
                    for (int j = 0; j < hardwareConfig.listVOA.Count; j++)
                    {
                        dataConfig.Get_DataConfigHardware().listHardware[i].listVOA.Add(hardwareConfig.listVOA[j]);
                    }
                    Data_Config_Hardware_Config editPLC = dataConfig.EditPLC(dataConfig.Get_DataConfigHardware().listHardware[i]);
                    if (editPLC == null)
                    {
                        MessageBox.Show("Cập nhật thông tin cho PLC thất bại .");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thông tin cho " + editPLC.name + " thành công .");
                    }
                    i = dataConfig.Get_DataConfigHardware().listHardware.Count;
                }
            }
            if (hardwareIDCheck == false)
            {
                MessageBox.Show("Cập nhật thông tin cho PLC thất bại. Không thể tìm thấy PLC với ID: " + hardwareConfig.hardwareID + " .");
            }
        }
        private void UIConfigHardware_VO_PLC_OnOffTestButtonClick_EventHandle(object sender, Data_Config_Device_VOA valveConfig)
        {
            for (int i = 0; i < listConnectionHardware.Count; i++)
            {
                if (listConnectionHardware[i].connectionPLC != null)
                {
                    if (valveConfig.hardwareID == listConnectionHardware[i].connectionPLC.Get_PLCID())
                    {
                        listConnectionHardware[i].connectionPLC.ChangeValveInfo_VOA(valveConfig);
                    }
                }
            }
        }
        private void UIConfigHardware_VS_PLC_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiConfigHardware.Refresh();
            mainLayout.Content = uiConfigHardware;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIConfigHardware_VS_PLC_SaveButtonClick_EventHandle(object sender, Data_Config_Hardware_Config hardwareConfig)
        {
            bool hardwareIDCheck = false;
            for (int i = 0; i < dataConfig.Get_DataConfigHardware().listHardware.Count; i++)
            {
                if (hardwareConfig.hardwareID == dataConfig.Get_DataConfigHardware().listHardware[i].hardwareID)
                {
                    hardwareIDCheck = true;
                    dataConfig.Get_DataConfigHardware().listHardware[i].listVSA.Clear();
                    for (int j = 0; j < hardwareConfig.listVSA.Count; j++)
                    {
                        dataConfig.Get_DataConfigHardware().listHardware[i].listVSA.Add(hardwareConfig.listVSA[j]);
                    }
                    Data_Config_Hardware_Config editPLC = dataConfig.EditPLC(dataConfig.Get_DataConfigHardware().listHardware[i]);
                    if (editPLC == null)
                    {
                        MessageBox.Show("Cập nhật thông tin cho PLC thất bại .");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thông tin cho " + editPLC.name + " thành công .");
                    }
                    i = dataConfig.Get_DataConfigHardware().listHardware.Count;
                }
            }
            if (hardwareIDCheck == false)
            {
                MessageBox.Show("Cập nhật thông tin cho PLC thất bại. Không thể tìm thấy PLC với ID: " + hardwareConfig.hardwareID + " .");
            }
        }
        private void UIConfigHardware_VS_PLC_MoveTestButtonClick_EventHandle(object sender, Data_Config_Device_VSA valveConfig)
        {
            for (int i = 0; i < listConnectionHardware.Count; i++)
            {
                if (listConnectionHardware[i].connectionPLC != null)
                {
                    if (valveConfig.hardwareID == listConnectionHardware[i].connectionPLC.Get_PLCID())
                    {
                        listConnectionHardware[i].connectionPLC.ChangeValveInfo_VSA(valveConfig);
                    }
                }
            }
        }
        private void UIConfigHardware_VS_PLC_GoHomeButtonClick_EventHandle(object sender, Data_Config_Device_VSA valveConfig)
        {
            for (int i = 0; i < listConnectionHardware.Count; i++)
            {
                if (listConnectionHardware[i].connectionPLC != null)
                {
                    if (valveConfig.hardwareID == listConnectionHardware[i].connectionPLC.Get_PLCID())
                    {
                        listConnectionHardware[i].connectionPLC.ChangeValveInfo_VSA(valveConfig);
                    }
                }
            }
        }
        private void UIEditMusicEffect_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiEditPlaylist.Refresh();
            mainLayout.Content = uiEditPlaylist;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIEditPlaylist_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiRunning.Refresh();
            mainLayout.Content = uiRunning;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIEditPlaylist_AddButtonClick_EventHandle(object sender, int noUse)
        {
            MessageBox.Show("Vui lòng chờ phần backend viết xong.");
        }
        private void UIEditPlaylist_EditButtonClick_EventHandle(object sender, int noUse)
        {
            uiMusicEffect.Refresh();
            mainLayout.Content = uiMusicEffect;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void uiGroupEdit_VS_PLC_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiRunning.Refresh();
            mainLayout.Content = uiConfigGroup;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void uiGroupEdit_VO_PLC_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiRunning.Refresh();
            mainLayout.Content = uiConfigGroup;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UILog_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiRunning.Refresh();
            mainLayout.Content = uiRunning;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UILog_PrevButtonClick_EventHandle(object sender, int noUse)
        {
            MessageBox.Show("Vui lòng chờ phần backend viết xong.");
        }
        private void UILog_NextButtonClick_EventHandle(object sender, int noUse)
        {
            MessageBox.Show("Vui lòng chờ phần backend viết xong.");
        }
        private void UILogin_LoginButtonClick_EventHandle(object sender, Data_UserInfo userInfo)
        {
            if (dataLogin.Compare_UserNameAndPassword(userInfo.userName, userInfo.password))
            {
                uiRunning.Refresh();
                mainLayout.Content = uiRunning;
                mainLayout.NavigationService.RemoveBackEntry();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.\nVui lòng thử lại.");
            }
        }
        private void UIRunning_BackButtonClick_EventHandle(object sender, int noUse)
        {
            uiLogin.Refresh();
            mainLayout.Content = uiLogin;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIRunning_ConfigGroupButtonClick_EventHandle(object sender, int noUse)
        {
            uiConfigGroup.Refresh();
            uiConfigGroup.Update_ListGroup(dataConfig.Get_DataConfigHardware().listGroup);
            mainLayout.Content = uiConfigGroup;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIRunning_ConfigHardwareButtonClick_EventHandle(object sender, int noUse)
        {
            uiConfigHardware.Refresh();
            uiConfigHardware.Update_ListHardware(dataConfig.Get_DataConfigHardware().listHardware);
            mainLayout.Content = uiConfigHardware;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIRunning_EditPlaylistButtonClick_EventHandle(object sender, int noUse)
        {
            uiEditPlaylist.Refresh();
            mainLayout.Content = uiEditPlaylist;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        private void UIRunning_LogButtonClick_EventHandle(object sender, int noUse)
        {
            uiLog.Refresh();
            mainLayout.Content = uiLog;
            mainLayout.NavigationService.RemoveBackEntry();
        }
        // Public methods -------------------------------------------------------------------------------
        void Window_Closing(object sender, CancelEventArgs e)
        {
        }
    }
}
