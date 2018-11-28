namespace RestService
{
    public class FileMessage
    {
        public bool ForceOverWrite { get; set; }
        public string ID { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public FileType FileType { get; set; }

        public FileMessage()
        {
            
        }
        public FileMessage(FileType type, string id, string fileName, string content, bool forceOverWrite)
        {
            ForceOverWrite = forceOverWrite;
            ID = id;
            FileName = fileName;
            Content = content;
            ForceOverWrite = forceOverWrite;
        }
    }
}