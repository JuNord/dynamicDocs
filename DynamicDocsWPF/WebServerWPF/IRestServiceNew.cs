using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using DynamicDocsWPF.Model;
using RestService;
using RestService.Model.Database;
using RestService.RestDTOs;
using WebServerWPF;

namespace WebServer
{
    [ServiceContract(Name = "RestServicesNew")]
    public interface IRestServiceNew
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessTemplate, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplate GetProcessTemplate(string message);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetDocTemplate, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplate GetDocTemplate(string message);
        
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
        [WebGet(UriTemplate = Routing.GetProcessInstanceList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessInstanceList GetProcessInstanceList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetResponsibilityList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetResponsibilityList GetResponsibilityList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetUserList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetUserList GetUserList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetEntryList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetEntryList GetEntryList(string message);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetPermissionLevel, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetPermissionLevel GetPermissionLevel(string message);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessInstance, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessInstance GetProcessInstance(string message);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessTemplate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessTemplate PostProcessTemplate(RequestPostProcessTemplate request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostPermissionChange, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPermissionChange ChangePermission(RequestPermissionChange request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostDocTemplate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostDocTemplate PostDocTemplate(RequestPostDocTemplate request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessUpdate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessUpdate PostProcessUpdate(RequestPostProcessUpdate request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostEntryUpdate, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostEntryUpdate PostEntryUpdate(RequestPostEntryUpdate requestPost);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessInstance, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessInstance PostProcessInstance(RequestPostProcessInstance request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostEntry, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostEntry PostEntry(RequestPostEntry request);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostUser, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ReplyPostUser Register(RequestPostUser request);
    }
}