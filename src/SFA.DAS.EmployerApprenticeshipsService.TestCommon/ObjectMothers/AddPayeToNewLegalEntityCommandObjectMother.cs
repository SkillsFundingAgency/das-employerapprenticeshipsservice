﻿using System;
using SFA.DAS.EAS.Application.Commands.AddPayeToNewLegalEntity;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class AddPayeToNewLegalEntityCommandObjectMother
    {
        public static AddPayeToNewLegalEntityCommand Create(string externalUserId = "", string hashedId = "JDJFAF123")
        {
            var command = new AddPayeToNewLegalEntityCommand
            {
                HashedId = hashedId,
                ExternalUserId = string.IsNullOrEmpty(externalUserId) ? Guid.NewGuid().ToString() : externalUserId,
                Empref = "123/ABC",
                RefreshToken = "123GGFFDD",
                AccessToken = "123GGFFDD",
                LegalEntityCode = "ABC123",
                LegalEntityAddress = "My Test Address",
                LegalEntityName = "Entity Name",
                LegalEntityDate = new DateTime(2016, 02, 12)
            };

            return command;
        }
    }
}