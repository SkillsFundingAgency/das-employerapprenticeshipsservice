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
        public static void CreateDasAccount(SignInUserModel user, EmployerAccountOrchestrator orchestrator)
        {

            orchestrator.CreateAccount(new CreateAccountModel
            {
                UserId = user.UserId,
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                CompanyDateOfIncorporation = new DateTime(2016,01,01),
                EmployerRef = "123/ABC",
                CompanyName = "Test Company",
                CompanyNumber = "123456TGB",
                CompanyRegisteredAddress = "Address Line 1"
            }).Wait();
            
            
        }
    }
}
