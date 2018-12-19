using System.ServiceModel;
using System.ServiceModel.Web;
using RestService;
using RestService.RestDTOs;
using WebServerWPF;

namespace WebServer
{
    [ServiceContract(Name = "RestServicesNew")]
    public interface IRestServiceNew
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcess, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplate GetProcessTemplate(string message);

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetDocTemplate, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplate GetDocTemplate(string message);

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcesses, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplateList GetProcessTemplateList();

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetReceipts, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplateList GetDocTemplateList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetRoles, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetRoles GetRoleList();

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetAuthorized, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetAuthenticationResult CheckAuthentication();

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetInstances, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessInstanceList GetProcessInstanceList();

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetArchived, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetArchivedInstanceList GetArchivedInstanceList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetPending, BodyStyle = WebMessageBodyStyle.Bare,
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
        [WebGet(UriTemplate = Routing.GetPermission, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetPermissionLevel GetPermissionLevel(string message);

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetInstance, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessInstance GetProcessInstance(string message);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.AddProcess, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessTemplate PostProcessTemplate(RequestPostProcessTemplate request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.UpdatePermission, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPermissionChange ChangePermission(RequestPermissionChange request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.AddReceipt, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostDocTemplate PostDocTemplate(RequestPostDocTemplate request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.UpdateInstance, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessUpdate PostProcessUpdate(RequestPostProcessUpdate request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.UpdateEntry, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostEntryUpdate PostEntryUpdate(RequestPostEntryUpdate requestPost);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.AddInstance, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessInstance PostProcessInstance(RequestPostProcessInstance request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.AddEntry, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostEntry PostEntry(RequestPostEntry request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.Register, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostUser Register(RequestPostUser request);
    }
}