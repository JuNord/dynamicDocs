using System.Collections.Generic;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class ViewAllInstances : Window
    {
        private readonly NetworkHelper _networkHelper;
        private List<RunningProcess> _instances;

        public ViewAllInstances(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            _instances = _networkHelper.GetProcessInstances();
            InstanceList.ItemsSource();
        }

        private void ViewAllInstances_Btn_Next_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
