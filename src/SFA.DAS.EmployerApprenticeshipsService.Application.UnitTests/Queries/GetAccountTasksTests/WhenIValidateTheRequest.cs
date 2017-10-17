using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountTasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTasksTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountTasksQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountTasksQueryValidator();
        }

        [Test]
        public void ThenTheRequestIsValidIfAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountTasksQuery
            {
                AccountId = 123
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenTheRequestIsNotValidIfAllFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountTasksQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetAccountTasksQuery.AccountId), "Account Id must be supplied"), actual.ValidationDictionary);
        }
    }
}
