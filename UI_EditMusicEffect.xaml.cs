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
    /// Interaction logic for UI_EditMusicEffect.xaml
    /// </summary>
    public partial class UI_EditMusicEffect : Page
    {
        // Variables

        // UI
        private Button backButton;

        // Event
        public event EventHandler<int> backClick;
        public UI_EditMusicEffect(string xmlFilePath)
        {
            InitializeComponent();

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                backButton = LogicalTreeHelper.FindLogicalNode(rootObject, "back") as Button;
                if (backButton is null)
                {
                    MessageBox.Show("UI_EditMusicEffect: Can not find back button in file " + xmlFilePath);
                }
                else
                {
                    backButton.Click += new RoutedEventHandler(BackButton_Click);
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
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
        }
    }
}
