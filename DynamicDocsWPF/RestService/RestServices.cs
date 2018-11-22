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
            Random r = new Random();
            string ReturnString = "";
            int Idnum = Convert.ToInt32(id);
            for (int i = 0; i < Idnum; i++)
                ReturnString += char.ConvertFromUtf32(r.Next(65, 85));

            return ReturnString;
        }
    }
}