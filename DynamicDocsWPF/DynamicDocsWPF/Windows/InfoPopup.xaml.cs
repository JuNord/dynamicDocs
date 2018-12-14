using System.Windows;

namespace DynamicDocsWPF.Windows
{
    public partial class InfoPopup : Window
    {
        private InfoPopup(MessageBoxButton buttons, string text)
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

        public static bool ShowOk(string message)
        {
            return new InfoPopup(MessageBoxButton.OK, message).ShowDialog() ?? false;
        }

        public static bool ShowYesNo(string message)
        {
            return new InfoPopup(MessageBoxButton.YesNo, message).ShowDialog() ?? false;
        }
    }
}