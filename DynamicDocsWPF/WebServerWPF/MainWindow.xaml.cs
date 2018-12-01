using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using System.Xml;
using WebServer;

namespace WebServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow mainWindow;
        private const string BaseUrl = "http://localhost:8000/Service";
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
            httpBinding.MaxBufferSize = MaxRequestSize;
            httpBinding.MaxReceivedMessageSize = MaxRequestSize;
            
            _serviceHost.AddServiceEndpoint(typeof(IRestServiceNew), httpBinding, new Uri(BaseUrl));

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
            catch (Exception e)
            {
                PostToLog(e.Source);
                PostToLog(e.Message);
                PostToLog(e.StackTrace);
            }
        }

        public static void PostToLog(string text)
        {
            mainWindow.Log.Text += "\n" + text;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            PostToLog("Server stopping...");
            base.OnClosing(e);            
            Thread.Sleep(500);
            _serviceHost.Close();
        }
    }
}