using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;

namespace RestService
{
    public class Host
    {
        public const string BASEURL = "http://localhost:8000/DEMOService";
        public static void Run()
        {
            WebHttpBinding binding2 = new WebHttpBinding();
            XmlDictionaryReaderQuotas myReaderQuotas = new XmlDictionaryReaderQuotas();
            myReaderQuotas.MaxStringContentLength = 2147483647;
            myReaderQuotas.MaxArrayLength = 2147483647;
            myReaderQuotas.MaxBytesPerRead = 2147483647;
            myReaderQuotas.MaxDepth = 2147483647;
            myReaderQuotas.MaxNameTableCharCount = 2147483647;

            binding2.GetType().GetProperty("ReaderQuotas").SetValue(binding2, myReaderQuotas, null);
            binding2.MaxBufferSize = 2147483647;
            binding2.MaxReceivedMessageSize = 2147483647;

            WebServiceHost host2 = new WebServiceHost(typeof(RestService));
            host2.AddServiceEndpoint(typeof(IRestService), binding2, new Uri(BASEURL));

            host2.Open();
            Console.ReadKey();
            host2.Close();
        }
    }
}