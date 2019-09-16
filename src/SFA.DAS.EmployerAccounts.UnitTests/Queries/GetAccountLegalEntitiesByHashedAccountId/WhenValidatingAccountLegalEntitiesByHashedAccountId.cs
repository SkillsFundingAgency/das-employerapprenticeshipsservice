using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntitiesByHashedAccountId
{
    public class WhenValidatingAccountLegalEntitiesByHashedAccountId
    {
        private GetAccountLegalEntitiesByHashedAccountIdValidator _validator;

        [SetUp]
        public void Arange()
        {
            _validator = new GetAccountLegalEntitiesByHashedAccountIdValidator();
        }

        [Test]
        public void ThenFalseIsReturnedIfTheFieldsArentPopulated()
        {
            //Act
            var result = _validator.Validate(new GetAccountLegalEntitiesByHashedAccountIdRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"),result.ValidationDictionary );
        }

        [Test]
        public void ThenTrueIsReturnedIfTheFieldsArePopulated()
        {
            //Act
            var result = _validator.Validate(new GetAccountLegalEntitiesByHashedAccountIdRequest { HashedAccountId = "12345"});

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}