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
    public class UISupport_ConfigGroup
    {
        public int groupID { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public Grid layoutGrid { get; set; }
        public Label groupIDLabel;
        public Label typeLabel;
        public TextBox nameTextBox;
        public Button editButton;
        public Button deleteButton;
    }
    /// <summary>
    /// Interaction logic for UI_ConfigGroup.xaml
    /// </summary>
    public partial class UI_ConfigGroup : Page
    {
        // Variables
        private List<UISupport_ConfigGroup> listGroup;

        // UI
        private Button backButton;
        private Button saveButton;
        private Panel listGroupPanel;
        private TextBox newNameTextBox;
        private ComboBox typeComboBox;
        private Button addButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<List<Data_Config_Hardware_Group>> saveClick;
        public event EventHandler<Data_Config_Hardware_Group> editClick;
        public event EventHandler<int> deleteClick;
        public event EventHandler<Data_Config_Hardware_Group> addClick;
        public UI_ConfigGroup(string xmlFilePath)
        {
            InitializeComponent();

            {// Variable creat
                listGroup = new List<UISupport_ConfigGroup>();
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_ConfigGroup: Can not find back Button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                saveButton = LogicalTreeHelper.FindLogicalNode(rootObject, "save") as Button;
                if (saveButton is null)
                {
                    MessageBox.Show("UI_ConfigGroup: Can not find save Button in file " + xmlFilePath);
                }
                else
                {
                    saveButton.Click += new RoutedEventHandler(SaveButton_Click);
                }

                listGroupPanel = LogicalTreeHelper.FindLogicalNode(rootObject, "listGroup") as StackPanel;
                if (listGroupPanel is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find listGroup StackPanel in file " + xmlFilePath);
                }

                newNameTextBox = LogicalTreeHelper.FindLogicalNode(rootObject, "newName") as TextBox;
                if (newNameTextBox is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find newName TextBox in file " + xmlFilePath);
                }

                typeComboBox = LogicalTreeHelper.FindLogicalNode(rootObject, "newType") as ComboBox;
                if (listGroupPanel is null)
                {
                    MessageBox.Show("UI_ConfigHardware: Can not find newType ComboBox in file " + xmlFilePath);
                }

                addButton = LogicalTreeHelper.FindLogicalNode(rootObject, "add") as Button;
                if (addButton is null)
                {
                    MessageBox.Show("UI_ConfigGroup: Can not find add Button in file " + xmlFilePath);
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
            // Invoke event
            backClick.Invoke(this, 0);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<Data_Config_Hardware_Group> listGroupBuf = new List<Data_Config_Hardware_Group>();
            for (int i = 0; i < listGroup.Count; i++)
            {
                Data_Config_Hardware_Group groupBuf = new Data_Config_Hardware_Group();
                listGroupBuf.Add(groupBuf);
            }
            saveClick.Invoke(this, listGroupBuf);
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            bool senderCheck = false;
            for (int i = 0; i < listGroup.Count; i++)
            {
                if (listGroup[i].editButton == sender)
                {
                    Data_Config_Hardware_Group groupBuf = new Data_Config_Hardware_Group();
                    groupBuf.groupID = int.Parse(listGroup[i].groupIDLabel.Content.ToString());
                    groupBuf.name = listGroup[i].nameTextBox.Text;
                    groupBuf.type = listGroup[i].typeLabel.Content.ToString();
                    editClick.Invoke(this, groupBuf);
                    senderCheck = true;
                }
            }
            if (senderCheck == false)
            {
                MessageBox.Show("TestButton_Click: Cannot find button in listGroup config");
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            bool senderCheck = false;
            for (int i = 0; i < listGroup.Count; i++)
            {
                if (listGroup[i].deleteButton == sender)
                {
                    deleteClick.Invoke(this, int.Parse(listGroup[i].groupIDLabel.Content.ToString()));
                    senderCheck = true;
                }
            }
            if (senderCheck == false)
            {
                MessageBox.Show("TestButton_Click: Cannot find button in listGroup config");
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Data_Config_Hardware_Group newGroup = new Data_Config_Hardware_Group();
            newGroup.name = newNameTextBox.Text;
            newGroup.type = typeComboBox.Text;
            addClick.Invoke(this, newGroup);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
        public void Update_ListGroup(List<Data_Config_Hardware_Group> newList)
        {
            // Clear list data and UI stack panel
            listGroup.Clear();
            listGroupPanel.Children.Clear();
            // Add new list
            for (int i = 0; i < newList.Count; i++)
            {
                UISupport_ConfigGroup newConfig = new UISupport_ConfigGroup();
                newConfig.groupID = newList[i].groupID;
                newConfig.name = newList[i].name;
                newConfig.type = newList[i].type;
                newConfig.groupIDLabel = new Label();
                {// GlobalID label creat
                    newConfig.groupIDLabel.Content = newConfig.groupID.ToString();
                    //newConfig.pIDLabel.Margin = new Thickness(0, 10, 0, 0);
                    newConfig.groupIDLabel.Background = new BrushConverter().ConvertFromString("#99000000") as SolidColorBrush;
                    newConfig.groupIDLabel.Foreground = Brushes.GhostWhite;
                    newConfig.groupIDLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newConfig.groupIDLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    newConfig.groupIDLabel.FontSize = 20;
                    newConfig.groupIDLabel.FontFamily = new FontFamily("Arial");
                    newConfig.groupIDLabel.BorderBrush = Brushes.Gray;
                    newConfig.groupIDLabel.BorderThickness = new Thickness(1);
                }
                newConfig.nameTextBox = new TextBox();
                {// GlobalID label creat
                    newConfig.nameTextBox.Text = newConfig.name;
                    //newConfig.nameLabel.Margin = new Thickness(0, 10, 0, 0);
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
                    newConfig.layoutGrid.Children.Add(newConfig.groupIDLabel);
                    //Grid.SetRow(newConfig.pIDLabel, 0);
                    Grid.SetColumn(newConfig.groupIDLabel, 1);

                    newConfig.layoutGrid.Children.Add(newConfig.nameTextBox);
                    Grid.SetColumn(newConfig.nameTextBox, 2);

                    newConfig.layoutGrid.Children.Add(newConfig.typeLabel);
                    Grid.SetColumn(newConfig.typeLabel, 3);

                    newConfig.layoutGrid.Children.Add(newConfig.editButton);
                    Grid.SetColumn(newConfig.editButton, 4);

                    newConfig.layoutGrid.Children.Add(newConfig.deleteButton);
                    Grid.SetColumn(newConfig.deleteButton, 5);
                }
                listGroupPanel.Children.Add(newConfig.layoutGrid);
                listGroup.Add(newConfig);
            }
        }
    }
}
