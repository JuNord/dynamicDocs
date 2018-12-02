using RestService.Model.Database;

namespace WebServerWPF.RestDTOs
{
    public class RequestPostProcessUpdate
    {
        public int Id { get; set; }
        public bool Declined { get; set; }
        public bool Locks { get; set; }
    }
}