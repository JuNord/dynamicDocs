using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;

namespace RestService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestService : IRestService
    {
        public string GetTemplate(string id)
        {
            return "BLA";
        }

        public byte[] GetProcess(string name)
        {
            return Encoding.UTF8.GetBytes("BLA");
        }

        public TemplateMessage PostTemplate(TemplateMessage message)
        {
            Console.WriteLine("Received SMTH");
            Console.WriteLine(message.TemplateName);
            Console.WriteLine("SAVING AS: "+message.FileName);
            File.WriteAllBytes(message.FileName,Encoding.UTF8.GetBytes(message.Content));
   
            return message;
        }

        public ProcessMessage PostProcess(ProcessMessage message)
        {
            Console.WriteLine("Received SMTH");
            Console.WriteLine(message.XML);
            return message;
        }
    }
}