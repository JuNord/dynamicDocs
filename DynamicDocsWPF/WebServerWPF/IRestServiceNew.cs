using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using DynamicDocsWPF.Model;
using RestService;
using RestService.Model.Database;
using WebServerWPF;
using WebServerWPF.RestDots;
using WebServerWPF.RestDTOs;

namespace WebServer
{
    [ServiceContract(Name = "RestServicesNew")]
    public interface IRestServiceNew
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessTemplate, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplate GetProcessTemplate(RequestGetProcessTemplate request);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetDocTemplate, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplate GetDocTemplate(RequestGetDocTemplate request);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessTemplateList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplateList GetProcessTemplateList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetDocTemplateList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplateList GetDocTemplateList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetAuthorized, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetAuthenticationResult CheckAuthentication();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetEntryList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetEntryList GetEntryList(RequestGetEntryList request);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetPermissionLevel, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetPermissionLevel GetPermissionLevel(RequestGetPermissionLevel request);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessInstance, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessInstance GetProcessInstance(RequestGetProcessInstance request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessTemplate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessTemplate PostProcessTemplate(RequestPostProcessTemplate request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostDocTemplate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostDocTemplate PostDocTemplate(RequestPostDocTemplate request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessUpdate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessUpdate PostProcessUpdate(RequestPostProcessUpdate request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessInstance, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessInstance PostProcessInstance(RequestPostProcessInstance request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostEntry, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostEntry PostEntry(RequestPostEntry request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostUser, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostUser Register(RequestPostUser request);
    }
}