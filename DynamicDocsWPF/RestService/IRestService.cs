
using System;
using System.Diagnostics;
using System.Net;
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
        [WebGet(UriTemplate = Routing.GetProcess, BodyStyle = WebMessageBodyStyle.Bare)]
        byte[] GetProcess(string name);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostTemplate, RequestFormat = WebMessageFormat.Json)]
        TemplateMessage PostTemplate(TemplateMessage message);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcess, RequestFormat = WebMessageFormat.Json)]
        ProcessMessage PostProcess(ProcessMessage message);
    }
}