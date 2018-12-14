using System.Windows.Controls;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class ManageUserPermissions
    {
        private readonly NetworkHelper _networkHelper;

        private int _selectedIndex = -1;

        public ManageUserPermissions(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            Refresh();
        }

        private User SelectedUser => (User) UserList.SelectedItem;

        public void Refresh()
        {
            UserList.ItemsSource = _networkHelper.GetUsers();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList.SelectedIndex != -1)
                _networkHelper.PostPermissionChange(SelectedUser.Email, SelectedUser.PermissionLevel);
        }
    }
}