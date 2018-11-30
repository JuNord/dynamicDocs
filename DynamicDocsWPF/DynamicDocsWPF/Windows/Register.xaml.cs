using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class Register : Window
    {
        public string Email => EmailBox.Text;
        public string Password => PasswordBox.Password;
        public User User => new User(Email, Password);
        public Register()
        {
            InitializeComponent();
        }

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Register_InfoText.Text = "Bitte geben Sie eine Email-Adresse ein.";
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                Register_InfoText.Text = "Bitte geben Sie ein Passwort ein.";
            }
            else
            {
                var result = new NetworkHelper("http://localhost:8000/Service",User).Register();
                switch (result)
                {
                    case UploadResult.USER_EXISTS:
                        Register_InfoText.Text = "Ein Nutzer mit dieser Email Adresse existiert bereits.";
                        break;
                    case UploadResult.SUCCESS:
                        DialogResult = true;
                        Close();
                        break;
                    default:
                        Register_InfoText.Text = "Etwas ist schiefgelaufen. Prüfen Sie Ihre Eingaben oder kontaktieren Sie den Administrator.";
                        break;
                }
            }
        }
    }
}
