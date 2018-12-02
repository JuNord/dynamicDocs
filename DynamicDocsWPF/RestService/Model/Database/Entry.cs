namespace RestService.Model.Database
{
    public class Entry
    {
        public int EntryId { get; set; }
        public int InstanceId { get; set; }
        public string FieldName { get; set; }
        public string Data { get; set; }
    }
}