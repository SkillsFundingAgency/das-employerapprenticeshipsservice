using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class FileSystemUserRepository : FileSystemRepository, IUserRepository
    {
        private const string UserDataFileName = "user_data";

        public FileSystemUserRepository()
            : base("Users")
        {
        }
        
        public async Task<User> GetById(string id)
        {
            return await ReadFileById<User>(id);
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