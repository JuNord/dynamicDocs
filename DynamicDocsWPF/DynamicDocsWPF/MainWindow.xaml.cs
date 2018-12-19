using System;
using System.Net;
using System.Threading;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using DynamicDocsWPF.Windows;
using RestService;
using RestService.Model.Database;

namespace DynamicDocsWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _lastPermission = -1;
        private NetworkHelper _networkHelper;
        private User _user;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Connect();
                DisplayInfo(MoTD);
                new Thread(AuthCheck) {IsBackground = true}.Start();
                new Thread(Idle) {IsBackground = true}.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.InnerException.Message);
            }
        }

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

        private void AuthCheck()
        {
            while (true)
            {
                Dispatcher.Invoke(HandlePermissionLevel);
                Thread.Sleep(1000);
            }
        }

        private void Idle()
        {
            while (true)
            {
                if (_lastPermission > -1)
                    Dispatcher.Invoke(() =>
                    {
                        var foreign = ForeignInstances.Content as ViewPendingInstances;
                        var own = OwnInstances.Content as ViewOwnInstances;
                        var admin = AdministrationContent.Content as ManageUserPermissions;
                        foreign?.Refresh();
                        own?.Refresh();
                        admin?.Refresh();
                    });

                Thread.Sleep(2000);
            }
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
                    _networkHelper = new NetworkHelper(ConfigurationManager.GetInstance().Url, _user);

                    HandlePermissionLevel();
                }
                else
                {
                    Close();
                }
            }
            catch (WebException)
            {
                InfoPopup.ShowOk(StringResources.ConnectionError);
                login.Close();
                Close();
            }
        }

        private void HandlePermissionLevel()
        {
            var level = _networkHelper?.GetPermissionLevel();
            if (level == null || level == _lastPermission) return;

            if (level > -1)
            {
                _lastPermission = (int) level;
                NoPermissionText.Visibility = Visibility.Collapsed;
                Administration.Visibility = Visibility.Visible;
                MyProcesses.Visibility = Visibility.Visible;
                ForeignProcesses.Visibility = Visibility.Visible;

                switch (level)
                {
                    case 1:
                    {
                        Administration.Visibility = Visibility.Collapsed;
                        if (OwnInstances?.Content == null)
                            OwnInstances.Content = new ViewOwnInstances(this, _networkHelper);
                        else OwnInstances.Visibility = Visibility.Visible;
                        if (ForeignInstances?.Content == null)
                            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);
                        else ForeignInstances.Visibility = Visibility.Visible;
                        break;
                    }
                    case 2:
                    {
                        if (OwnInstances?.Content == null)
                            OwnInstances.Content = new ViewOwnInstances(this, _networkHelper);
                        else OwnInstances.Visibility = Visibility.Visible;
                        if (ForeignInstances?.Content == null)
                            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);
                        else ForeignInstances.Visibility = Visibility.Visible;
                        if (AdministrationContent?.Content == null)
                            AdministrationContent.Content = new ManageUserPermissions(_networkHelper);
                        else AdministrationContent.Visibility = Visibility.Visible;
                        ((ManageUserPermissions) AdministrationContent.Content).UserList.Visibility =
                            Visibility.Collapsed;
                        break;
                    }
                    case 3:
                    {
                        if (OwnInstances?.Content == null)
                            OwnInstances.Content = new ViewOwnInstances(this, _networkHelper);
                        else OwnInstances.Visibility = Visibility.Visible;
                        if (ForeignInstances?.Content == null)
                            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);
                        else ForeignInstances.Visibility = Visibility.Visible;
                        if (OwnInstances?.Content == null)
                            OwnInstances.Content = new ViewOwnInstances(this, _networkHelper);
                        else OwnInstances.Visibility = Visibility.Visible;
                        if (AdministrationContent?.Content == null)
                            AdministrationContent.Content = new ManageUserPermissions(_networkHelper);
                        else AdministrationContent.Visibility = Visibility.Visible;
                        ((ManageUserPermissions) AdministrationContent.Content).UserList.Visibility =
                            Visibility.Visible;
                        break;
                    }
                    default:    
                        Administration.Visibility = Visibility.Collapsed;
                        MyProcesses.Visibility = Visibility.Collapsed;
                        ForeignProcesses.Visibility = Visibility.Collapsed;
                        NoPermissionText.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                Connect();
            }
        }

        public void DisplayInfo(string text)
        {
            InfoBlock.Text = text;
        }

        private void NewProcess_Click(object sender, RoutedEventArgs e)
        {
            CreateProcessTemplate create = null;

            try
            {
                create = new CreateProcessTemplate(_networkHelper);
                create.ShowDialog();
            }
            catch (WebException)
            {
                InfoPopup.ShowOk(StringResources.ConnectionError);
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