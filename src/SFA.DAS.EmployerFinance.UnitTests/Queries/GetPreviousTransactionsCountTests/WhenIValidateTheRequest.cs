using NUnit.Framework;
using SFA.DAS.EmployerFinance.Queries.GetPreviousTransactionsCount;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetPreviousTransactionsCountTests
{
    public class WhenIValidateTheRequest
    {
        private GetPreviousTransactionsCountRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPreviousTransactionsCountRequestValidator();
        }

        [Test]
        public async Task ThenTheValidationShouldPassIfUsingCorrectValues()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetPreviousTransactionsCountRequest
            {
                AccountId = 1,
                FromDate = DateTime.Now
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenTheValidationShouldFailIfUsingIncorrectValues()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetPreviousTransactionsCountRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetPreviousTransactionsCountRequest.AccountId), "AccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetPreviousTransactionsCountRequest.FromDate), "FromDate has not been supplied"), actual.ValidationDictionary);
        }
    }
}
