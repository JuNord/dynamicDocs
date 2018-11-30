namespace WebServerWPF
{
    public static class Routing
    {
        public const string AuthExtension = "?{Email}:{Password}";
        
        public const string GetProcessTemplate = "/ProcessTemplate/{requestGetProcessTemplate}";
        public const string GetDocTemplate = "/ProcessTemplate/{requestGetDocTemplate}";
        public const string PostProcessTemplate = "/ProcessTemplate/{requestPostProcessTemplate}";
        public const string PostDocTemplate = "/DocumentTemplate/{requestPostDocTemplate}";
        public const string GetProcessTemplateList = "/ProcessTemplateList";
        public const string GetDocTemplateList = "/ProcessDocTemplateList";
        public const string GetAuthorized = "/AuthCheck";
    }
}