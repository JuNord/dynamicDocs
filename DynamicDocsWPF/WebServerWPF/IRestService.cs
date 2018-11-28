using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using RestService;
using WebServerWPF;

namespace WebServer
{
    [ServiceContract(Name = "RestServices")]
    public interface IRestService
    {
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetFile, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        FileMessage GetFile(string fileType, string name);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostFile, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UploadResult PostFile(FileMessage message);
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = Routing.PostData, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UploadResult PostData(DataMessage message);
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetProcessList, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetProcessList();
        
        [OperationContract]
        [WebGet(UriTemplate = Routing.GetTemplateList, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetTemplateList();
    }
}