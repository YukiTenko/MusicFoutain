using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using static System.Resources.ResXFileRef;

namespace MusicFountain.UI.Hardware
{
    /// <summary>
    /// Interaction logic for UI_HardwareConfig.xaml
    /// </summary>
    /// 
    public partial class UI_HardwareConfig : Page
    {
        // Variables
        private Data.Data_Config_List_Hardware listHardwareConfig;

        // Sub items
        private TreeView listHardwareMenu_TreeView;

        // Sub UIs
        private ListHardware.UI_ListHardware uiListHardware;

        private Setting.UI_Setting_ValveStep_PLC uiSetting_ValveStepPLC;
        private Setting.UI_Setting_PumpAD_PLC uiSetting_PumpADPLC;

        private Group.UI_Group_ValveStep_PLC uiGroup_ValveStepPLC;
        private Group.UI_Group_ValveOnOff_PLC uiGroup_ValveOnOffPLC;
        private Group.UI_Group_PumpADAndPower_PLC uiGroup_PumpADAndPowerPLC;
        private Group.UI_Group_LEDDMX_Artnet uiGroup_LEDDMX_Artnet;
        private Group.UI_Group_LED485_PLC uiGroup_LED485_PLC;
        private Group.UI_Group_ValveOnOffAndStep_PLC uiGroup_ValveOnOffAndStepPLC;
        private Group.UI_Group_ValveStepAndLED485_PLC uiGroup_ValveStepAndLED485PLC;

        private UI_Unknown ui_Unknown;

        // Events
        public event EventHandler<List<Data.Data_Config_Hardware>> updateNewListHardwareEvent;
        public event EventHandler<int> reconnectHardwareEvent;
        public event EventHandler<Data.Data_Config_Hardware_Group> groupTestClick;
        public event EventHandler<Data.Data_Config_Hardware> groupSaveClick;
        public event EventHandler<Data.Data_Config_Hardware> updateHardwareOption;

        public UI_HardwareConfig()
        {
            InitializeComponent();
            {// Creat variables
                listHardwareConfig = new Data.Data_Config_List_Hardware();
                listHardwareConfig.listHardware = new List<Data.Data_Config_Hardware>();
            }
            {// Sub UIs creat
                {// List hardware UIs
                    uiListHardware = new ListHardware.UI_ListHardware();
                    uiListHardware.updateNewListHardwareEvent += ListHardwareUpdate_EventHandle;
                    uiListHardware.reconnectHardwareEvent += ReconnectHardware_EventHandle;
                }
                {// Setting UIs
                    uiSetting_ValveStepPLC = new Setting.UI_Setting_ValveStep_PLC();
                    uiSetting_ValveStepPLC.updateHardwareOption += OptionSettingUpdate_EventHandle;

                    uiSetting_PumpADPLC = new Setting.UI_Setting_PumpAD_PLC();
                    uiSetting_PumpADPLC.updateHardwareOption += OptionSettingUpdate_EventHandle;
                }
                {// Group UIs
                    uiGroup_ValveStepPLC = new Group.UI_Group_ValveStep_PLC();
                    uiGroup_ValveStepPLC.testClickEvent += GroupTest_EventHandle;
                    uiGroup_ValveStepPLC.updateClickEvent += GroupUpdate_EventHandle;

                    uiGroup_LEDDMX_Artnet = new Group.UI_Group_LEDDMX_Artnet();
                    uiGroup_LEDDMX_Artnet.testClickEvent += GroupTest_EventHandle;
                    uiGroup_LEDDMX_Artnet.updateClickEvent += GroupUpdate_EventHandle;

                    uiGroup_PumpADAndPowerPLC = new Group.UI_Group_PumpADAndPower_PLC();
                    uiGroup_PumpADAndPowerPLC.testClickEvent += GroupTest_EventHandle;
                    uiGroup_PumpADAndPowerPLC.updateClickEvent += GroupUpdate_EventHandle;

                    uiGroup_LED485_PLC = new Group.UI_Group_LED485_PLC();
                    uiGroup_LED485_PLC.testClickEvent += GroupTest_EventHandle;
                    uiGroup_LED485_PLC.updateClickEvent += GroupUpdate_EventHandle;

                    uiGroup_ValveOnOffPLC = new Group.UI_Group_ValveOnOff_PLC();
                    uiGroup_ValveOnOffPLC.testClickEvent += GroupTest_EventHandle;
                    uiGroup_ValveOnOffPLC.updateClickEvent += GroupUpdate_EventHandle;

                    uiGroup_ValveOnOffAndStepPLC = new Group.UI_Group_ValveOnOffAndStep_PLC();
                    uiGroup_ValveOnOffAndStepPLC.testClickEvent += GroupTest_EventHandle;
                    uiGroup_ValveOnOffAndStepPLC.updateClickEvent += GroupUpdate_EventHandle;

                    uiGroup_ValveStepAndLED485PLC = new Group.UI_Group_ValveStepAndLED485_PLC();
                    uiGroup_ValveStepAndLED485PLC.testClickEvent += GroupTest_EventHandle;
                    uiGroup_ValveStepAndLED485PLC.updateClickEvent += GroupUpdate_EventHandle;
                }
                {// Others
                    ui_Unknown = new UI_Unknown();
                }
                uiListHardware.Refresh();
                hardwareConfig_SubFrame.Content = uiListHardware;
                hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
            }
            {// Sub items creat
                ListHardwareMenuUpdate();
            }
        }
        // Public methods
        public void Update_HardwareConfig(Data.Data_Config_List_Hardware newHardwareConfig)
        {
            listHardwareConfig = newHardwareConfig;
            // Update new hardwareListMenu
            ListHardwareMenuUpdate();
            uiListHardware.Update_ListHardware(listHardwareConfig.listHardware);
        }
        public void Update_HardwareConnectionState(int hardwareIDBuf, string connectionStateBuf)
        {
            uiListHardware.Update_HardwareConnectionState(hardwareIDBuf, connectionStateBuf);
        }

        public void ApplicationExit()
        {
        }

        // Private methods
        private void ListHardwareUpdate_EventHandle(object sender, ObservableCollection<ListHardware.UI_ListHardware.DataGrid_HardwareConfig> newList)
        {
            bool checkNewListData = true;
            List<Data.Data_Config_Hardware> newlistHardware = new List<Data.Data_Config_Hardware>();
            for (int i = 0; i < newList.Count; i++)
            {
                Data.Data_Config_Hardware newPLCConfig = new Data.Data_Config_Hardware();
                UriHostNameType checkURLBuf = Uri.CheckHostName(newList[i].serverURL);
                if (checkURLBuf == UriHostNameType.IPv4)
                {
                    newPLCConfig.hardwareID = newList[i].id;
                    newPLCConfig.name = newList[i].name;
                    newPLCConfig.type = newList[i].type;
                    newPLCConfig.opcServerURL = newList[i].serverURL;
                    newPLCConfig.artnetServerURL = newList[i].serverURL;
                    newPLCConfig.modbusServerURL = newList[i].serverURL;
                    newlistHardware.Add(newPLCConfig);
                }
                else
                {
                    checkNewListData = false;
                }
            }
            if (checkNewListData)
            {
                if (updateNewListHardwareEvent != null)
                {
                    updateNewListHardwareEvent.Invoke(this, newlistHardware);
                }
            }
            else
            {
                MessageBox.Show("Địa chỉ IP không đúng định dạng\nVui lòng điền lại địa chỉ IP");
            }
        }
        
        private void GroupTest_EventHandle(object sender, Data.Data_Config_Hardware_Group groupTest)
        {
            groupTestClick.Invoke(this, groupTest);
        }
        private void GroupUpdate_EventHandle(object sender, Data.Data_Config_Hardware newHardwareGroup)
        {
            if (groupSaveClick != null)
            {
                groupSaveClick.Invoke(this, newHardwareGroup);
            }
        }
        
        private void OptionSettingUpdate_EventHandle(object sender, Data.Data_Config_Hardware newHardwareOption)
        {
            
            if (updateHardwareOption != null)
            {
                updateHardwareOption.Invoke(this, newHardwareOption);
            }
        }

        private void ReconnectHardware_EventHandle(object sender, int hardwareIDBuf)
        {
            if (reconnectHardwareEvent != null)
            {
                reconnectHardwareEvent.Invoke(this, hardwareIDBuf);
            }
        }
        
        private void ListHardwareMenuUpdate()
        {
            listHardwareMenu_StackPanel.Children.Clear();

            listHardwareMenu_TreeView = new TreeView();
            listHardwareMenu_TreeView.FontSize = 14;
            listHardwareMenu_TreeView.Foreground = Brushes.White;
            listHardwareMenu_TreeView.Background = null;
            listHardwareMenu_TreeView.BorderBrush = null;
            // Lever 1 Project
            TreeViewItem TreeViewProject = ListHardwareMenu_ItemsTreeView("Project", "Cấu hình phần cứng", Brushes.White, new Uri("/Icon/hardware.png", UriKind.Relative), ListHardwareMenu_MainItem_DoubleClick_EventHandle);

            listHardwareMenu_TreeView.Items.Add(TreeViewProject);
            if (listHardwareConfig.listHardware != null)
            {
                if(listHardwareConfig.listHardware.Count > 0)
                {
                    for (int i = 0; i < listHardwareConfig.listHardware.Count; i++)
                    {
                        TreeViewProject.Items.Add(ListHardwareMenu_TreeViewCreat(i, listHardwareConfig.listHardware[i].name));
                    }
                }
            }
            listHardwareMenu_StackPanel.Children.Add(listHardwareMenu_TreeView);
        }
        private TreeViewItem ListHardwareMenu_TreeViewCreat(int index, string name)
        {
            TreeViewItem listHardwareMenu_Items = ListHardwareMenu_ItemsTreeView("Device" + index, name, Brushes.White, new Uri("/Icon/hardware.png", UriKind.Relative), ListHardwareMenu_MainItem_DoubleClick_EventHandle);
            if (listHardwareMenu_Items != null)
            {
                TreeViewItem listHardwareMenu_Item_Setting = ListHardwareMenu_ItemsTreeView("Genneral" + index, "Genneral Setting", Brushes.White, new Uri("/Icon/setting.png", UriKind.Relative), ListHardwareMenu_SubItemSetting_DoubleClick_EventHandle);
                listHardwareMenu_Items.Items.Add(listHardwareMenu_Item_Setting);

                TreeViewItem listHardwareMenu_Item_Group = ListHardwareMenu_ItemsTreeView("Group" + index, "Group", Brushes.White, new Uri("/Icon/group.png", UriKind.Relative), ListHardwareMenu_SubItemGroup_DoubleClick_EventHandle);
                listHardwareMenu_Items.Items.Add(listHardwareMenu_Item_Group);
            }
            return listHardwareMenu_Items;
        }
        private TreeViewItem ListHardwareMenu_ItemsTreeView(string newName, string newText, Brush colorText, Uri imageSource, MouseButtonEventHandler eventHandle)
        {
            Image imgBuf = new Image();
            if (imageSource != null)
            {
                imgBuf.Source = new BitmapImage(imageSource);
            }
            imgBuf.Width = 15;
            imgBuf.Height = 15;

            TextBlock tempText = new TextBlock();
            tempText.Text = newText;
            tempText.Background = null;
            tempText.Foreground = colorText;
            tempText.Margin = new Thickness(10, 0, 0, 0);

            StackPanel panelItem = new StackPanel();
            panelItem.Name = newName;
            if (eventHandle != null)
            {
                panelItem.MouseLeftButtonUp += eventHandle;
            }
            panelItem.Orientation = Orientation.Horizontal;
            panelItem.VerticalAlignment = VerticalAlignment.Stretch;
            if (imageSource != null)
            {
                panelItem.Children.Add(imgBuf);
            }
            panelItem.Children.Add(tempText);

            TreeViewItem itemBuf = new TreeViewItem();
            itemBuf.IsSelected = false;
            itemBuf.Foreground = colorText;
            itemBuf.IsExpanded = true;
            itemBuf.Header = panelItem;

            return itemBuf;
        }
        private void ListHardwareMenu_MainItem_DoubleClick_EventHandle(object sender, RoutedEventArgs e)
        {// Back to uiListHardware
            uiListHardware.Refresh();
            hardwareConfig_SubFrame.Content = uiListHardware;
            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
        }
        private void ListHardwareMenu_SubItemSetting_DoubleClick_EventHandle(object sender, RoutedEventArgs e)
        {            
            try
            {
                StackPanel point = (StackPanel)sender;
                string name = point.Name.ToString().Substring(8);
                int index = int.Parse(name);
                switch (listHardwareConfig.listHardware[index].type)
                {
                    case "Valve Stepper - PLC":
                    case "Valve On/Off & Stepper - PLC":
                    case "Valve Stepper & LED 485 - PLC":
                        {
                            uiSetting_ValveStepPLC.UpdateHardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiSetting_ValveStepPLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "Pump A/D & Power - PLC":
                        {
                            uiSetting_PumpADPLC.UpdateHardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiSetting_PumpADPLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    default:
                        {
                            hardwareConfig_SubFrame.Content = ui_Unknown;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                }
            }
            catch(Exception)
            {

            }

        }
        private void ListHardwareMenu_SubItemGroup_DoubleClick_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                StackPanel point = (StackPanel)sender;
                string name = point.Name.ToString().Substring(5);
                int index = int.Parse(name);
                switch (listHardwareConfig.listHardware[index].type)
                {
                    case "Valve Stepper - PLC":
                        {
                            uiGroup_ValveStepPLC.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_ValveStepPLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "Valve On/Off - PLC":
                        {
                            uiGroup_ValveOnOffPLC.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_ValveOnOffPLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "LED DMX - Artnet":
                        {
                            uiGroup_LEDDMX_Artnet.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_LEDDMX_Artnet;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "LED 485 - PLC":
                        {
                            uiGroup_LED485_PLC.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_LED485_PLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "Pump A/D & Power - PLC":
                        {
                            uiGroup_PumpADAndPowerPLC.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_PumpADAndPowerPLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "Valve On/Off & Stepper - PLC":
                        {
                            uiGroup_ValveOnOffAndStepPLC.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_ValveOnOffAndStepPLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    case "Valve Stepper & LED 485 - PLC":
                        {
                            uiGroup_ValveStepAndLED485PLC.Update_HardwareConfig(listHardwareConfig.listHardware[index]);
                            hardwareConfig_SubFrame.Content = uiGroup_ValveStepAndLED485PLC;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                    default:
                        {
                            hardwareConfig_SubFrame.Content = ui_Unknown;
                            hardwareConfig_SubFrame.NavigationService.RemoveBackEntry();
                        }
                        break;
                }

            }
            catch (Exception)
            {

            }

        }
    }
}
