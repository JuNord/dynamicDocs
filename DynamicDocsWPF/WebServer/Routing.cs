namespace WebServer
{
    public static class Routing
    {
        public const string GetFile = "/{fileType}/{name}";
        public const string GetProcessList = "/Processes";
        public const string GetTemplateList = "/Templates";
        public const string PostFile = "/Upload";
    }
}