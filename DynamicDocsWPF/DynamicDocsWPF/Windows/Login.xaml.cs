using System.Configuration;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using DynamicDocsWPF.Model;
using RestService;
using RestService.Model.Database;
using ConfigurationManager = DynamicDocsWPF.HelperClasses.ConfigurationManager;
using Window = System.Windows.Window;

namespace DynamicDocsWPF.Windows
{
    public partial class Login : Window
    {
        public string Email => EmailBox.Text;
        public string Password => PasswordBox.Password;
        public User User => new User(Email, Password);
        
        public Login()
        {
            InitializeComponent();
        }

        private void Login_OnClick(object sender, RoutedEventArgs e)
        { if (string.IsNullOrWhiteSpace(Email))
            {
                Login_InfoText.Text = "Bitte geben sie eine Email-Adresse ein.";
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                Login_InfoText.Text = "Bitte geben sie ein Passwort ein.";
            }
            else
            {
                if (new NetworkHelper(ConfigurationManager.GetInstance().Url,User).CheckAuthorization() ==
                    AuthorizationResult.AUTHORIZED)
                {
                    DialogResult = true;
                    Close();
                }
                else Login_InfoText.Text = "Ein Account mit dieser Kombination wurde nicht gefunden.";
            }
        }

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            var register = new Register();
            register.ShowDialog();
            if (register.DialogResult == true)
                Login_InfoText.Text = "Sie sind nun registriert.";
        }
    }
}
