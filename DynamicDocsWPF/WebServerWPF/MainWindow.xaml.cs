using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using System.Xml;
using RestService;
using WebServer;

namespace WebServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int MaxRequestSize = 2147483647;
        private static MainWindow _mainWindow;
        private static WebServiceHost _serviceHost;

        public MainWindow()
        {
            _mainWindow = this;
            InitializeComponent();

            try
            {
                RunService();
                
                new Thread(() =>
                {
                    var helper = new DatabaseHelper();
                    
                    foreach (var template in helper.GetDocTemplates())
                    {
                        if (!File.Exists(template.FilePath))
                        {
                            helper.RemoveDocTemplate(template.Id);
                        }
                    }
                    
                    foreach (var template in helper.GetProcessTemplates())
                    {
                        if (!File.Exists(template.FilePath))
                        {
                            helper.RemoveProcessTemplate(template.Id);
                        }
                    }
                    
                    Thread.Sleep(60000);
                })
                {
                    IsBackground = true
                }.Start();
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

        public static void PostToLog(string text)
        {
            _mainWindow.Log.Text += $"\n[{DateTime.Now.ToShortTimeString()}] {text}";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _serviceHost.Close();
        }
    }
}