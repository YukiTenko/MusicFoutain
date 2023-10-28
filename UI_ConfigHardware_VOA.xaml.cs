using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicFountain
{
    public class UISupport_ConfigPLC_VOA
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public int groupID { get; set; }
        public bool enable { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }

        public Label devIDLabel;
        public CheckBox voaEnableCheckBox;
        public TextBox voaNameTextBox;
        public TextBox opcAddressTextBox;
        public Button onTestButton;
        public Button offTestButton;
    }
    /// <summary>
    /// Interaction logic for UI_ConfigHardware_VO_PLC.xaml
    /// </summary>
    public partial class UI_ConfigHardware_VO_PLC : Page
    {
        // Variables
        private Data_Config_Hardware_Config plcConfig;
        private List<UISupport_ConfigPLC_VOA> listVOA;

        // UI
        private Label plcNameLabel;
        private Button backButton;
        private Button saveButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<Data_Config_Hardware_Config> saveClick;
        public event EventHandler<Data_Config_Device_VOA> onOffTestClick;

        public UI_ConfigHardware_VO_PLC(string xmlFilePath)
        {
            InitializeComponent();

            {// Variable creat
                listVOA = new List<UISupport_ConfigPLC_VOA>();
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                plcNameLabel = LogicalTreeHelper.FindLogicalNode(rootObject, "plcName") as Label;
                if (plcNameLabel is null)
                {
                    MessageBox.Show("UI_ConfigPLC_voa: Can not find plcName Label in file " + xmlFilePath);
                }

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_ConfigPLC_voa: Can not find back Button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                saveButton = LogicalTreeHelper.FindLogicalNode(rootObject, "save") as Button;
                if (saveButton is null)
                {
                    MessageBox.Show("UI_ConfigPLC_voa: Can not find save Button in file " + xmlFilePath);
                }
                else
                {
                    saveButton.Click += new RoutedEventHandler(SaveButton_Click);
                }

                listVOA.Clear();
                for (int i = 1; i <= GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC; i++)
                {
                    UISupport_ConfigPLC_VOA voaBuf = new UISupport_ConfigPLC_VOA();

                    voaBuf.devIDLabel = LogicalTreeHelper.FindLogicalNode(rootObject, "devID" + i.ToString()) as Label;
                    if (voaBuf.devIDLabel is null)
                    {
                        MessageBox.Show("UI_ConfigPLC_voa: Can not find devID" + i.ToString() + " Label in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC;
                    }

                    voaBuf.voaEnableCheckBox = LogicalTreeHelper.FindLogicalNode(rootObject, "voaEnable" + i.ToString()) as CheckBox;
                    if (voaBuf.voaEnableCheckBox is null)
                    {
                        MessageBox.Show("UI_ConfigPLC_voa: Can not find voaEnable" + i.ToString() + " CheckBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC;
                    }

                    voaBuf.voaNameTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "voaName" + i.ToString()) as TextBox;
                    if (voaBuf.voaNameTextBox is null)
                    {
                        MessageBox.Show("UI_ConfigPLC_voa: Can not find voaName" + i.ToString() + " TextBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC;
                    }

                    voaBuf.opcAddressTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "opcAddress" + i.ToString()) as TextBox;
                    if (voaBuf.opcAddressTextBox is null)
                    {
                        MessageBox.Show("UI_ConfigPLC_voa: Can not find opcAddress" + i.ToString() + " TextBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC;
                    }

                    voaBuf.onTestButton = LogicalTreeHelper.FindLogicalNode(rootObject, "onTest" + i.ToString()) as Button;
                    if (voaBuf.onTestButton is null)
                    {
                        MessageBox.Show("UI_ConfigPLC_voa: Can not find onTest" + i.ToString() + " Button in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC;
                    }
                    else
                    {
                        voaBuf.onTestButton.Click += OnTestButton_Click;
                    }

                    voaBuf.offTestButton = LogicalTreeHelper.FindLogicalNode(rootObject, "offTest" + i.ToString()) as Button;
                    if (voaBuf.offTestButton is null)
                    {
                        MessageBox.Show("UI_ConfigPLC_voa: Can not find offTest" + i.ToString() + " Button in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VOA_IN_A_PLC;
                    }
                    else
                    {
                        voaBuf.offTestButton.Click += OffTestButton_Click;
                    }

                    listVOA.Add(voaBuf);
                }

                mainLayout.Children.Add(rootObject);
            }
            catch
            {
                MessageBox.Show("UI_Running: Can not load file " + xmlFilePath);
            }
        }
        // Private methods -------------------------------------------------------------------------------
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            backClick.Invoke(this, 0);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Data_Config_Hardware_Config plcConfigBuf = new Data_Config_Hardware_Config();
            plcConfigBuf.listVOA = new List<Data_Config_Device_VOA>();
            plcConfigBuf.listVOA.Clear();
            for (int i = 0; i < listVOA.Count; i++)
            {
                Data_Config_Device_VOA valveConfig = new Data_Config_Device_VOA();
                plcConfigBuf.hardwareID = plcConfig.hardwareID;
                valveConfig.hardwareID = listVOA[i].hardwareID;
                valveConfig.type = listVOA[i].type;
                valveConfig.devID = int.Parse(listVOA[i].devIDLabel.Content.ToString());
                valveConfig.enable = listVOA[i].voaEnableCheckBox.IsEnabled;
                valveConfig.name = listVOA[i].voaNameTextBox.Text;
                valveConfig.opcAddress = listVOA[i].opcAddressTextBox.Text;
                plcConfigBuf.listVOA.Add(valveConfig);
            }
            saveClick.Invoke(this, plcConfigBuf);
        }
        private void OnTestButton_Click(object sender, RoutedEventArgs e)
        {
            bool senderCheck = false;
            for (int i = 0; i < listVOA.Count; i++)
            {
                if (listVOA[i].onTestButton == sender)
                {
                    senderCheck = true;
                    Data_Config_Device_VOA valveConfig = new Data_Config_Device_VOA();
                    valveConfig.hardwareID = listVOA[i].hardwareID;
                    valveConfig.type = listVOA[i].type;
                    valveConfig.devID = int.Parse(listVOA[i].devIDLabel.Content.ToString());
                    valveConfig.enable = listVOA[i].voaEnableCheckBox.IsEnabled;
                    valveConfig.name = listVOA[i].voaNameTextBox.Text;
                    valveConfig.opcAddress = listVOA[i].opcAddressTextBox.Text;
                    valveConfig.setOnOff = true;
                    onOffTestClick.Invoke(this, valveConfig);
                    i = listVOA.Count;
                }
            }
            if (senderCheck == false)
            {
                MessageBox.Show("MoveTestButton_Click: Cannot find button in listVOA config");
            }
        }
        private void OffTestButton_Click(object sender, RoutedEventArgs e)
        {
            bool senderCheck = false;
            for (int i = 0; i < listVOA.Count; i++)
            {
                if (listVOA[i].offTestButton == sender)
                {
                    senderCheck = true;
                    Data_Config_Device_VOA valveConfig = new Data_Config_Device_VOA();
                    valveConfig.hardwareID = listVOA[i].hardwareID;
                    valveConfig.type = listVOA[i].type;
                    valveConfig.devID = int.Parse(listVOA[i].devIDLabel.Content.ToString());
                    valveConfig.enable = listVOA[i].voaEnableCheckBox.IsEnabled;
                    valveConfig.name = listVOA[i].voaNameTextBox.Text;
                    valveConfig.opcAddress = listVOA[i].opcAddressTextBox.Text;
                    valveConfig.setOnOff = false;
                    onOffTestClick.Invoke(this, valveConfig);
                    i = listVOA.Count;
                }
            }
            if (senderCheck == false)
            {
                MessageBox.Show("MoveTestButton_Click: Cannot find button in listVOA config");
            }
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
        public void Update_PLCInfo(Data_Config_Hardware_Config newPLC)
        {
            plcConfig = newPLC;
            plcNameLabel.Content = "Cấu hình " + newPLC.name;
            int maxNumber = newPLC.listVOA.Count;
            if (maxNumber > listVOA.Count)
            {
                MessageBox.Show("Số lượng van trong file cấu hình nhiều hơn số van tối đa .");
                maxNumber = listVOA.Count;
            }
            for (int i = 0; i < maxNumber; i++)
            {
                listVOA[i].hardwareID = newPLC.listVOA[i].hardwareID;
                listVOA[i].type = newPLC.listVOA[i].type;

                listVOA[i].devID = newPLC.listVOA[i].devID;
                listVOA[i].devIDLabel.Content = listVOA[i].devID.ToString();

                listVOA[i].enable = newPLC.listVOA[i].enable;
                listVOA[i].voaEnableCheckBox.IsChecked = listVOA[i].enable;

                listVOA[i].name = newPLC.listVOA[i].name;
                listVOA[i].voaNameTextBox.Text = listVOA[i].name;

                listVOA[i].opcAddress = newPLC.listVOA[i].opcAddress;
                listVOA[i].opcAddressTextBox.Text = listVOA[i].opcAddress;
            }
        }
    }
}
