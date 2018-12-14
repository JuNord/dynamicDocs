using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class ProcessSelect : Window
    {
        private readonly NetworkHelper _networkHelper;

        public ProcessSelect(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            ProcessCombobox.ItemsSource = _networkHelper.GetProcessTemplates();
        }

        public ProcessTemplate SelectedProcessTemplate { get; set; }

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
                ProcessSelectInfoText.Text = "Bitte wählen Sie einen Prozess aus.";
            }
        }
    }
}