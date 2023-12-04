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
    /// Interaction logic for UI_Group_ValveStep_PLC.xaml
    /// </summary>
    public partial class UI_Group_ValveStep_PLC : Page
    {
        public class GroupStepPlc
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string startDev { get; set; }
            public string endDev { get; set; }
            public string Command { get; set; }
            public int Speed { get; set; }
            public int DelayT { get; set; }
            public int HoldT { get; set; }
            public int PosStart { get; set; }
            public int PosEnd { get; set; }

        }
        public List<string> _VavleStepPlc_item = new List<string>() { "Valve 1", "Valve 2", "Valve 3", "Valve 4" };
        public List<string> _CommandStepPlc_item = new List<string>() { "Tắt toàn bộ", "Về gốc tọa độ", "Vẫy đồng thời", "Vẫy đồng thời - xen kẽ 1 van", "Vẫy đồng thời - xen kẽ 2 van", "Hai nửa đối xứng"};
        public class vw_GroupStepPlc
        {

            public ObservableCollection<GroupStepPlc> ListGroupValve;
        }

        public vw_GroupStepPlc vmGroupStepPlc = new vw_GroupStepPlc();

        public event EventHandler<Data.Data_Config_Hardware_Group> testClickEvent;

        public event EventHandler<Data.Data_Config_Hardware> updateClickEvent;

        private  Data.Data_Config_Hardware hardwareConfig;

        public UI_Group_ValveStep_PLC()
        {
            InitializeComponent();
            startDev_ComboBox.ItemsSource = _VavleStepPlc_item;
            endDev_ComboBox.ItemsSource= _VavleStepPlc_item;
            effect_ComboBox.ItemsSource = _CommandStepPlc_item;

            vmGroupStepPlc.ListGroupValve = new ObservableCollection<GroupStepPlc>();
            listGroup_DataGrid.ItemsSource = vmGroupStepPlc.ListGroupValve;

            CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
        }
        public void Update_HardwareConfig(Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            vmGroupStepPlc.ListGroupValve.Clear();
            for (int i = 0; i < hardwareConfig.listGroup.Count; i++)
            {
                GroupStepPlc buffer1 = new GroupStepPlc();
                buffer1.id = hardwareConfig.listGroup[i].groupID;
                buffer1.Name = hardwareConfig.listGroup[i].name;
                buffer1.Command = hardwareConfig.listGroup[i].effectRunning;
                buffer1.startDev = "Valve " + hardwareConfig.listGroup[i].startDevID.ToString();
                buffer1.endDev = "Valve " + hardwareConfig.listGroup[i].endDevID.ToString();
                vmGroupStepPlc.ListGroupValve.Add(buffer1);
            }
        }
        private void GroupTest_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listGroup_DataGrid.SelectedIndex;
                //MessageBox.Show(index.ToString());
                if (vmGroupStepPlc.ListGroupValve[index].id == hardwareConfig.listGroup[index].groupID)
                {
                    hardwareConfig.listGroup[index].effectRunning = vmGroupStepPlc.ListGroupValve[index].Command;
                    hardwareConfig.listGroup[index].timeHoldOn = vmGroupStepPlc.ListGroupValve[index].DelayT;
                    hardwareConfig.listGroup[index].timeHoldOff = vmGroupStepPlc.ListGroupValve[index].HoldT;
                    hardwareConfig.listGroup[index].speed = vmGroupStepPlc.ListGroupValve[index].Speed;
                    hardwareConfig.listGroup[index].startPosition = vmGroupStepPlc.ListGroupValve[index].PosStart;
                    hardwareConfig.listGroup[index].endPosition = vmGroupStepPlc.ListGroupValve[index].PosEnd;
                    testClickEvent.Invoke(this, hardwareConfig.listGroup[index]);
                }
            }
            catch
            {
                MessageBox.Show("Vui lòng lưu thông tin cấu hình nhóm trước khi gửi");
            }
        }
        private void GroupSave_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            if (updateClickEvent != null)
            {
                 Data.Data_Config_Hardware newHardwareGroupInfo = new  Data.Data_Config_Hardware();
                newHardwareGroupInfo.hardwareID = hardwareConfig.hardwareID;
                newHardwareGroupInfo.type = hardwareConfig.type;
                newHardwareGroupInfo.listVS_PLC = hardwareConfig.listVS_PLC;
                newHardwareGroupInfo.optionVS_PLC = hardwareConfig.optionVS_PLC;
                newHardwareGroupInfo.optionVS_PLC.coordAngle = 0;
                newHardwareGroupInfo.optionVS_PLC.maxAngle = 180;
                newHardwareGroupInfo.optionVS_PLC.maxSpeed = 150;
                newHardwareGroupInfo.optionVS_PLC.ratio = 2;
                newHardwareGroupInfo.optionVS_PLC.coordAngle = 0;
                newHardwareGroupInfo.optionVS_PLC.coordAngle = 0;
                newHardwareGroupInfo.listGroup = new List<Data.Data_Config_Hardware_Group>();
                newHardwareGroupInfo.listGroup.Clear();
                for (int i = 0; i < vmGroupStepPlc.ListGroupValve.Count; i++)
                {
                     Data.Data_Config_Hardware_Group newGroupBuf = new  Data.Data_Config_Hardware_Group();
                    newGroupBuf.hardwareID = newHardwareGroupInfo.hardwareID;
                    newGroupBuf.groupID = vmGroupStepPlc.ListGroupValve[i].id;
                    newGroupBuf.name = vmGroupStepPlc.ListGroupValve[i].Name;
                    newGroupBuf.type = newHardwareGroupInfo.type;
                    newGroupBuf.startDevID = int.Parse(vmGroupStepPlc.ListGroupValve[i].startDev.Replace("Valve ", ""));
                    newGroupBuf.endDevID = int.Parse(vmGroupStepPlc.ListGroupValve[i].endDev.Replace("Valve ", ""));
                    newGroupBuf.effectSave = vmGroupStepPlc.ListGroupValve[i].Command;
                    newGroupBuf.timeHoldOn = vmGroupStepPlc.ListGroupValve[i].DelayT;
                    newGroupBuf.timeHoldOff = vmGroupStepPlc.ListGroupValve[i].HoldT;
                    newGroupBuf.speed = vmGroupStepPlc.ListGroupValve[i].Speed;
                    newGroupBuf.startPosition = vmGroupStepPlc.ListGroupValve[i].PosStart;
                    newGroupBuf.endPosition = vmGroupStepPlc.ListGroupValve[i].PosEnd;
                    newHardwareGroupInfo.listGroup.Add(newGroupBuf);
                }
                updateClickEvent.Invoke(this, newHardwareGroupInfo);
            }
        }
    }
}
