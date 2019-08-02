using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests.Given_User_Is_Not_In_EOI_Whitelist.But_There_Is_Already_An_EOI_Agreement
{
    [ExcludeFromCodeCoverage]
    public class When_LegalEntity_Is_Created
    :But_There_Is_Already_An_EOI_Agreement
    {
        [Test]
        public async Task Then_LegalEntity_Is_Created_With_ExpressionOfInterest_Agreement()
        {
            await CommandHandler.Handle(Command);

            AccountRepository.Verify(
                m => m.CreateLegalEntityWithAgreement(
                    It.Is<CreateLegalEntityWithAgreementParams>(
                        arg => arg.AgreementType.Equals(AgreementType.NoneLevyExpressionOfInterest))));
        }
    }
}