using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionsTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountTransactionsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountTransactionsValidator();

        }

        [Test]
        public void ThenTheRequestIsInvalidIfTheFieldsAreNotPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountTransactionsRequest {  });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
        }
        
        [Test]
        public void ThenTheRequestIsValidIfTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountTransactionsRequest {AccountId = 34543511});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
