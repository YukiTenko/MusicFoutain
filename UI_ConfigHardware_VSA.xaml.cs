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
    public class UISupport_ConfigPLC_VSA
    {
        public int hardwareID { get; set; }
        public string type { get; set; }
        public int devID { get; set; }
        public int groupID { get; set; }
        public bool enable { get; set; }
        public string name { get; set; }
        public string opcAddress { get; set; }
        public float setSpeed { get; set; }
        public float setAngle { get; set; }

        public Label devIDLabel;
        public CheckBox vsaEnableCheckBox;
        public TextBox vsaNameTextBox;
        public TextBox opcAddressTextBox;
        public TextBox vsaSetSpeedTextBox;
        public TextBox vsaSetAngleTextBox;
        public Button moveTestButton;
        public Button goHomeButton;
    }

    /// <summary>
    /// Interaction logic for UI_ConfigHardware_VS_PLC.xaml
    /// </summary>
    public partial class UI_ConfigHardware_VS_PLC : Page
    {
        // Variables
        private Data_Config_Hardware_Config plcConfig;
        private List<UISupport_ConfigPLC_VSA> listVSA;

        // UI
        private Label plcNameLabel;
        private Button backButton;
        private Button saveButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<Data_Config_Hardware_Config> saveClick;
        public event EventHandler<Data_Config_Device_VSA> moveTestClick;
        public event EventHandler<Data_Config_Device_VSA> goHomeClick;

        public UI_ConfigHardware_VS_PLC(string xmlFilePath)
        {
            InitializeComponent();

            {// Variable creat
                listVSA = new List<UISupport_ConfigPLC_VSA>();
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                plcNameLabel = LogicalTreeHelper.FindLogicalNode(rootObject, "plcName") as Label;
                if (plcNameLabel is null)
                {
                    MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find plcName Label in file " + xmlFilePath);
                }

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find back Button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                saveButton = LogicalTreeHelper.FindLogicalNode(rootObject, "save") as Button;
                if (saveButton is null)
                {
                    MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find save Button in file " + xmlFilePath);
                }
                else
                {
                    saveButton.Click += new RoutedEventHandler(SaveButton_Click);
                }

                listVSA.Clear();
                for (int i = 1; i <= GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC; i++)
                {
                    UISupport_ConfigPLC_VSA vsaBuf = new UISupport_ConfigPLC_VSA();

                    vsaBuf.devIDLabel = LogicalTreeHelper.FindLogicalNode(rootObject, "devID" + i.ToString()) as Label;
                    if (vsaBuf.devIDLabel is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find devID" + i.ToString() + " Label in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }

                    vsaBuf.vsaEnableCheckBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaEnable" + i.ToString()) as CheckBox;
                    if (vsaBuf.vsaEnableCheckBox is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find vsaEnable" + i.ToString() + " CheckBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }

                    vsaBuf.vsaNameTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaName" + i.ToString()) as TextBox;
                    if (vsaBuf.vsaNameTextBox is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find vsaName" + i.ToString() + " TextBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }

                    vsaBuf.opcAddressTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "opcAddress" + i.ToString()) as TextBox;
                    if (vsaBuf.opcAddressTextBox is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find opcAddress" + i.ToString() + " TextBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }

                    vsaBuf.vsaSetSpeedTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaSetSpeed" + i.ToString()) as TextBox;
                    if (vsaBuf.vsaSetSpeedTextBox is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find vsaSetAngle" + i.ToString() + " TextBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }

                    vsaBuf.vsaSetAngleTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "vsaSetAngle" + i.ToString()) as TextBox;
                    if (vsaBuf.vsaSetAngleTextBox is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find vsaSetAngle" + i.ToString() + " TextBox in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }

                    vsaBuf.moveTestButton = LogicalTreeHelper.FindLogicalNode(rootObject, "moveTest" + i.ToString()) as Button;
                    if (vsaBuf.moveTestButton is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find moveTest" + i.ToString() + " Button in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }
                    else
                    {
                        vsaBuf.moveTestButton.Click += MoveTestButton_Click;
                    }

                    vsaBuf.goHomeButton = LogicalTreeHelper.FindLogicalNode(rootObject, "goHome" + i.ToString()) as Button;
                    if (vsaBuf.moveTestButton is null)
                    {
                        MessageBox.Show("UI_ConfigHardware_VS_PLC: Can not find goHome" + i.ToString() + " Button in file " + xmlFilePath);
                        i = GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC;
                    }
                    else
                    {
                        vsaBuf.goHomeButton.Click += GoHomeButton_Click;
                    }

                    listVSA.Add(vsaBuf);
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
            plcConfigBuf.listVSA = new List<Data_Config_Device_VSA>();
            plcConfigBuf.listVSA.Clear();
            for (int i = 0; i < listVSA.Count; i++)
            {
                Data_Config_Device_VSA valveConfig = new Data_Config_Device_VSA();
                plcConfigBuf.hardwareID = plcConfig.hardwareID;
                valveConfig.hardwareID = listVSA[i].hardwareID;
                valveConfig.type = listVSA[i].type;
                valveConfig.devID = int.Parse(listVSA[i].devIDLabel.Content.ToString());
                valveConfig.enable = listVSA[i].vsaEnableCheckBox.IsEnabled;
                valveConfig.name = listVSA[i].vsaNameTextBox.Text;
                valveConfig.opcAddress = listVSA[i].opcAddressTextBox.Text;
                valveConfig.setSpeed = float.Parse(listVSA[i].vsaSetSpeedTextBox.Text);
                valveConfig.setAngle = float.Parse(listVSA[i].vsaSetAngleTextBox.Text);
                plcConfigBuf.listVSA.Add(valveConfig);
            }
            saveClick.Invoke(this, plcConfigBuf);
        }
        private void MoveTestButton_Click(object sender, RoutedEventArgs e)
        {
            bool senderCheck = false;
            for (int i = 0; i < listVSA.Count; i++)
            {
                if (listVSA[i].moveTestButton == sender)
                {
                    senderCheck = true;
                    Data_Config_Device_VSA valveConfig = new Data_Config_Device_VSA();
                    valveConfig.hardwareID = listVSA[i].hardwareID;
                    valveConfig.type = listVSA[i].type;
                    valveConfig.devID = int.Parse(listVSA[i].devIDLabel.Content.ToString());
                    valveConfig.enable = listVSA[i].vsaEnableCheckBox.IsEnabled;
                    valveConfig.name = listVSA[i].vsaNameTextBox.Text;
                    valveConfig.opcAddress = listVSA[i].opcAddressTextBox.Text;
                    valveConfig.setSpeed = float.Parse(listVSA[i].vsaSetSpeedTextBox.Text);
                    valveConfig.setAngle = float.Parse(listVSA[i].vsaSetAngleTextBox.Text);
                    moveTestClick.Invoke(this, valveConfig);
                    i = listVSA.Count;
                }
            }
            if (senderCheck == false)
            {
                MessageBox.Show("MoveTestButton_Click: Cannot find button in listVSA config");
            }
        }
        private void GoHomeButton_Click(object sender, RoutedEventArgs e)
        {
            bool senderCheck = false;
            for (int i = 0; i < listVSA.Count; i++)
            {
                if (listVSA[i].goHomeButton == sender)
                {
                    senderCheck = true;
                    Data_Config_Device_VSA valveConfig = new Data_Config_Device_VSA();
                    valveConfig.hardwareID = listVSA[i].hardwareID;
                    valveConfig.type = listVSA[i].type;
                    valveConfig.devID = int.Parse(listVSA[i].devIDLabel.Content.ToString());
                    valveConfig.enable = listVSA[i].vsaEnableCheckBox.IsEnabled;
                    valveConfig.name = listVSA[i].vsaNameTextBox.Text;
                    valveConfig.opcAddress = listVSA[i].opcAddressTextBox.Text;
                    valveConfig.setSpeed = float.Parse(listVSA[i].vsaSetSpeedTextBox.Text);
                    valveConfig.setAngle = 0;
                    goHomeClick.Invoke(this, valveConfig);
                    i = listVSA.Count;
                }
            }
            if (senderCheck == false)
            {
                MessageBox.Show("GoHomeButton_Click: Cannot find button in listVSA config");
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
            int maxNumber = newPLC.listVSA.Count;
            if (maxNumber > listVSA.Count)
            {
                MessageBox.Show("Số lượng van trong file cấu hình nhiều hơn số van tối đa .");
                maxNumber = listVSA.Count;
            }
            for (int i = 0; i < maxNumber; i++)
            {
                listVSA[i].hardwareID = newPLC.listVSA[i].hardwareID;
                listVSA[i].type = newPLC.listVSA[i].type;

                listVSA[i].devID = newPLC.listVSA[i].devID;
                listVSA[i].devIDLabel.Content = listVSA[i].devID.ToString();

                listVSA[i].enable = newPLC.listVSA[i].enable;
                listVSA[i].vsaEnableCheckBox.IsChecked = listVSA[i].enable;

                listVSA[i].name = newPLC.listVSA[i].name;
                listVSA[i].vsaNameTextBox.Text = listVSA[i].name;

                listVSA[i].opcAddress = newPLC.listVSA[i].opcAddress;
                listVSA[i].opcAddressTextBox.Text = listVSA[i].opcAddress;

                listVSA[i].setSpeed = newPLC.listVSA[i].setSpeed;
                listVSA[i].vsaSetSpeedTextBox.Text = listVSA[i].setSpeed.ToString();

                listVSA[i].setAngle = newPLC.listVSA[i].setAngle;
                listVSA[i].vsaSetAngleTextBox.Text = listVSA[i].setAngle.ToString();
            }
        }
    }
}
