using RestService.Model.Database;

namespace WebServerWPF.RestDTOs
{
    public class RequestPostUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}