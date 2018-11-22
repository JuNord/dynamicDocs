using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace RestService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestService : IRestService
    {
        public string GetTemplate(string id)
        {
            return "";
        }

        public byte[] GetProcess(string name)
        {
            return null;
        }

        public void PostTemplate(TemplateMessage message)
        {
            Console.WriteLine(message.TemplateName);
        }

        public void PostProcess(ProcessMessage message)
        {
            Console.WriteLine(message.ProcessName);
        }
    }
}