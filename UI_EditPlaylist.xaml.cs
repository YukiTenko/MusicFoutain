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
    /// <summary>
    /// Interaction logic for UI_EditPlaylist.xaml
    /// </summary>
    public partial class UI_EditPlaylist : Page
    {
        // Variables

        // UI
        private Button backButton;
        private Button addButton;
        private Button editButton;

        // Event
        public event EventHandler<int> backClick;
        public event EventHandler<int> addClick;
        public event EventHandler<int> editClick;
        public UI_EditPlaylist(string xmlFilePath)
        {
            InitializeComponent();

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_EditPlaylist: Can not find back button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
                }

                addButton = LogicalTreeHelper.FindLogicalNode(rootObject, "addMedia") as Button;
                if (addButton is null)
                {
                    MessageBox.Show("UI_EditPlaylist: Can not find addMedia button in file " + xmlFilePath);
                }
                else
                {
                    addButton.Click += new RoutedEventHandler(AddButton_Click);
                }

                editButton = LogicalTreeHelper.FindLogicalNode(rootObject, "editMusicEffect") as Button;
                if (editButton is null)
                {
                    MessageBox.Show("UI_EditPlaylist: Can not find editMusicEffect button in file " + xmlFilePath);
                }
                else
                {
                    editButton.Click += new RoutedEventHandler(EditButton_Click);
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
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            addClick.Invoke(this, 0);
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Invoke event
            editClick.Invoke(this, 0);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
    }
}
