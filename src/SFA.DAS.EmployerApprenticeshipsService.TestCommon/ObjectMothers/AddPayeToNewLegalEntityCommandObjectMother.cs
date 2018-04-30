using System;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class AddPayeToNewLegalEntityCommandObjectMother
    {
        public static AddPayeToAccountCommand Create(Guid externalUserId, string hashedId = "JDJFAF123")
        {
            var command = new AddPayeToAccountCommand
            {
                HashedAccountId = hashedId,
                ExternalUserId = externalUserId == Guid.Empty ? Guid.NewGuid() : externalUserId,
                Empref = "123/ABC",
                RefreshToken = "123GGFFDD",
                AccessToken = "123GGFFDD",
                EmprefName = "Paye Scheme 1"
            };

            return command;
        }
    }
}