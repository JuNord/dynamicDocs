using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class ProcessSelect : Window
    {
        private NetworkHelper _networkHelper;
        public ProcessTemplate SelectedProcessTemplate { get; set; }
        public ProcessSelect(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            ProcessCombobox.ItemsSource = _networkHelper.GetProcessTemplates();
        }

        private void ProcessSelect_btnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            if (ProcessCombobox.SelectedIndex > -1)
            {
                SelectedProcessTemplate = ProcessCombobox.SelectedItem as ProcessTemplate;
                DialogResult = true;
                Close();
            }
            else
            {
                ProcessSelect_InfoText.Text = "Bitte wählen Sie einen Prozess aus.";
            }
            
        }
    }
}
