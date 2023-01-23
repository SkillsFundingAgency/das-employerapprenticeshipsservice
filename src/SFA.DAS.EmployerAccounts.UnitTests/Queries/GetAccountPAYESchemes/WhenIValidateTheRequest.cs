using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountPAYESchemes
{
    public class WhenIValidateTheRequest
    {
        private GetAccountPayeSchemesQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            new Mock<IMembershipRepository>();

            _validator = new GetAccountPayeSchemesQueryValidator();
        }

        [Test]
        public async Task ThenTheRequestIsNotValidIfAllFieldsArentPopulatedAndTheRepositoryIsNotCalled()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountPayeSchemesQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "Hashed account ID has not been supplied"), actual.ValidationDictionary);
        }
    }
}
