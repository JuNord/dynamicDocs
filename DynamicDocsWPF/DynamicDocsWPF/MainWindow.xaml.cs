using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using DynamicDocsWPF.HelperClasses;
using DynamicDocsWPF.Windows;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;

namespace DynamicDocsWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private User _user;

        public MainWindow()
        {
            InitializeComponent();
            var newinstance = new CreateProcessInstance(XmlHelper.ReadXMLFromPath(@"C:\Users\sebastian.bauer\RiderProjects\dynamicDocs\DynamicDocsWPF\XmlProcessor\XMLFile1.xml"));
            newinstance.ShowDialog();
            /*var login = new Login();
            login.ShowDialog();

            if (login.DialogResult == true)
            {
                _user = new User(login.Email, login.Password);
                var helper = new NetworkHelper("http://localhost:8000/Service", _user);

                var processSelect = new ProcessSelect(helper);
                processSelect.ShowDialog();
            }
            else Close();*/
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
            throw new NotImplementedException();
        }
    }
}