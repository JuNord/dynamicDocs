namespace RestService
{
    public class FileMessage
    {
        public bool ForceOverWrite { get; set; }
        public string ID { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public FileType FileType { get; set; }
    }
}