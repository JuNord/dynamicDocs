namespace WebServerWPF
{
    public static class Routing
    {
        public const string GetProcessTemplate = "/ProcessTemplate/{message}";
        public const string GetDocTemplate = "/DocumentTemplate/{message}";
        public const string PostProcessTemplate = "/ProcessTemplate";
        public const string PostDocTemplate = "/DocumentTemplate";
        public const string GetProcessTemplateList = "/ProcessTemplateList";
        public const string GetDocTemplateList = "/DocTemplateList";
        public const string GetAuthorized = "/AuthCheck";
        public const string GetPermissionLevel = "/PermissionLevel/{message}";
        public const string GetEntryList = "/Entries/{message}";
        public const string PostProcessUpdate = "/ProcessUpdate";
        public const string PostProcessInstance = "/ProcessCreate";
        public const string GetProcessInstance = "/ProcessInstance/{message}";
        public const string GetProcessInstanceList = "/ProcessInstanceList";
        public const string GetResponsibilityList = "/ResponsibilityList";
        public const string GetUserList = "/UserList";
        public const string PostEntry = "/Entry";
        public const string PostEntryUpdate = "/EntryUpdate";
        public const string PostPermissionChange = "/Level";
        public const string PostUser = "/User";
    }
}