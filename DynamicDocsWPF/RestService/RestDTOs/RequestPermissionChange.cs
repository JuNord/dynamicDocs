namespace RestService.RestDTOs
{
    public class RequestPermissionChange
    {
        public string Email { get; set; }
        public int PermissionLevel { get; set; }
        public string Role { get; set; }
    }
}