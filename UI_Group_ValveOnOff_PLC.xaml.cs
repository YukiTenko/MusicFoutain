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

namespace MusicFountain.UI.Hardware.Group
{
    /// <summary>
    /// Interaction logic for UI_GroupValve.xaml
    /// </summary>

    public partial class UI_Group_ValveOnOff_PLC : Page
    {
        public class GroupValvePlc
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string startDev { get; set; }
            public string endDev { get; set; }
            public string Command { get; set; }
            public int TimeDelay { get; set; }
            public int TimeHold { get; set; }

        }

        public List<string> _ValvePlc_item = new List<string>();

        public List<string> _CommandValvePlc_item = new List<string>() { "Tắt toàn bộ", "Mở toàn bộ", "Mở rồi tắt - lặp lại", "Bật/tắt xen kẽ", "Bật/tắt xen kẽ 2", "Chạy lượn sóng Trái-Phải - Lặp lại"};
        public class vw_GroupValvePlc
        {
            public ObservableCollection<GroupValvePlc> ListGroupValvePlc { get; set; }
        }

        public vw_GroupValvePlc vmGroupValvePlc = new vw_GroupValvePlc();

        public event EventHandler<Data.Data_Config_Hardware_Group> testClickEvent;

        public event EventHandler<Data.Data_Config_Hardware> updateClickEvent;

        private  Data.Data_Config_Hardware hardwareConfig; 

        public UI_Group_ValveOnOff_PLC()
        {
            InitializeComponent();

            for (int i = 0; i < 16; i++)
            {
                string Valve = "Valve " + i.ToString();
                _ValvePlc_item.Add(Valve);
            }

            devStart_ComboBox.ItemsSource = _ValvePlc_item;
            devEnd_ComboBox.ItemsSource = _ValvePlc_item;
            groupEffect_ComboBox.ItemsSource = _CommandValvePlc_item;

            vmGroupValvePlc.ListGroupValvePlc = new ObservableCollection<GroupValvePlc>();
            //GroupValvePlc step1 = new GroupValvePlc();
            ////step1.id = 1;
            //step1.endDev = "Valve 2";
            //step1.startDev = "Valve 3";
            //vmGroupValvePlc.ListGroupValvePlc.Add(step1);

            mainLayout.DataContext = vmGroupValvePlc;
            listGroup_DataGrid.ItemsSource = vmGroupValvePlc.ListGroupValvePlc;

            CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
        }

        private void Button_VALVEPLC_Handler(object sender, RoutedEventArgs e)
        {
            int index = listGroup_DataGrid.SelectedIndex;
            //MessageBox.Show(index.ToString());
            if (vmGroupValvePlc.ListGroupValvePlc[index].id == hardwareConfig.listGroup[index].groupID)
            {
                hardwareConfig.listGroup[index].effectRunning = vmGroupValvePlc.ListGroupValvePlc[index].Command;
                hardwareConfig.listGroup[index].timeHoldOn = vmGroupValvePlc.ListGroupValvePlc[index].TimeDelay;
                hardwareConfig.listGroup[index].timeHoldOff = vmGroupValvePlc.ListGroupValvePlc[index].TimeHold;
                testClickEvent.Invoke(this, hardwareConfig.listGroup[index]);
            }
        }
        private void Button_update_groupValvePlc(object sender, RoutedEventArgs e)
        {
            if (updateClickEvent != null)
            {
                 Data.Data_Config_Hardware newHardwareGroupInfo = new  Data.Data_Config_Hardware();
                newHardwareGroupInfo.hardwareID = hardwareConfig.hardwareID;
                newHardwareGroupInfo.type = hardwareConfig.type;
                newHardwareGroupInfo.listVO_PLC = hardwareConfig.listVO_PLC;
                newHardwareGroupInfo.optionVO_PLC = hardwareConfig.optionVO_PLC;
                newHardwareGroupInfo.listGroup = new List<Data.Data_Config_Hardware_Group>();
                newHardwareGroupInfo.listGroup.Clear();
                for (int i = 0; i < vmGroupValvePlc.ListGroupValvePlc.Count; i++)
                {
                     Data.Data_Config_Hardware_Group newGroupBuf = new  Data.Data_Config_Hardware_Group();
                    newGroupBuf.groupID = vmGroupValvePlc.ListGroupValvePlc[i].id;
                    newGroupBuf.name = vmGroupValvePlc.ListGroupValvePlc[i].Name;
                    newGroupBuf.type = newHardwareGroupInfo.type;
                    newGroupBuf.startDevID = int.Parse(vmGroupValvePlc.ListGroupValvePlc[i].startDev.Replace("Valve ", ""));
                    newGroupBuf.endDevID = int.Parse(vmGroupValvePlc.ListGroupValvePlc[i].endDev.Replace("Valve ", ""));
                    newGroupBuf.effectSave = vmGroupValvePlc.ListGroupValvePlc[i].Command;
                    newGroupBuf.timeHoldOn = vmGroupValvePlc.ListGroupValvePlc[i].TimeDelay;
                    newGroupBuf.timeHoldOff = vmGroupValvePlc.ListGroupValvePlc[i].TimeHold;
                    newHardwareGroupInfo.listGroup.Add(newGroupBuf);
                }
                updateClickEvent.Invoke(this, newHardwareGroupInfo);
            }
        }

        private void DataGrid_Event_RowEditEndig(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (listGroup_DataGrid.SelectedItem != null)
            {

                listGroup_DataGrid.RowEditEnding -= DataGrid_Event_RowEditEndig;
                listGroup_DataGrid.CommitEdit();
                listGroup_DataGrid.Items.Refresh();
                listGroup_DataGrid.RowEditEnding += DataGrid_Event_RowEditEndig;

                //for (int i = 0; i < vmGroupValvePlc.ListGroupValvePlc.Count; i++)
                //{
                //    vmGroupValvePlc.ListGroupValvePlc[i].id = i + 1;
                //}
                CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
            }
        }
        public void Update_HardwareConfig( Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            vmGroupValvePlc.ListGroupValvePlc.Clear();
            for (int i = 0; i < hardwareConfig.listGroup.Count; i++)
            {
                GroupValvePlc buffer1 = new GroupValvePlc();
                buffer1.id = hardwareConfig.listGroup[i].groupID;
                buffer1.Name = hardwareConfig.listGroup[i].name;
                buffer1.Command = hardwareConfig.listGroup[i].effectRunning;
                buffer1.TimeDelay = (int)hardwareConfig.listGroup[i].timeHoldOn;
                buffer1.TimeHold = (int)hardwareConfig.listGroup[i].timeHoldOff;
                buffer1.startDev = "Valve " + hardwareConfig.listGroup[i].startDevID.ToString();
                buffer1.endDev = "Valve " + hardwareConfig.listGroup[i].endDevID.ToString();
                vmGroupValvePlc.ListGroupValvePlc.Add(buffer1);
            }
        }

    }
}
