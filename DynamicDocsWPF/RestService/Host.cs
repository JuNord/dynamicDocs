using System;
using System.ServiceModel.Web;

namespace RestService
{
    public class Host
    {
        public static void Run()
        {
            var DemoServices = new RestService();
            var _serviceHost = new WebServiceHost(DemoServices, 
                new Uri("http://localhost:8000/DEMOService"));
            _serviceHost.Open();
            Console.ReadKey();
            _serviceHost.Close();
        }
    }
}