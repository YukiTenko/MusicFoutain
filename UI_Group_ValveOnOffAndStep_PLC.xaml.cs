using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Shapes;

namespace MusicFountain.UI.Hardware.Group
{
    public partial class UI_Group_ValveOnOffAndStep_PLC : Page
    {
        // Support class and constants
        public class DataGrid_GroupConfig
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string startDev { get; set; }
            public string endDev { get; set; }
            public string effect { get; set; }
            public float timeHoldOff { get; set; }
            public float timeHoldOn { get; set; }
            public float speed { get; set; }
            public float startPosition { get; set; }
            public float endPosition { get; set; }

        }
        public class DataGrid_AllData_Observer
        {
            public ObservableCollection<DataGrid_GroupConfig> listGroup;
        }

        private List<string> LIST_GROUP_TYPE = new List<string>() { "Valve On/Off - PLC", "Valve Stepper - PLC" };

        // Variables
            // For show UI
        private DataGrid_AllData_Observer dataGridAllDataObserver;
            // For backend data
        private Data.Data_Config_Hardware hardwareConfig;

        // Events
        public event EventHandler<Data.Data_Config_Hardware_Group> testClickEvent;
        public event EventHandler<Data.Data_Config_Hardware> updateClickEvent;

        public UI_Group_ValveOnOffAndStep_PLC()
        {
            InitializeComponent();

            {// Creat variables
                {// For UI data grid
                    dataGridAllDataObserver = new DataGrid_AllData_Observer();
                    dataGridAllDataObserver.listGroup = new ObservableCollection<DataGrid_GroupConfig>();
                }
                {// For backend process
                    hardwareConfig = new Data.Data_Config_Hardware();
                }
            }

            {// First time UI Update
                {// Set source for data grid first
                    groupType_ComboBox.ItemsSource = LIST_GROUP_TYPE;

                    listGroup_DataGrid.ItemsSource = dataGridAllDataObserver.listGroup;
                    CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
                }
                {// Init others
                }
            }
        }

        // Public methods
        public void Update_HardwareConfig(Data.Data_Config_Hardware newHardwareConfig)
        {
            hardwareConfig = newHardwareConfig;
            dataGridAllDataObserver.listGroup.Clear();
            for (int i = 0; i < hardwareConfig.listGroup.Count; i++)
            {
                DataGrid_GroupConfig dataGridBuffer = new DataGrid_GroupConfig();
                dataGridBuffer.id = hardwareConfig.listGroup[i].groupID;
                dataGridBuffer.name = hardwareConfig.listGroup[i].name;
                dataGridBuffer.type = hardwareConfig.listGroup[i].type;
                dataGridBuffer.effect = hardwareConfig.listGroup[i].effectSave;
                switch (dataGridBuffer.type)
                {
                    case "Valve On/Off - PLC":
                        {
                            dataGridBuffer.startDev = "Van " + hardwareConfig.listGroup[i].startDevID.ToString();
                            dataGridBuffer.endDev = "Van " + hardwareConfig.listGroup[i].endDevID.ToString();
                            dataGridBuffer.timeHoldOff = hardwareConfig.listGroup[i].timeHoldOff;
                            dataGridBuffer.timeHoldOn = hardwareConfig.listGroup[i].timeHoldOn;
                        }
                        break;
                    case "Valve Stepper - PLC":
                        {
                            dataGridBuffer.startDev = "Van " + hardwareConfig.listGroup[i].startDevID.ToString();
                            dataGridBuffer.endDev = "Van " + hardwareConfig.listGroup[i].endDevID.ToString();
                            dataGridBuffer.timeHoldOff = hardwareConfig.listGroup[i].timeHoldOff;
                            dataGridBuffer.timeHoldOn = hardwareConfig.listGroup[i].timeHoldOn;
                            dataGridBuffer.speed = hardwareConfig.listGroup[i].speed;
                            dataGridBuffer.startPosition = hardwareConfig.listGroup[i].startPosition;
                            dataGridBuffer.endPosition = hardwareConfig.listGroup[i].endPosition;
                        }
                        break;
                    default:
                        {

                        }
                        break;

                }
                dataGridAllDataObserver.listGroup.Add(dataGridBuffer);
            }
            CollectionViewSource.GetDefaultView(listGroup_DataGrid.ItemsSource).Refresh();
        }

        // Private methods
        private void GroupTest_ButtonClick_EventHandle(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = listGroup_DataGrid.SelectedIndex;
                //MessageBox.Show(index.ToString());
                if (dataGridAllDataObserver.listGroup[index].id == hardwareConfig.listGroup[index].groupID)
                {
                    hardwareConfig.listGroup[index].effectRunning = dataGridAllDataObserver.listGroup[index].effect;
                    hardwareConfig.listGroup[index].timeHoldOn = dataGridAllDataObserver.listGroup[index].timeHoldOn;
                    hardwareConfig.listGroup[index].timeHoldOff = dataGridAllDataObserver.listGroup[index].timeHoldOff;
                    hardwareConfig.listGroup[index].speed = dataGridAllDataObserver.listGroup[index].speed;
                    hardwareConfig.listGroup[index].startPosition = dataGridAllDataObserver.listGroup[index].startPosition;
                    hardwareConfig.listGroup[index].endPosition = dataGridAllDataObserver.listGroup[index].endPosition;
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
                Data.Data_Config_Hardware newHardwareGroupInfo = new Data.Data_Config_Hardware();
                newHardwareGroupInfo.hardwareID = hardwareConfig.hardwareID;
                newHardwareGroupInfo.type = hardwareConfig.type;
                newHardwareGroupInfo.listVS_PLC = hardwareConfig.listVS_PLC;
                newHardwareGroupInfo.optionVS_PLC = hardwareConfig.optionVS_PLC;
                newHardwareGroupInfo.listGroup = new List<Data.Data_Config_Hardware_Group>();
                newHardwareGroupInfo.listGroup.Clear();
                for (int i = 0; i < dataGridAllDataObserver.listGroup.Count; i++)
                {
                    Data.Data_Config_Hardware_Group newGroupBuf = new Data.Data_Config_Hardware_Group();
                    newGroupBuf.hardwareID = newHardwareGroupInfo.hardwareID;
                    newGroupBuf.groupID = dataGridAllDataObserver.listGroup[i].id;
                    newGroupBuf.name = dataGridAllDataObserver.listGroup[i].name;
                    newGroupBuf.type = dataGridAllDataObserver.listGroup[i].type;
                    newGroupBuf.effectSave = dataGridAllDataObserver.listGroup[i].effect;
                    try
                    {
                        switch (newGroupBuf.type)
                        {
                            case "Valve On/Off - PLC":
                                {
                                    newGroupBuf.startDevID = int.Parse(dataGridAllDataObserver.listGroup[i].startDev.Replace("Van ", ""));
                                    newGroupBuf.endDevID = int.Parse(dataGridAllDataObserver.listGroup[i].endDev.Replace("Van ", ""));
                                    newGroupBuf.timeHoldOn = dataGridAllDataObserver.listGroup[i].timeHoldOn;
                                    newGroupBuf.timeHoldOff = dataGridAllDataObserver.listGroup[i].timeHoldOff;
                                }
                                break;
                            case "Valve Stepper - PLC":
                                {
                                    newGroupBuf.startDevID = int.Parse(dataGridAllDataObserver.listGroup[i].startDev.Replace("Van ", ""));
                                    newGroupBuf.endDevID = int.Parse(dataGridAllDataObserver.listGroup[i].endDev.Replace("Van ", ""));
                                    newGroupBuf.timeHoldOn = dataGridAllDataObserver.listGroup[i].timeHoldOn;
                                    newGroupBuf.timeHoldOff = dataGridAllDataObserver.listGroup[i].timeHoldOff;
                                    newGroupBuf.speed = dataGridAllDataObserver.listGroup[i].speed;
                                    newGroupBuf.startPosition = dataGridAllDataObserver.listGroup[i].startPosition;
                                    newGroupBuf.endPosition = dataGridAllDataObserver.listGroup[i].endPosition;
                                }
                                break;
                            default:
                                {

                                }
                                break;

                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Vui lòng chọn danh sách thiết bị trong nhóm trước khi lưu . Hiên tại sẽ lưu danh sách thiết bị mặc định .");
                        newGroupBuf.startDevID = 1;
                        newGroupBuf.endDevID = 1;
                        newGroupBuf.timeHoldOn = dataGridAllDataObserver.listGroup[i].timeHoldOn;
                        newGroupBuf.timeHoldOff = dataGridAllDataObserver.listGroup[i].timeHoldOff;
                        newGroupBuf.speed = dataGridAllDataObserver.listGroup[i].speed;
                        newGroupBuf.startPosition = dataGridAllDataObserver.listGroup[i].startPosition;
                        newGroupBuf.endPosition = dataGridAllDataObserver.listGroup[i].endPosition;
                    }
                    newHardwareGroupInfo.listGroup.Add(newGroupBuf);
                }
                updateClickEvent.Invoke(this, newHardwareGroupInfo);
            }
        }
    }
}

