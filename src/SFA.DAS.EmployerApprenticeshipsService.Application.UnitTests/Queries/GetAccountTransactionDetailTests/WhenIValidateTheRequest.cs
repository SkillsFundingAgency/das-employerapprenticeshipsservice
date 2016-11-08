using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransactionDetailTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountTransactionDetailValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountTransactionDetailValidator();
        }

        [Test]
        public void ThenTheResultIsValidWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountTransactionDetailQuery {Id = 3333});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenTheResultIsNotValidWhenFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountTransactionDetailQuery ());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("Id","Id has not been supplied"),actual.ValidationDictionary );
        }
    }
}
