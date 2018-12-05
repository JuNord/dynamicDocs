using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.HelperClasses;
using Microsoft.Office.Interop.Word;
using RestService;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;
using Dialog = RestService.Model.Input.Dialog;
using Window = System.Windows.Window;

namespace DynamicDocsWPF.Windows
{
    public partial class ManageUserPermissions
    {
        private readonly NetworkHelper _networkHelper;
        
        private User SelectedUser => ((User) UserList.SelectedItem);

        public ManageUserPermissions(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            UserList.ItemsSource = _networkHelper.GetUsers();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList.SelectedIndex != -1)
                _networkHelper.PostPermissionChange(SelectedUser.Email, UserList.SelectedIndex);
            else new InfoPopup(MessageBoxButton.OK, "");
        }
    }
}
