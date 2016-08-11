using System;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
{
    public static class AddPayeToNewLegalEntityCommandObjectMother
    {
        public static AddPayeToNewLegalEntityCommand Create()
        {
            var command = new AddPayeToNewLegalEntityCommand
            {
                AccountId = 123456,
                ExternalUserId = Guid.NewGuid().ToString(),
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