namespace WebServer.Model
{
    public class Entry
    {
        public int Entry_ID { get; set; }
        public int Process_ID { get; set; }
        public string FieldName { get; set; }
        public string DataType { get; set; }
        public string Data { get; set; }
        public int PermissionLevel { get; set; }
    }
}