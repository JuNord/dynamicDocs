using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using DynamicDocsWPF.Model;
using RestService;
using RestService.Model.Database;
using WebServerWPF;
using WebServerWPF.RestDots;

namespace WebServer
{
    [ServiceContract(Name = "RestServicesNew")]
    public interface IRestServiceNew
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessTemplate+Routing.AuthExtension, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplate GetProcessTemplate(RequestGetProcessTemplate requestGetProcessTemplate, string email, string password);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessTemplate+Routing.AuthExtension, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplate GetDocTemplate(RequestGetDocTemplate requestGetDocTemplate, string email, string password);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessTemplateList+Routing.AuthExtension, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetProcessTemplateList GetProcessTemplateList(string email, string password);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetDocTemplateList+Routing.AuthExtension, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyGetDocTemplateList GetDocTemplateList(string email, string password);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostProcessTemplate+Routing.AuthExtension, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessTemplate PostProcessTemplate(RequestPostProcessTemplate requestPostProcessTemplate);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostDocTemplate+Routing.AuthExtension, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReplyPostProcessTemplate PostDocTemplate(RequestPostDocTemplate requestPostDocTemplate);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.GetAuthorized+Routing.AuthExtension, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        AuthorizationResult CheckAuth();
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.GetPermissionLevel, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        int GetPermissionLevel(User user);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.GetFile, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        FileMessage GetFile(DataMessage message);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostFile, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        UploadResult PostFile(FileMessage message);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostData, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        UploadResult PostData(DataMessage message);

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        List<string> GetProcessList();

        [OperationContract]
        [WebGet(UriTemplate = Routing.GetTemplateList, BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        List<string> GetTemplateList();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.GetDataList, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DataMessage GetDataList(DataMessage message);
    }
}