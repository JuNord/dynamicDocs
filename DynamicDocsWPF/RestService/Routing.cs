namespace RestService
{
    public static class Routing
    {
        public const string GetProcess = "/Process?templateName={message}";
        public const string GetDocTemplate = "/Receipt?templateName={message}"; 
        public const string GetPermission = "/Permission?email={message}";
        public const string GetEntryList = "/Entries?instanceId={message}";
        public const string GetInstance = "/Instance?instanceId={message}";
        
        public const string GetProcesses = "/Process";
        public const string GetInstances = "/Instance";
        public const string GetRoles = "/Roles";
        public const string GetArchived = "/Archived";
        public const string GetPending = "/Pending";
        public const string GetReceipts = "/Receipt";
        public const string GetAuthorized = "/AuthCheck";
        public const string GetUserList = "/Users";
        
        public const string UpdateInstance = "/Instance?mode=update";
        public const string AddInstance = "/Instance?mode=add";
        public const string AddEntry = "/Entry?mode=add";
        public const string UpdateEntry = "/Entry?mode=update";
        public const string UpdatePermission = "/Permission";
        public const string AddProcess = "/Process";
        public const string AddReceipt = "/Receipt";
        public const string Register = "/Register";
    }
}