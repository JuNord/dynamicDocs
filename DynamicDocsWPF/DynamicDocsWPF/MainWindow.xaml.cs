﻿using System;
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
                            MainMenu_BtnUploadProcess.Visibility = Visibility.Collapsed;
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

            OwnInstances.Content = new ViewOwnInstances(_networkHelper);
            ForeignInstances.Content = new ViewPendingInstances(_networkHelper);
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


        private void ManagePermissionsClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}