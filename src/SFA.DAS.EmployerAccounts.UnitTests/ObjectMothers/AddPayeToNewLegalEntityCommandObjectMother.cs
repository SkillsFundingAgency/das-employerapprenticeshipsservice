using System;
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;

namespace SFA.DAS.EmployerAccounts.UnitTests.ObjectMothers
{
    public static class AddPayeToNewLegalEntityCommandObjectMother
    {
        public static AddPayeToAccountCommand Create(string externalUserId = "", string hashedAccountId = "JDJFAF123", string aorn = "")
        {
            var command = new AddPayeToAccountCommand
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = string.IsNullOrEmpty(externalUserId) ? Guid.NewGuid().ToString() : externalUserId,
                Empref = "123/ABC",
                RefreshToken = "123GGFFDD",
                AccessToken = "123GGFFDD",
                EmprefName = "Paye Scheme 1",
                Aorn = aorn
            };

            return command;
        }
    }
}