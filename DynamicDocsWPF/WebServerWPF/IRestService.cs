using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using DynamicDocsWPF.Model;
using RestService;
using RestService.Model.Database;
using WebServerWPF;

namespace WebServer
{
    [ServiceContract(Name = "RestServices")]
    public interface IRestService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.CheckAuth, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        AuthorizationResult CheckAuth(User user);
        
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