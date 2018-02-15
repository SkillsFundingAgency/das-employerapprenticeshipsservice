using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.AccountFixupTool.Work
{
    public class CreateExternalHashedAccountIdJob : IAdminJob
    {
        public async Task Fix()
        {
            try
            {
                var config = new EmployerApprenticeshipsServiceConfiguration
                {
                    DatabaseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString,
                    ExternalHashstring = System.Configuration.ConfigurationManager.AppSettings["ExternalHashstring"],
                    ExternalAllowedHashstringCharacters = System.Configuration.ConfigurationManager.AppSettings["ExternalAllowedHashstringCharacters"],
                };

                var accountRepo = new AccountRepository(
                    config, new NullLogger());

                var externalHasher = new HashingService.HashingService(config.ExternalAllowedHashstringCharacters,
                    config.ExternalHashstring);

                foreach (var id in await accountRepo.GetAccountsMissingExternalHashId())
                {
                    var externalHash = externalHasher.HashValue(id);
                    await accountRepo.SetExternalHashedId(externalHash, id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
