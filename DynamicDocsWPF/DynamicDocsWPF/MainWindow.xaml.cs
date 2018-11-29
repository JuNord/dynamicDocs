using System;
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

        public MainWindow()
        {
            InitializeComponent();
            
            var login = new Login();
            login.ShowDialog();

            if (login.DialogResult == true)
            {
                _user = new User(login.Email, login.Password);
                _networkHelper = new NetworkHelper("http://localhost:8000/Service", _user);

            }
            else Close();
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
            
            
            var file = _networkHelper.GetProcessById(processSelect.SelectedProcessTemplate.Id);
            var process = XmlHelper.ReadXMLFromString(file);
            var newInstance = new CreateProcessInstance(process);
            newInstance.ShowDialog();
        }
    }
}