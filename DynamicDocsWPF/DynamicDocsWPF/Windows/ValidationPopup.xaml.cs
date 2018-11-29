using System.Windows;

namespace DynamicDocsWPF.Windows
{
    public partial class ValidationPopup : Window
    {
        public ValidationPopup()
        {
            InitializeComponent();
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
