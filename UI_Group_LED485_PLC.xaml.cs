using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
    /// Interaction logic for UI_Group_LED485_PLC.xaml
    /// </summary>
    public partial class UI_Group_LED485_PLC : Page
    {
        public class GroupLed485
        {
            public int id { get; set; }

            public string Name { get; set; }
            public string startDev { get; set; }
            public string endDev { get; set; }
            public string Command { get; set; }
            public int TimeDelay { get; set; }
            public int TimeHold { get; set; }
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }

        }

        public List<string> _Led485_item = new List<string>();

        public List<string> _CommandLed485_item = new List<string>() {"Tắt toàn bộ","Mở toàn bộ","Mở rồi tắt - lặp lại", "Nhấp nháy - xen kẽ 1","Nhấp nháy - xen kẽ 2", "Sáng dần - phải", "Sáng dần - trái","Tắt Trái-Phải, Trên-Dưới, Trong-Ngoài, Trước-Sau - Lặp lại","Tắt Phải-Trái, Dưới-Trên, Ngoài-Trong, Sau-Trước - Lặp lại"};
        public class vw_GroupLed485
        {

            public ObservableCollection<GroupLed485> ListGroupLed { get; set; }
        }

        public vw_GroupLed485 vmGroupLed485 = new vw_GroupLed485();

        private  Data.Data_Config_Hardware hardwareConfig;

        public event EventHandler<Data.Data_Config_Hardware_Group> testClickEvent;

        public event EventHandler<Data.Data_Config_Hardware> updateClickEvent;

        public UI_Group_LED485_PLC()
        {
            InitializeComponent();

            for (int i = 1; i <= 500; i++)
            {
                string Led = "Led " + i;
                _Led485_item.Add(Led);
            }

            Xaml_GroupLed485Start.ItemsSource = _Led485_item;
            Xaml_GroupLed485End.ItemsSource = _Led485_item;
            Xaml_GroupLed485Command.ItemsSource = _CommandLed485_item;

            vmGroupLed485.ListGroupLed = new ObservableCollection<GroupLed485>();

            //vmGroupLed485.ListGroupLed.Add(step1);

            Xaml_GroupLed485Page.DataContext = vmGroupLed485;
            Xaml_GroupLed485DataGrid.ItemsSource = vmGroupLed485.ListGroupLed;

            CollectionViewSource.GetDefaultView(Xaml_GroupLed485DataGrid.ItemsSource).Refresh();
        }
        private void GroupTest_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            int index = Xaml_GroupLed485DataGrid.SelectedIndex;
            //MessageBox.Show(index.ToString());
            if (vmGroupLed485.ListGroupLed[index].id == hardwareConfig.listGroup[index].groupID)
            {
                hardwareConfig.listGroup[index].effectRunning = vmGroupLed485.ListGroupLed[index].Command;
                hardwareConfig.listGroup[index].timeHoldOn = vmGroupLed485.ListGroupLed[index].TimeDelay;
                hardwareConfig.listGroup[index].timeHoldOff = vmGroupLed485.ListGroupLed[index].TimeHold;
                hardwareConfig.listGroup[index].red = vmGroupLed485.ListGroupLed[index].Red;
                hardwareConfig.listGroup[index].green = vmGroupLed485.ListGroupLed[index].Green;
                hardwareConfig.listGroup[index].blue = vmGroupLed485.ListGroupLed[index].Blue;
                testClickEvent.Invoke(this, hardwareConfig.listGroup[index]);
            }
        }
        private void GroupSave_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            if (updateClickEvent != null)
            {
                 Data.Data_Config_Hardware newHardwareGroupInfo = new  Data.Data_Config_Hardware();
                newHardwareGroupInfo.hardwareID = hardwareConfig.hardwareID;
                newHardwareGroupInfo.type = hardwareConfig.type;
                newHardwareGroupInfo.listLED_PLC = hardwareConfig.listLED_PLC;
                newHardwareGroupInfo.optionLED_PLC = hardwareConfig.optionLED_PLC;
                newHardwareGroupInfo.listGroup = new List<Data.Data_Config_Hardware_Group>();
                newHardwareGroupInfo.listGroup.Clear();
                for (int i = 0; i < vmGroupLed485.ListGroupLed.Count; i++)
                {
                     Data.Data_Config_Hardware_Group newGroupBuf = new  Data.Data_Config_Hardware_Group();
                    newGroupBuf.hardwareID = newHardwareGroupInfo.hardwareID;
                    newGroupBuf.groupID = vmGroupLed485.ListGroupLed[i].id;
                    newGroupBuf.name = vmGroupLed485.ListGroupLed[i].Name;
                    newGroupBuf.type = newHardwareGroupInfo.type;
                    newGroupBuf.startDevID = int.Parse(vmGroupLed485.ListGroupLed[i].startDev.Replace("Led ", ""));
                    newGroupBuf.endDevID = int.Parse(vmGroupLed485.ListGroupLed[i].endDev.Replace("Led ", ""));
                    newGroupBuf.effectSave = vmGroupLed485.ListGroupLed[i].Command;
                    newGroupBuf.timeHoldOn = vmGroupLed485.ListGroupLed[i].TimeDelay;
                    newGroupBuf.timeHoldOff = vmGroupLed485.ListGroupLed[i].TimeDelay;
                    newGroupBuf.red = vmGroupLed485.ListGroupLed[i].Red;
                    newGroupBuf.green = vmGroupLed485.ListGroupLed[i].Green;
                    newGroupBuf.blue = vmGroupLed485.ListGroupLed[i].Blue;
                    newHardwareGroupInfo.listGroup.Add(newGroupBuf);
                }
                updateClickEvent.Invoke(this, newHardwareGroupInfo);
            }
        }
        private void DataGrid_Event_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (Xaml_GroupLed485DataGrid.SelectedItem != null)
            {

                Xaml_GroupLed485DataGrid.RowEditEnding -= DataGrid_Event_RowEditEnding;
                Xaml_GroupLed485DataGrid.CommitEdit();
                Xaml_GroupLed485DataGrid.Items.Refresh();
                Xaml_GroupLed485DataGrid.RowEditEnding += DataGrid_Event_RowEditEnding;

                //for (int i = 0; i < vmGroupLed485.ListGroupLed.Count; i++)
                //{
                //    vmGroupLed485.ListGroupLed[i].id = i + 1;
                //}
                CollectionViewSource.GetDefaultView(Xaml_GroupLed485DataGrid.ItemsSource).Refresh();
            }
        }
        public void Update_HardwareConfig( Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            vmGroupLed485.ListGroupLed.Clear();
            for (int i = 0; i < hardwareConfig.listGroup.Count; i++)
            {
                GroupLed485 buffer1 = new GroupLed485();
                buffer1.id = hardwareConfig.listGroup[i].groupID;
                buffer1.Name = hardwareConfig.listGroup[i].name;
                buffer1.Command = hardwareConfig.listGroup[i].effectRunning;
                buffer1.startDev = "Led " + hardwareConfig.listGroup[i].startDevID.ToString();
                buffer1.endDev = "Led " + hardwareConfig.listGroup[i].endDevID.ToString();
                buffer1.Command = hardwareConfig.listGroup[i].effectRunning;
                buffer1.TimeDelay = (int)hardwareConfig.listGroup[i].timeHoldOn;
                buffer1.TimeHold = (int)hardwareConfig.listGroup[i].timeHoldOff;
                buffer1.Red = hardwareConfig.listGroup[i].red;
                buffer1.Green = hardwareConfig.listGroup[i].green;
                buffer1.Blue = hardwareConfig.listGroup[i].blue;
                vmGroupLed485.ListGroupLed.Add(buffer1);
            }
        }
    }
}
