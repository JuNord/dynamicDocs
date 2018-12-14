using RestService.Model.Database;

namespace RestService
{
    public class DataMessage
    {
        public User User { get; set; }
        public string Content { get; set; }
        public DataType DataType { get; set; }
    }
}