using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.CommonSteps
{
    public static class AccountCreationSteps
    {
        public static long CreateDasAccount(SignInUserModel user, IAccountRepository accountRepository, IUserRepository userRepository)
        {
            var userRecord = userRepository.GetById(user.UserId).Result;
            
            return accountRepository.CreateAccount(userRecord.Id, "123456", "TestCompany", "Test Address", new DateTime(2016, 01, 01), "123/ABC123", Guid.NewGuid().ToString(), Guid.NewGuid().ToString()).Result;
            
        }
    }
}
