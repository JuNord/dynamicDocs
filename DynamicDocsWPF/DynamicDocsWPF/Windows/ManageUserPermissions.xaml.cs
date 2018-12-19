using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class ManageUserPermissions
    {
        private readonly NetworkHelper _networkHelper;

        private int _lastHash = 0;
        private readonly List<int> _changed = new List<int>();

        public ManageUserPermissions(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            Refresh();
        }

        private AdministrationContainer SelectedUser => (AdministrationContainer) UserList.SelectedItem;

        public void Refresh()
        {
            List<User> users = null;
            Role[] roles = null;

            users = _networkHelper.GetUsers();
            roles = _networkHelper.GetRoles()?.ToArray();

            var containers = new List<AdministrationContainer>();

            if (users != null)
            {
                foreach (var user in users)
                    containers.Add(new AdministrationContainer()
                        {
                            Email = user.Email,
                            PermissionLevel = user.PermissionLevel,
                            Role = roles?.FirstOrDefault(role => role.Mail.Equals(user.Email))?.RoleId ?? ""
                        }
                    );

                if (_lastHash != 0)
                    if (containers.GetHash() == _lastHash)
                        return;

                if (UserList != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        UserList.SelectedItems.Clear();
                        UserList.ItemsSource = containers;
                        return;
                    });
                    _lastHash = containers.GetHash();
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var entry in _changed)
            {
                var user = ((List<AdministrationContainer>) UserList.ItemsSource)[entry];
                _networkHelper.PostPermissionChange(user.Email, user.PermissionLevel,
                    user.Role);
            }
            
            _changed.Clear();
            Refresh();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if(UserList.SelectedIndex != -1)
            if(!_changed.Contains(UserList.SelectedIndex)) _changed.Add(UserList.SelectedIndex);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(UserList.SelectedIndex != -1)
            if(!_changed.Contains(UserList.SelectedIndex)) _changed.Add(UserList.SelectedIndex);
        }

    }
}