namespace RestService.Model.Database
{
    public class User
    {
        public User()
        {
        }

        public User(string email, string password, int permissionLevel = 0)
        {
            Email = email;
            Password = password;
            PermissionLevel = permissionLevel;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public int PermissionLevel { get; set; }
    }
}