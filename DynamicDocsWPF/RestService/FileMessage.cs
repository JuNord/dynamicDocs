using RestService.Model.Database;

namespace RestService
{
    public class FileMessage
    {
        public FileMessage()
        {
        }

        public FileMessage(User user, FileType type, string id, string fileName, string content, bool forceOverWrite)
        {
            User = user;
            FileType = type;
            ID = id;
            FileName = fileName;
            Content = content;
            ForceOverWrite = forceOverWrite;
        }

        public User User { get; set; }
        public bool ForceOverWrite { get; set; }
        public string ID { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public FileType FileType { get; set; }
    }
}