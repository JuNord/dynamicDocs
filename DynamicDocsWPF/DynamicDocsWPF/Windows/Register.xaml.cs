using System.Text.RegularExpressions;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        public string Email => EmailBox.Text;
        public string Password => PasswordBox.Password;
        public User User => new User(Email, Password);

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            Regex regex;
            regex = new Regex("^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$");

            if (string.IsNullOrWhiteSpace(Email))
            {
                RegisterInfoText.Text = "Bitte geben Sie eine Email-Adresse ein.";
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                RegisterInfoText.Text = "Bitte geben Sie ein Passwort ein.";
            }
            else if (!regex.IsMatch(Email))
            {
                RegisterInfoText.Text = "Das ist keine gültige Email-Adresse.";
            }
            else
            {
                var result = new NetworkHelper(ConfigurationManager.GetInstance().Url, User).Register();
                switch (result)
                {
                    case UploadResult.UserExists:
                        RegisterInfoText.Text = "Ein Nutzer mit dieser Email Adresse existiert bereits.";
                        break;
                    case UploadResult.Success:
                        DialogResult = true;
                        Close();
                        break;
                    default:
                        RegisterInfoText.Text =
                            "Etwas ist schiefgelaufen. Prüfen Sie Ihre Eingaben oder kontaktieren Sie den Administrator.";
                        break;
                }
            }
        }
    }
}