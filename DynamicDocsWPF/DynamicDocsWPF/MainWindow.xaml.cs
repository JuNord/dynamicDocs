using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Media;
using DynamicDocsWPF.HelperClasses;
using DynamicDocsWPF.Windows;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;
using Login = DynamicDocsWPF.Windows.Login;

namespace DynamicDocsWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private NetworkHelper _networkHelper;
        private User _user;
        
        public static string MoTD
        {
            get
            {
                if (DateTime.Now.Hour < 12) return "Guten Morgen!";
                if (DateTime.Now.Hour < 17) return "Guten Tag!";
                if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 17) return "Guten Abend!";
                return "Hallo";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Connect();
            DisplayInfo(MoTD);
        }

        private void Connect()
        {
            var login = new Login();
            try
            {
                login.ShowDialog();

                if (login.DialogResult == true)
                {
                    _user = new User(login.Email, login.Password);
                    _networkHelper = new NetworkHelper("http://localhost:8000/Service", _user);
                    int level = _networkHelper.GetPermissionLevel();
                 
                    NoPermissionText.Visibility = Visibility.Collapsed;
                    Administration.Visibility = Visibility.Visible;
                    MyProcesses.Visibility = Visibility.Visible;
                    ForeignProcesses.Visibility = Visibility.Visible;
                    
                    switch (level)
                    {
                        case 0:
                            Administration.Visibility = Visibility.Collapsed;
                            MyProcesses.Visibility = Visibility.Collapsed;
                            ForeignProcesses.Visibility = Visibility.Collapsed;
                            NoPermissionText.Visibility = Visibility.Visible;
                            break;
                        case 1:
                            Administration.Visibility = Visibility.Collapsed;
                            OwnInstances.Content = new ViewOwnInstances(this,_networkHelper);
                            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);
                            break;
                        case 2:
                            OwnInstances.Content = new ViewOwnInstances(this,_networkHelper);
                            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);

                            break;
                        case 3:
                            OwnInstances.Content = new ViewOwnInstances(this,_networkHelper);
                            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);
                            AdministrationContent.Content = new ManageUserPermissions(_networkHelper);
                            break;
                    }
                    
                }
                else Close();
            }
            catch (WebException)
            {
                new InfoPopup(MessageBoxButton.OK ,"Der Server ist derzeit nicht erreichbar.").ShowDialog();
                login.Close();
                Close();
            }

            
        }

        public void DisplayInfo(string text)
        {
            InfoBlock.Text = text;
        }

        private void MainMenu_BtnUploadProcess_OnClick(object sender, RoutedEventArgs e)
        {
            CreateProcessTemplate create = null;
            
            try
            {
                create = new CreateProcessTemplate(_networkHelper);
                create.ShowDialog();
            }
            catch (WebException)
            {
                new InfoPopup(MessageBoxButton.OK ,"Der Server ist derzeit nicht erreichbar.").ShowDialog();
                create?.Close();
                Close();
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }
    }
}