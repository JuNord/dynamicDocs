using System.Windows;

namespace DynamicDocsWPF.Windows
{
    public partial class InfoPopup : Window
    {
        public InfoPopup(MessageBoxButton buttons, string text)
        {
            InitializeComponent();
            if (buttons == MessageBoxButton.OK)
            {
                Accept.Content = "OK";
                Decline.Visibility = Visibility.Collapsed;
            }

            InfoBlock.Text = text;
        }

        private void Accept_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Decline_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
