namespace WebServerWPF
{
    public static class Routing
    {
        public const string GetProcessTemplate = "/ProcessTemplate/{request}";
        public const string GetDocTemplate = "/DocumentTemplate/{request}";
        public const string PostProcessTemplate = "/ProcessTemplate";
        public const string PostDocTemplate = "/DocumentTemplate";
        public const string GetProcessTemplateList = "/ProcessTemplateList";
        public const string GetDocTemplateList = "/DocTemplateList";
        public const string GetAuthorized = "/AuthCheck";
        public const string GetPermissionLevel = "/PermissionLevel";
        public const string GetEntryList = "/Entries/{request}";
        public const string PostProcessUpdate = "/ProcessUpdate";
        public const string PostProcessInstance = "/ProcessCreate";
        public const string PostEntry = "/Entry";
        public const string PostUser = "/User";
    }
}