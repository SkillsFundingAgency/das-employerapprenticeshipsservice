using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountPAYESchemes
{
    public class WhenIValidateTheRequest
    {
        [Test]
        [MoqInlineAutoData(0)]
        [MoqInlineAutoData(-1)]
        [MoqInlineAutoData(-999)]
        public async Task ThenTheRequestIsNotValidIfAllFieldsArentPopulatedAndTheRepositoryIsNotCalled(
            long accountId,
            GetAccountPayeSchemesQuery query,
            GetAccountPayeSchemesQueryValidator validator)
        {
            //Arrange
            query.AccountId = accountId;

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "Account ID has not been supplied"), actual.ValidationDictionary);
        }
    }
}
