
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
        
        [OperationContract]
        [WebInvoke(Method = "Post", UriTemplate = Routing.PostTemplate, RequestFormat = WebMessageFormat.Json)]
        void PostTemplate(TemplateMessage message);
        
        [OperationContract]
        [WebInvoke(Method = "Post", UriTemplate = Routing.PostProcess, RequestFormat = WebMessageFormat.Json)]
        void PostProcess(ProcessMessage message);
    }
}