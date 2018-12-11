using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using System.Xml;
using MySql.Data.MySqlClient;
using RestService;
using WebServer;

namespace WebServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static MainWindow mainWindow;
        private const int MaxRequestSize = 2147483647;
        private static WebServiceHost _serviceHost;

        private static void RunService()
        {
            _serviceHost = new WebServiceHost(typeof(RestServiceNew));
            var httpBinding = new WebHttpBinding();
            var readerQuotas = new XmlDictionaryReaderQuotas
            {
                MaxStringContentLength = MaxRequestSize,
                MaxArrayLength = MaxRequestSize,
                MaxBytesPerRead = MaxRequestSize,
                MaxDepth = MaxRequestSize,
                MaxNameTableCharCount = MaxRequestSize
            };

            httpBinding.GetType().GetProperty("ReaderQuotas")?.SetValue(httpBinding, readerQuotas, null);
            httpBinding.MaxReceivedMessageSize = MaxRequestSize;
            httpBinding.MaxBufferSize = MaxRequestSize;

            var url = ConfigurationManager.GetInstance().Url;
            PostToLog(url);
            _serviceHost.AddServiceEndpoint(typeof(IRestServiceNew), httpBinding, new Uri(url));
            _serviceHost.Open();

            PostToLog("Server Running");
        }
        
        public MainWindow()
        {
            mainWindow = this;
            InitializeComponent();

            try
            {
                RunService();
            }
            catch (TargetInvocationException)
            {
                PostToLog("Der Datenbankserver ist nicht erreichbar.");
            }
            catch (Exception e)
            {
                PostToLog(e.Source);
                PostToLog(e.Message);
                PostToLog(e.StackTrace);
            }
        }

        public static void PostToLog(string text)
        {
            mainWindow.Log.Text += $"\n[{DateTime.Now.ToShortTimeString()}] {text}";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);            
            _serviceHost.Close();
        }
    }
}