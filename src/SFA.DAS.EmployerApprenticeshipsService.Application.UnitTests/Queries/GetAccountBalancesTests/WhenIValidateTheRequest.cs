using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountBalancesTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountBalancesValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountBalancesValidator();
            
        }

        [Test]
        public void ThenTheRequestIsInvalidIfTheFieldsAreNotPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountBalancesRequest {AccountIds = new List<long>()});

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AccountIds", "AccountIds has not been supplied"),actual.ValidationDictionary );
        }

        [Test]
        public void ThenTheRequestIsInvalidIfTheFieldIsNull()
        {
            //Act
            var actual = _validator.Validate(new GetAccountBalancesRequest {  });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheRequestIsValidIfTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountBalancesRequest {AccountIds = new List<long> {123} });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
