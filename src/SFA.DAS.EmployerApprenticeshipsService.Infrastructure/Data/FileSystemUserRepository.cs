using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class FileSystemUserRepository : FileSystemRepository, IUserRepository
    {
        private const string UserDataFileName = "user_data";

        public FileSystemUserRepository()
            : base("Users")
        {
        }

        public Task Upsert(User user)
        {
            throw new NotImplementedException();
        }
    }
}