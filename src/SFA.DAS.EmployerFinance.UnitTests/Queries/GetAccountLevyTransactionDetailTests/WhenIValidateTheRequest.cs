using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountProviderPayments;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetAccountLevyTransactionDetailTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountProviderPaymentsByDateRangeValidator _validator;
        
        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountProviderPaymentsByDateRangeValidator();
        }

        [Test]
        public async Task ThenTheResultIsValidWhenAllFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountProviderPaymentsByDateRangeQuery
            {
                AccountId = 1,
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(-2)
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenTheResultIsNotValidWhenFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountProviderPaymentsByDateRangeQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AccountId","Account ID has not been supplied"), actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("FromDate","From date has not been supplied"), actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("ToDate","To date has not been supplied"), actual.ValidationDictionary );
        }
    }
}
