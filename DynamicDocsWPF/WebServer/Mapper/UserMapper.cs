using WebServer.Model;

namespace WebServer.Mapper
{
    public class UserMapper : IMapper<User>
    {
        public User Map(string[] dataSet)
        {
            var user = new User()
            {
                User_ID = int.Parse(dataSet[0]),
                Email = dataSet[1],
                Password_Hash = dataSet[2],
                PermissionLevel = int.Parse(dataSet[3])
            };

            return user;
        }

        public bool TryMap(string[] dataSet, out User result)
        {
            throw new System.NotImplementedException();
        }
    }
}