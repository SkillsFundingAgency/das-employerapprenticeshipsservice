using System;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class AddPayeToNewLegalEntityCommandObjectMother
    {
        public static AddPayeToAccountCommand Create(string externalUserId = "", string hashedId = "JDJFAF123")
        {
            var command = new AddPayeToAccountCommand
            {
                HashedAccountId = hashedId,
                ExternalUserId = string.IsNullOrEmpty(externalUserId) ? Guid.NewGuid().ToString() : externalUserId,
                Empref = "123/ABC",
                RefreshToken = "123GGFFDD",
                AccessToken = "123GGFFDD"
            };

            return command;
        }
    }
}