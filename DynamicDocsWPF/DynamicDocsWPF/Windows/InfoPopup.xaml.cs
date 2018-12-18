using System.Data;
using System.Windows;

namespace DynamicDocsWPF.Windows
{
    public partial class InfoPopup : Window
    {
        
        private InfoPopup(string text, string yesLabel)
        {
            InitializeComponent();
            Accept.Content = yesLabel;
            Decline.Visibility = Visibility.Collapsed;
            InfoBlock.Text = text;
        }
        
        private InfoPopup(string text, string yesLabel, string noLabel)
        {
            InitializeComponent();
            Accept.Content = yesLabel;
            Decline.Content = noLabel;
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
            return new InfoPopup(message, "OK").ShowDialog() ?? false;
        }

        
        public static bool ShowYesNo(string message, string yesLabel, string noLabel)
        {
            return new InfoPopup(message, yesLabel, noLabel).ShowDialog() ?? false;
        }
        
        public static bool ShowYesNo(string message)
        {
            return new InfoPopup(message, "Bestätigen", "Ablehnen").ShowDialog() ?? false;
        }
    }
}