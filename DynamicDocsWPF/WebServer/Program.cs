using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;
using RestService;

namespace WebServer
{
    internal class Program
    {
        private const string BaseUrl = "http://localhost:8000/Service";
        private const int MaxRequestSize = 2147483647;
        public static void Main(string[] args)
        {
            RunService();
        }

        private static void RunService()
        {
            var serviceHost = new WebServiceHost(typeof(RestService));
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
            
            serviceHost.AddServiceEndpoint(typeof(IRestService), httpBinding, new Uri(BaseUrl));

            serviceHost.Open();
            Console.ReadKey();
            serviceHost.Close();
        }
    }
}
