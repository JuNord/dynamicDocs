using System.Windows;
using DynamicDocsWPF.Model;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model;
using RestService.Model.Database;

namespace DynamicDocsWPF.Windows
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        public string Email => EmailBox.Text;
        public string Password => PasswordBox.Password;
        public User User => new User(Email, Password);

        private void Login_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                LoginInfoText.Text = "Bitte geben sie eine Email-Adresse ein.";
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                LoginInfoText.Text = "Bitte geben sie ein Passwort ein.";
            }
            else
            {
                if (new NetworkHelper(ConfigurationManager.GetInstance().Url, User).CheckAuthorization() ==
                    AuthorizationResult.Authorized)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    LoginInfoText.Text = "Ein Account mit dieser Kombination wurde nicht gefunden.";
                }
            }
        }

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            var register = new Register();
            register.ShowDialog();
            if (register.DialogResult == true)
                LoginInfoText.Text = "Sie sind nun registriert.";
        }
    }
}