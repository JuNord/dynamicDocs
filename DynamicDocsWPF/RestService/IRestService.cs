
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RestService
{
    [ServiceContract(Name = "RestServices")]
    public interface IRestService
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetTemplate, BodyStyle = WebMessageBodyStyle.Bare)]
        string GetTemplate(string name);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetTemplate, BodyStyle = WebMessageBodyStyle.Bare)]
        byte[] GetProcess(string name);
    }
}