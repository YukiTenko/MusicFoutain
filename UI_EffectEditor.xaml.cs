using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace MusicFountain.UI.Effect
{
    /// <summary>
    /// Interaction logic for UI_EffectEditor.xaml
    /// </summary>
    public partial class UI_EffectEditor : Page
    {
        // Support class
        private class DataGrid_Effect
        {
            public int effectID { get; set; }
            public string effectName { get; set; }
            public int hardwareID { get; set; }
            public int groupID { get; set; }
            public string groupName { get; set; }
            public string groupType { get; set; }
            public string groupEffect { get; set; }
            public string groupPara1Name { get; set; }
            public string groupPara1 { get; set; }
            public string groupPara2Name { get; set; }
            public string groupPara2 { get; set; }
            public string groupPara3Name { get; set; }
            public string groupPara3 { get; set; }
            public string groupPara4Name { get; set; }
            public string groupPara4 { get; set; }
            public string groupPara5Name { get; set; }
            public string groupPara5 { get; set; }
            public string groupPara6Name { get; set; }
            public string groupPara6 { get; set; }
        }
        private class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_Effect> listEffect;
        }

        // Variables
        // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver;
        // For backend data
        private List<string> listGroupName;
        private List<Data.Data_Config_Effect> listEffect;
        private List<Data.Data_Config_Hardware_Group> listGroup;

        // Events
        public event EventHandler<List<Data.Data_Config_Effect>> saveEffectListClick;

        public UI_EffectEditor()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listEffect = new ObservableCollection<DataGrid_Effect>();
                }
                {// For backend process
                    listGroupName = new List<string>();
                    listEffect = new List<Data.Data_Config_Effect>();
                    listGroup = new List<Data.Data_Config_Hardware_Group>();
                }
            }

            {// First time UI Update
                {// Set source for data grid first
                    listEffect_DataGrid.ItemsSource = dataGridAllDataObserver.listEffect;
                    CollectionViewSource.GetDefaultView(listEffect_DataGrid.ItemsSource).Refresh();
                }
            }
        }


        // Private methods
        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dataGridAllDataObserver.listEffect.Count; i++)
            {
                dataGridAllDataObserver.listEffect[i].effectID = i + 1;
                for (int j = 0; j < listGroup.Count; j++)
                {
                    if (dataGridAllDataObserver.listEffect[i].groupName == listGroup[j].name)
                    {
                        dataGridAllDataObserver.listEffect[i].hardwareID = listGroup[j].hardwareID;
                        dataGridAllDataObserver.listEffect[i].groupID = listGroup[j].groupID;
                        dataGridAllDataObserver.listEffect[i].groupType = listGroup[j].type;
                        if ((dataGridAllDataObserver.listEffect[i].groupEffect == null) || (dataGridAllDataObserver.listEffect[i].groupEffect == ""))
                        {
                            dataGridAllDataObserver.listEffect[i].groupEffect = "Tắt toàn bộ";
                        }
                        switch (listGroup[j].type)
                        {
                            case "Valve Stepper - PLC":
                                {
                                    dataGridAllDataObserver.listEffect[i].groupPara1Name = "Tốc độ";
                                    dataGridAllDataObserver.listEffect[i].groupPara2Name = "Tg min (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara3Name = "Tg max (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara4Name = "Góc bắt đầu";
                                    dataGridAllDataObserver.listEffect[i].groupPara5Name = "Góc kết thúc";
                                    dataGridAllDataObserver.listEffect[i].groupPara6Name = "";
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara1 == null) || (dataGridAllDataObserver.listEffect[i].groupPara1 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara1 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara2 == null) || (dataGridAllDataObserver.listEffect[i].groupPara2 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara2 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara3 == null) || (dataGridAllDataObserver.listEffect[i].groupPara3 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara3 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara4 == null) || (dataGridAllDataObserver.listEffect[i].groupPara4 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara4 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara5 == null) || (dataGridAllDataObserver.listEffect[i].groupPara5 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara5 = "0";
                                    }
                                    dataGridAllDataObserver.listEffect[i].groupPara6 = "";
                                }
                                break;
                            case "Valve On/Off - PLC":
                                {
                                    dataGridAllDataObserver.listEffect[i].groupPara1Name = "Tg tắt (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara2Name = "Tg mở (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara3Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara4Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara5Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara6Name = "";
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara1 == null) || (dataGridAllDataObserver.listEffect[i].groupPara1 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara1 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara2 == null) || (dataGridAllDataObserver.listEffect[i].groupPara2 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara2 = "0";
                                    }
                                    dataGridAllDataObserver.listEffect[i].groupPara3 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara4 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara5 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara6 = "";
                                }
                                break;
                            case "Pump Analog - PLC":
                            case "Pump Analog Dual - PLC":
                                {
                                    dataGridAllDataObserver.listEffect[i].groupPara1Name = "CS max/đặt (%)";
                                    dataGridAllDataObserver.listEffect[i].groupPara2Name = "Tg đến max (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara3Name = "Tg giữ max/tắt (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara4Name = "CS min (%)";
                                    dataGridAllDataObserver.listEffect[i].groupPara5Name = "TG đến min (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara6Name = "Tg giữ min (ms)";
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara1 == null) || (dataGridAllDataObserver.listEffect[i].groupPara1 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara1 = "0";

                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara2 == null) || (dataGridAllDataObserver.listEffect[i].groupPara2 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara2 = "0";

                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara3 == null) || (dataGridAllDataObserver.listEffect[i].groupPara3 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara3 = "0";

                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara4 == null) || (dataGridAllDataObserver.listEffect[i].groupPara4 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara4 = "0";

                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara5 == null) || (dataGridAllDataObserver.listEffect[i].groupPara5 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara5 = "0";

                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara6 == null) || (dataGridAllDataObserver.listEffect[i].groupPara6 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara6 = "0";

                                    }
                                }
                                break;
                            case "LED 485 - PLC":
                            case "LED DMX - Artnet":
                                {
                                    dataGridAllDataObserver.listEffect[i].groupPara1Name = "Tg tắt (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara2Name = "Tg bật (ms)";
                                    dataGridAllDataObserver.listEffect[i].groupPara3Name = "Red (%)";
                                    dataGridAllDataObserver.listEffect[i].groupPara4Name = "Green (%)";
                                    dataGridAllDataObserver.listEffect[i].groupPara5Name = "Blue (%)";
                                    dataGridAllDataObserver.listEffect[i].groupPara6Name = "";
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara1 == null) || (dataGridAllDataObserver.listEffect[i].groupPara1 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara1 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara2 == null) || (dataGridAllDataObserver.listEffect[i].groupPara2 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara2 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara3 == null) || (dataGridAllDataObserver.listEffect[i].groupPara3 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara3 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara4 == null) || (dataGridAllDataObserver.listEffect[i].groupPara4 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara4 = "0";
                                    }
                                    if ((dataGridAllDataObserver.listEffect[i].groupPara5 == null) || (dataGridAllDataObserver.listEffect[i].groupPara5 == ""))
                                    {
                                        dataGridAllDataObserver.listEffect[i].groupPara5 = "0";
                                    }
                                    dataGridAllDataObserver.listEffect[i].groupPara5 = "";
                                }
                                break;
                            case "Pump Digital - PLC":
                            case "System Power":
                            default:
                                {
                                    dataGridAllDataObserver.listEffect[i].groupPara1Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara2Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara3Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara4Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara5Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara6Name = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara1 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara2 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara3 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara4 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara5 = "";
                                    dataGridAllDataObserver.listEffect[i].groupPara6 = "";
                                }
                                break;
                        }
                    }
                }
            }
            CollectionViewSource.GetDefaultView(listEffect_DataGrid.ItemsSource).Refresh();
            MessageBox.Show("Cập nhập tên tham số thành công");
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            List<Data.Data_Config_Effect> listEfffectConfigBuf = new List<Data.Data_Config_Effect>();
            for (int i = 0; i < dataGridAllDataObserver.listEffect.Count; i++)
            {
                Data.Data_Config_Effect newEffectBuf = new Data.Data_Config_Effect();
                newEffectBuf.effectID = 0;
                newEffectBuf.effectName = dataGridAllDataObserver.listEffect[i].effectName;
                newEffectBuf.hardwareID = dataGridAllDataObserver.listEffect[i].hardwareID;
                newEffectBuf.groupID = dataGridAllDataObserver.listEffect[i].groupID;
                newEffectBuf.groupName = dataGridAllDataObserver.listEffect[i].groupName;
                newEffectBuf.groupType = dataGridAllDataObserver.listEffect[i].groupType;
                newEffectBuf.groupEffect = dataGridAllDataObserver.listEffect[i].groupEffect;
                newEffectBuf.para1Name = dataGridAllDataObserver.listEffect[i].groupPara1Name;
                newEffectBuf.para1 = dataGridAllDataObserver.listEffect[i].groupPara1;
                newEffectBuf.para2Name = dataGridAllDataObserver.listEffect[i].groupPara2Name;
                newEffectBuf.para2 = dataGridAllDataObserver.listEffect[i].groupPara2;
                newEffectBuf.para3Name = dataGridAllDataObserver.listEffect[i].groupPara3Name;
                newEffectBuf.para3 = dataGridAllDataObserver.listEffect[i].groupPara3;
                newEffectBuf.para4Name = dataGridAllDataObserver.listEffect[i].groupPara4Name;
                newEffectBuf.para4 = dataGridAllDataObserver.listEffect[i].groupPara4;
                newEffectBuf.para5Name = dataGridAllDataObserver.listEffect[i].groupPara5Name;
                newEffectBuf.para5 = dataGridAllDataObserver.listEffect[i].groupPara5;
                newEffectBuf.para6Name = dataGridAllDataObserver.listEffect[i].groupPara6Name;
                newEffectBuf.para6 = dataGridAllDataObserver.listEffect[i].groupPara6;
                for (int j = 0; j < listGroup.Count; j++)
                {
                    if (newEffectBuf.groupName == listGroup[j].name)
                    {
                        newEffectBuf.listInverter = listGroup[j].listInverter;
                        newEffectBuf.listLED_ArtnetDMX = listGroup[j].listLED_ArtnetDMX;
                        newEffectBuf.listLED_PLC = listGroup[j].listLED_PLC;
                        newEffectBuf.listVO_PLC = listGroup[j].listVO_PLC;
                        newEffectBuf.listVS_PLC = listGroup[j].listVS_PLC;
                        j = listGroup.Count;
                    }
                }
                listEfffectConfigBuf.Add(newEffectBuf);
            }
            saveEffectListClick.Invoke(this, listEfffectConfigBuf);
        }



        // Public methods
        public void UpdateListEffectConfig(List<Data.Data_Config_Effect> newListEffectConfig)
        {
            listEffect = newListEffectConfig;
            dataGridAllDataObserver.listEffect.Clear();
            for (int i = 0; i < listEffect.Count; i++)
            {
                DataGrid_Effect newDatagridEffectBuf = new DataGrid_Effect();
                newDatagridEffectBuf.effectID = listEffect[i].effectID;
                newDatagridEffectBuf.effectName = listEffect[i].effectName;
                newDatagridEffectBuf.hardwareID = listEffect[i].hardwareID;
                newDatagridEffectBuf.groupID = listEffect[i].groupID;
                newDatagridEffectBuf.groupName = listEffect[i].groupName;
                newDatagridEffectBuf.groupType = listEffect[i].groupType;
                newDatagridEffectBuf.groupEffect = listEffect[i].groupEffect;
                newDatagridEffectBuf.groupPara1Name = listEffect[i].para1Name;
                newDatagridEffectBuf.groupPara1 = listEffect[i].para1;
                newDatagridEffectBuf.groupPara2Name = listEffect[i].para2Name;
                newDatagridEffectBuf.groupPara2 = listEffect[i].para2;
                newDatagridEffectBuf.groupPara3Name = listEffect[i].para3Name;
                newDatagridEffectBuf.groupPara3 = listEffect[i].para3;
                newDatagridEffectBuf.groupPara4Name = listEffect[i].para4Name;
                newDatagridEffectBuf.groupPara4 = listEffect[i].para4;
                newDatagridEffectBuf.groupPara5Name = listEffect[i].para5Name;
                newDatagridEffectBuf.groupPara5 = listEffect[i].para5;
                newDatagridEffectBuf.groupPara6Name = listEffect[i].para6Name;
                newDatagridEffectBuf.groupPara6 = listEffect[i].para6;
                dataGridAllDataObserver.listEffect.Add(newDatagridEffectBuf);
            }
            CollectionViewSource.GetDefaultView(listEffect_DataGrid.ItemsSource).Refresh();
        }
        public void UpdateListGroupConfig(List<Data.Data_Config_Hardware_Group> newListGroupConfig)
        {
            listGroup = newListGroupConfig;
            listGroupName.Clear();
            for (int i = 0; i < newListGroupConfig.Count; i++)
            {
                listGroup = newListGroupConfig;
                listGroupName.Add(newListGroupConfig[i].name);
            }
            groupName_ComboBox.ItemsSource = listGroupName;
        }


        public void ApplicationExit()
        {
        }

    }
}
