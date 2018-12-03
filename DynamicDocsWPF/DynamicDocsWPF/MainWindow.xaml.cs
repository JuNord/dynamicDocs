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
        private readonly NetworkHelper _networkHelper;
        private readonly User _user;

        public MainWindow()
        {
            InitializeComponent();
            var login = new Login();
            try
            {
                
                login.ShowDialog();

                if (login.DialogResult == true)
                {
                    _user = new User(login.Email, login.Password);
                    _networkHelper = new NetworkHelper("http://localhost:8000/Service", _user);
                    int level = _networkHelper.GetPermissionLevel();

                    switch (level)
                    {
                        case 0:
                            NoPermissionText.Visibility = Visibility.Visible;
                            MainMenu_BtnNewProcessInstance.Visibility = Visibility.Collapsed;
                            MainMenu_BtnViewInstances.Visibility = Visibility.Collapsed;
                            MainMenu_BtnUploadProcess.Visibility = Visibility.Collapsed;
                            MainMenu_BtnViewResponsibilities.Visibility = Visibility.Collapsed;
                            MainMenu_BtnManagePermissions.Visibility = Visibility.Collapsed;
                            break;
                        case 1:
                            MainMenu_BtnUploadProcess.Visibility = Visibility.Collapsed;
                            MainMenu_BtnManagePermissions.Visibility = Visibility.Collapsed;
                            break;
                        case 2:
                            MainMenu_BtnManagePermissions.Visibility = Visibility.Collapsed;
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

        private void HandleUploadResult(UploadResult result)
        {
            switch (result)
            {
                case UploadResult.FAILED_FILEEXISTS:
                    MessageBox.Show(this, "Filename already taken.");
                    break;
                case UploadResult.FAILED_ID_EXISTS:
                    MessageBox.Show(this, "ID already taken.");
                    break;
                case UploadResult.FAILED_OTHER:
                    MessageBox.Show(this, "Something went wrong.");
                    break;
                case UploadResult.INVALID_LOGIN:
                    MessageBox.Show(this, "Username or password was wrong.");
                    break;
                case UploadResult.NO_PERMISSION:
                    MessageBox.Show(this, "You are not permitted.");
                    break;
            }
        }

        private void MainMenu_BtnNewProcessInstance_OnClick(object sender, RoutedEventArgs e)
        {
            var processSelect = new ProcessSelect(_networkHelper);
            processSelect.ShowDialog();

            if (processSelect.DialogResult == true)
            {
                var file = _networkHelper.GetProcessTemplate(processSelect.SelectedProcessTemplate.Id);
                var process = XmlHelper.ReadXMLFromString(file);
                var newInstance = new CreateProcessInstance(process, _networkHelper);
                newInstance.ShowDialog();
            }
        }

        private void MainMenu_BtnUploadProcess_OnClick(object sender, RoutedEventArgs e)
        {
            var create = new CreateProcessTemplate(_networkHelper);
            create.ShowDialog();
        }

        private void MainMenu_BtnViewInstances_OnClick(object sender, RoutedEventArgs e)
        {
            var view = new ViewAllInstances(_networkHelper);
            view.ShowDialog();
        }

        private void MainMenu_BtnViewResponsibilities_OnClick(object sender, RoutedEventArgs e)
        {
            var view = new ViewPendingInstances(_networkHelper);
            view.ShowDialog();
        }
    }
}