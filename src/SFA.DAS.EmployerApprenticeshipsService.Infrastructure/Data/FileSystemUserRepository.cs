using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class FileSystemUserRepository : FileSystemRepository, IUserRepository
    {
        private const string UserDataFileName = "user_data";

        public FileSystemUserRepository()
            : base("Users")
        {
        }

        public async Task<User> GetUserById(long id)
        {
            var users = await ReadFileById<Users>(UserDataFileName);

            return users.UserList.SingleOrDefault(x => x.Id.Equals(id));
        }

        public async Task<User> GetByUserRef(string id)
        {
            var users = await ReadFileById<Users>(UserDataFileName);

            return users.UserList.SingleOrDefault(x => x.UserRef.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var userFiles = GetDataFiles();

            foreach (var path in userFiles)
            {
                var user = await ReadFile<User>(path);
                if (user.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }

            return null;
        }
        public async Task Create(User registerUser)
        {
            throw new NotImplementedException();
        }
        public async Task Update(User user)
        {
            throw new NotImplementedException();

        }

        public async Task<Users> GetAllUsers()
        {
            return await ReadFileById<Users>(UserDataFileName);
        }
    }
}