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
    public class UISupport_ConfigHardware
    {
        public int hardwareID { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string opcServerURL { get; set; }
        public int opcServerPort { get; set; }
        public Grid layoutGrid { get; set; }
        public Label pIDLabel { get; set; }
        public TextBox nameTextBox { get; set; }
        public Label typeLabel { get; set; }
        public TextBox opcServerURLTextBox { get; set; }
        public Button editButton { get; set; }
        public Button deleteButton { get; set; }
    }

    /// <summary>
    /// Interaction logic for UI_EditPlaylist.xaml
    /// </summary>
    public partial class UI_ConfigHardware : Page
    {
        // Variables
        private List<UISupport_ConfigHardware> listHardware;

        // UI
        private Button backButton;
        private Button optionButton;
        private Button saveButton;
        private Panel listHardwarePanel;
        private TextBox newName;
        private ComboBox newType;
        private TextBox newOPCUrl;
        private Button addButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<int> optionClick;
        public event EventHandler<List<Data_Config_Hardware_Config>> saveClick;
        public event EventHandler<Data_Config_Hardware_Config> addClick;
        public event EventHandler<int> editClick;
        public event EventHandler<int> deleteClick;

        public UI_ConfigHardware(string xmlFilePath)
        {
            InitializeComponent();

            {// Variables creat
                listHardware = new List<UISupport_ConfigHardware>();
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find back Button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                optionButton = LogicalTreeHelper.FindLogicalNode(rootObject, "option") as Button;
                if (optionButton is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find option Button in file " + xmlFilePath);
                }
                else
                {
                    optionButton.Click += new RoutedEventHandler(OptionButton_Click);
                }

                saveButton = LogicalTreeHelper.FindLogicalNode(rootObject, "save") as Button;
                if (saveButton is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find save Button in file " + xmlFilePath);
                }
                else
                {
                    saveButton.Click += new RoutedEventHandler(SaveButton_Click);
                }

                listHardwarePanel = LogicalTreeHelper.FindLogicalNode(rootObject, "listHardware") as StackPanel;
                if (listHardwarePanel is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find listHardware StackPanel in file " + xmlFilePath);
                }

                newName = LogicalTreeHelper.FindLogicalNode(rootObject, "newName") as TextBox;
                if (newName is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find newName TextBox in file " + xmlFilePath);
                }

                newType = LogicalTreeHelper.FindLogicalNode(rootObject, "newType") as ComboBox;
                if (newType is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find newType ComboBox in file " + xmlFilePath);
                }

                newOPCUrl = LogicalTreeHelper.FindLogicalNode(rootObject, "newOPCUrl") as TextBox;
                if (newOPCUrl is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find newOPCUrl TextBox in file " + xmlFilePath);
                }

                addButton = LogicalTreeHelper.FindLogicalNode(rootObject, "add") as Button;
                if (addButton is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find add Button in file " + xmlFilePath);
                }
                else
                {
                    addButton.Click += new RoutedEventHandler(AddButton_Click);
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
            backClick.Invoke(this, 0);
        }
        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            optionClick.Invoke(this, 0);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<Data_Config_Hardware_Config> newlistHardware = new List<Data_Config_Hardware_Config>();
            for (int i = 0; i < listHardware.Count; i++)
            {
                Data_Config_Hardware_Config newPLCConfig = new Data_Config_Hardware_Config();
                newPLCConfig.hardwareID = int.Parse(listHardware[i].pIDLabel.Content.ToString());
                newPLCConfig.name = listHardware[i].nameTextBox.Text.ToString();
                newPLCConfig.type = listHardware[i].typeLabel.Content.ToString();
                newPLCConfig.opcServerURL = listHardware[i].opcServerURLTextBox.Text.ToString();
                newlistHardware.Add(newPLCConfig);
            }
            saveClick.Invoke(this, newlistHardware);
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Data_Config_Hardware_Config newPLC = new Data_Config_Hardware_Config();
            newPLC.name = newName.Text;
            newPLC.type = newType.Text;
            newPLC.opcServerURL = newOPCUrl.Text;
            newPLC.listVSA = new List<Data_Config_Device_VSA>();
            newPLC.listVOA = new List<Data_Config_Device_VOA>();
            newPLC.listInverter = new List<Data_Config_Device_Inverter>();
            newPLC.listLED = new List<Data_Config_Device_LED>();
            switch (newPLC.type)
            {
                case "Valve Stepper - PLC":
                    {
                        for (int i = 1; i <= GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC; i++)
                        {
                            Data_Config_Device_VSA valveConfig = new Data_Config_Device_VSA();
                            valveConfig.type = newType.Text;
                            valveConfig.devID = i;
                            valveConfig.enable = false;
                            valveConfig.name = "Valve " + i.ToString();
                            valveConfig.opcAddress = "ns=4;i=" + (409 + i * 6).ToString();
                            valveConfig.setAngle = 90;
                            newPLC.listVSA.Add(valveConfig);
                        }
                    }
                    break;
                case "Valve On/Off - PLC":
                    {
                        for (int i = 1; i <= GLOBAL_CONSTANT.MAX_NUMBER_OF_VSA_IN_A_PLC; i++)
                        {
                            Data_Config_Device_VOA valveConfig = new Data_Config_Device_VOA();
                            valveConfig.type = newType.Text;
                            valveConfig.devID = i;
                            valveConfig.enable = false;
                            valveConfig.name = "Valve " + i.ToString();
                            valveConfig.opcAddress = "ns=4;i=" + (409 + i * 6).ToString();
                            newPLC.listVOA.Add(valveConfig);
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            addClick.Invoke(this, newPLC);
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listHardware.Count; i++)
            {
                if (sender == listHardware[i].editButton)
                {
                    editClick.Invoke(this, listHardware[i].hardwareID);
                    i = listHardware.Count;
                }
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listHardware.Count; i++)
            {
                if (sender == listHardware[i].deleteButton)
                {
                    deleteClick.Invoke(this, listHardware[i].hardwareID);
                    i = listHardware.Count;
                }
            }
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
        public void Update_ListHardware(List<Data_Config_Hardware_Config> newList)
        {
            // Clear list data and UI stack panel
            listHardware.Clear();
            listHardwarePanel.Children.Clear();
            // Add new list
            for (int i = 0; i < newList.Count; i++)
            {
                UISupport_ConfigHardware newConfig = new UISupport_ConfigHardware();
                newConfig.hardwareID = newList[i].hardwareID;
                newConfig.name = newList[i].name;
                newConfig.type = newList[i].type;
                newConfig.opcServerURL = newList[i].opcServerURL;
                newConfig.opcServerPort = newList[i].opcServerPort;
                newConfig.pIDLabel = new Label();
                {// GlobalID label creat
                    newConfig.pIDLabel.Content = newConfig.hardwareID.ToString();
                    //newConfig.pIDLabel.Margin = new Thickness(0, 10, 0, 0);
                    newConfig.pIDLabel.Background = new BrushConverter().ConvertFromString("#99000000") as SolidColorBrush;
                    newConfig.pIDLabel.Foreground = Brushes.GhostWhite;
                    newConfig.pIDLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newConfig.pIDLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.pIDLabel.FontSize = 20;
                    newConfig.pIDLabel.FontFamily = new FontFamily("Arial");
                    newConfig.pIDLabel.BorderBrush = Brushes.Gray;
                    newConfig.pIDLabel.BorderThickness = new Thickness(1);
                }
                newConfig.nameTextBox = new TextBox();
                {// GlobalID label creat
                    newConfig.nameTextBox.Text = newConfig.name;
                    //newConfig.nameTextBox.Margin = new Thickness(0, 10, 0, 0);
                    newConfig.nameTextBox.Background = new BrushConverter().ConvertFromString("#99000000") as SolidColorBrush;
                    newConfig.nameTextBox.Foreground = Brushes.GhostWhite;
                    newConfig.nameTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                    newConfig.nameTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.nameTextBox.FontSize = 20;
                    newConfig.nameTextBox.FontFamily = new FontFamily("Arial");
                    newConfig.nameTextBox.BorderBrush = Brushes.Gray;
                    newConfig.nameTextBox.BorderThickness = new Thickness(1);
                }
                newConfig.typeLabel = new Label();
                {// GlobalID label creat
                    newConfig.typeLabel.Content = newConfig.type.ToString();
                    //newConfig.typeLabel.Margin = new Thickness(0, 10, 0, 0);
                    newConfig.typeLabel.Background = new BrushConverter().ConvertFromString("#99000000") as SolidColorBrush;
                    newConfig.typeLabel.Foreground = Brushes.GhostWhite;
                    newConfig.typeLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
                    newConfig.typeLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.typeLabel.FontSize = 20;
                    newConfig.typeLabel.FontFamily = new FontFamily("Arial");
                    newConfig.typeLabel.BorderBrush = Brushes.Gray;
                    newConfig.typeLabel.BorderThickness = new Thickness(1);
                }
                newConfig.opcServerURLTextBox = new TextBox();
                {// GlobalID label creat
                    newConfig.opcServerURLTextBox.Text = newConfig.opcServerURL;
                    //newConfig.opcServerURLTextBox.Margin = new Thickness(0, 10, 0, 0);
                    newConfig.opcServerURLTextBox.Background = new BrushConverter().ConvertFromString("#99000000") as SolidColorBrush;
                    newConfig.opcServerURLTextBox.Foreground = Brushes.GhostWhite;
                    newConfig.opcServerURLTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                    newConfig.opcServerURLTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.opcServerURLTextBox.FontSize = 20;
                    newConfig.opcServerURLTextBox.FontFamily = new FontFamily("Arial");
                    newConfig.opcServerURLTextBox.BorderBrush = Brushes.Gray;
                    newConfig.opcServerURLTextBox.BorderThickness = new Thickness(1);
                }
                newConfig.editButton = new Button();
                {// Edit button creat
                    newConfig.editButton.Content = "Sửa";
                    newConfig.editButton.Margin = new Thickness(5, 5, 5, 5);
                    newConfig.editButton.Background = Brushes.DarkRed;
                    newConfig.editButton.Foreground = Brushes.GhostWhite;
                    newConfig.editButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newConfig.editButton.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.editButton.FontSize = 20;
                    newConfig.editButton.FontFamily = new FontFamily("Arial");
                    newConfig.editButton.BorderBrush = Brushes.GhostWhite;
                    newConfig.editButton.BorderThickness = new Thickness(2);
                    newConfig.editButton.Click += new RoutedEventHandler(EditButton_Click);
                }
                newConfig.deleteButton = new Button();
                {// Edit button creat
                    newConfig.deleteButton.Content = "Xoá";
                    newConfig.deleteButton.Margin = new Thickness(5, 5, 5, 5);
                    newConfig.deleteButton.Background = Brushes.DarkRed;
                    newConfig.deleteButton.Foreground = Brushes.GhostWhite;
                    newConfig.deleteButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newConfig.deleteButton.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.deleteButton.FontSize = 20;
                    newConfig.deleteButton.FontFamily = new FontFamily("Arial");
                    newConfig.deleteButton.BorderBrush = Brushes.GhostWhite;
                    newConfig.deleteButton.BorderThickness = new Thickness(2);
                    newConfig.deleteButton.Click += new RoutedEventHandler(DeleteButton_Click);
                }
                newConfig.layoutGrid = new Grid();
                {// Layout grid creat
                    ColumnDefinition column1 = new ColumnDefinition();
                    column1.Width = new GridLength(1, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column1);

                    ColumnDefinition column2 = new ColumnDefinition();
                    column2.Width = new GridLength(2, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column2);

                    ColumnDefinition column3 = new ColumnDefinition();
                    column3.Width = new GridLength(6, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column3);

                    ColumnDefinition column4 = new ColumnDefinition();
                    column4.Width = new GridLength(6, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column4);

                    ColumnDefinition column5 = new ColumnDefinition();
                    column5.Width = new GridLength(10, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column5);

                    ColumnDefinition column6 = new ColumnDefinition();
                    column6.Width = new GridLength(2, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column6);

                    ColumnDefinition column7 = new ColumnDefinition();
                    column7.Width = new GridLength(2, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column7);

                    ColumnDefinition column8 = new ColumnDefinition();
                    column8.Width = new GridLength(1, GridUnitType.Star);
                    newConfig.layoutGrid.ColumnDefinitions.Add(column8);
                }
                {// Add item to layout grid
                    newConfig.layoutGrid.Children.Add(newConfig.pIDLabel);
                    //Grid.SetRow(newConfig.pIDLabel, 0);
                    Grid.SetColumn(newConfig.pIDLabel, 1);

                    newConfig.layoutGrid.Children.Add(newConfig.nameTextBox);
                    Grid.SetColumn(newConfig.nameTextBox, 2);

                    newConfig.layoutGrid.Children.Add(newConfig.typeLabel);
                    Grid.SetColumn(newConfig.typeLabel, 3);

                    newConfig.layoutGrid.Children.Add(newConfig.opcServerURLTextBox);
                    Grid.SetColumn(newConfig.opcServerURLTextBox, 4);

                    newConfig.layoutGrid.Children.Add(newConfig.editButton);
                    Grid.SetColumn(newConfig.editButton, 5);

                    newConfig.layoutGrid.Children.Add(newConfig.deleteButton);
                    Grid.SetColumn(newConfig.deleteButton, 6);
                }
                listHardwarePanel.Children.Add(newConfig.layoutGrid);
                listHardware.Add(newConfig);
            }
        }
    }
}
