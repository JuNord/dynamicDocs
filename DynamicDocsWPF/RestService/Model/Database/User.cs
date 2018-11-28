namespace RestService.Model.Database
{
    public class User
    {
        public User()
        {
        }

        public User(string email, string passwordHash, int permissionLevel = 0)
        {
            Email = email;
            Password_Hash = passwordHash;
            PermissionLevel = permissionLevel;
        }

        public string Email { get; set; }
        public string Password_Hash { get; set; }
        public int PermissionLevel { get; set; }
    }
}