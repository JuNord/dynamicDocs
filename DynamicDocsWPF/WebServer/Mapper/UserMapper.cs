using WebServer.Model;

namespace WebServer.Mapper
{
    public class UserMapper : IMapper<User>
    {
        public User Map(string[] dataset)
        {
            var user = new User()
            {
                User_ID = int.Parse(dataset[0]),
                Email = dataset[1],
                Password_Hash = dataset[2],
                PermissionLevel = int.Parse(dataset[3])
            };

            return user;
        }

        public bool TryMap(string[] dataset, out User result)
        {
            throw new System.NotImplementedException();
        }
    }
}