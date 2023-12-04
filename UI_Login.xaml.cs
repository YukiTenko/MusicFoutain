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

namespace MusicFountain.UI
{
    /// <summary>
    /// Interaction logic for UI_Login.xaml
    /// </summary>
    public partial class UI_Login : Page
    {
        // Variables
        private Data.Data_UserInfo userInfo;

        // UI
        private TextBox userNameBox;
        private PasswordBox passwordBox;
        private Button loginButton;

        // Event
        public event EventHandler<Data.Data_UserInfo> loginClick;

        public UI_Login(string xmlFilePath)
        {
            InitializeComponent();

            {// Creat variables
                userInfo = new Data.Data_UserInfo();
            }

            try
            {// UI creat
                StreamReader mysr = new StreamReader(xmlFilePath);
                FrameworkElement rootObject = XamlReader.Load(mysr.BaseStream) as FrameworkElement;

                userNameBox = LogicalTreeHelper.FindLogicalNode(rootObject, "username") as TextBox;
                if (userNameBox is null)
                {
                    MessageBox.Show("UI_Login: Can not find username textbox in file " + xmlFilePath);
                }
                passwordBox = LogicalTreeHelper.FindLogicalNode(rootObject, "password") as PasswordBox;
                if (passwordBox is null)
                {
                    MessageBox.Show("UI_Login: Can not find password passwordbox in file " + xmlFilePath);
                }

                loginButton = LogicalTreeHelper.FindLogicalNode(rootObject, "login") as Button;
                if (loginButton is null)
                {
                    MessageBox.Show("UI_Login: Can not find login button in file " + xmlFilePath);
                }
                else
                {
                    loginButton.Click += new RoutedEventHandler(LoginButton_Click);
                }

                mainLayout.Children.Add(rootObject);
            }
            catch
            {
                MessageBox.Show("UI_Login: Can not load file " + xmlFilePath);
            }
        }
        // Private methods -------------------------------------------------------------------------------
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Get username and password from textbox
            userInfo.userName = userNameBox.Text;
            userInfo.password = passwordBox.Password;
            // Invoke event
            loginClick.Invoke(this, userInfo);
        }
        // Public methods -------------------------------------------------------------------------------
        public void Refresh()
        {
            userNameBox.Text = "";
            passwordBox.Password = "";
        }
    }
}
