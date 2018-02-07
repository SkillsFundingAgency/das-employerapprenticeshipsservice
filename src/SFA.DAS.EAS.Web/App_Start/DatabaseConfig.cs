using System.Data.Entity;
using SFA.DAS.EAS.Application.Data;

namespace SFA.DAS.EAS.Web
{
    public class DatabaseConfig
    {
        public static void Configure()
        {
            Database.SetInitializer<EmployerAccountDbContext>(null);
        }
    }
}