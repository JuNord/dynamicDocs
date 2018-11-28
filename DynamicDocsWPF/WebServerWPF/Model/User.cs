namespace WebServerWPF.Model
{
    public class User
    {
        public int User_ID { get; set; }
        public string Email { get; set; }
        public string Password_Hash { get; set; }
        public int PermissionLevel { get; set; }
    }
}