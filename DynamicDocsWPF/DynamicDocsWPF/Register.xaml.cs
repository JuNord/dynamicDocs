using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using WebServerWPF;

namespace DynamicDocsWPF
{
    public partial class Register : Window
    {
        public string Email => EmailBox.Text;
        public string Password => PasswordBox.Password;
        public Register()
        {
            InitializeComponent();
        }

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Register_InfoText.Text = "Bitte geben sie eine Email-Adresse ein.";
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                Register_InfoText.Text = "Bitte geben sie ein Passwort ein.";
            }
            else
            {
                var result = NetworkHelper.CreateUser("http://localhost:8000/Service", Email, HashHelper.Hash(Password));
                if (result == UploadResult.USER_EXISTS)
                {
                    Register_InfoText.Text = "Ein Nutzer mit dieser Email Adresse existiert bereits.";
                }
                else Close();
            }
        }
    }
}
