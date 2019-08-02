using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests.Given_User_Is_Not_In_EOI_Whitelist
{
        public class When_LegalEntity_Is_Created : Given_User_Is_Not_In_EIO_Whitelist
        {
            [Test]
            public async Task Then_LegalEntity_Is_Created_With_Levy_Agreement()
            {
                await CommandHandler.Handle(Command);

                AccountRepository.Verify(
                    m => m.CreateLegalEntityWithAgreement(
                        It.Is<CreateLegalEntityWithAgreementParams>(
                            arg => arg.AgreementType.Equals(AgreementType.Levy))));
            }
        }
    }