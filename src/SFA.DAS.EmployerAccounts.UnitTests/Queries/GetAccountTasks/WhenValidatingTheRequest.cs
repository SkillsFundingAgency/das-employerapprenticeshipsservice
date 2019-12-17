using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountTasks
{
    public class WhenValidatingTheRequest
    {
        private GetAccountTasksQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountTasksQueryValidator();
        }

        [Test]
        public void ThenMessageIsValidWhenAllFieldsArePopulated()
        {
            //Arrange
            var query = new GetAccountTasksQuery
            {
                AccountId = 1,
                ExternalUserId = Guid.NewGuid().ToString(),
            };

            //Act
            var actual = _validator.Validate(query);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenMessageIsInvalidWhenQueryIsNull()
        {
            //Arrange
            GetAccountTasksQuery query = null;           

            //Act
            var actual = _validator.Validate(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetAccountTasksQuery), "Message must be supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenMessageIsValidWhenRequiredFieldsAreNotPopulated()
        {
            //Arrange
            var query = new GetAccountTasksQuery();

            //Act
            var actual = _validator.Validate(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetAccountTasksQuery.AccountId), "Account id must be supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetAccountTasksQuery.ExternalUserId), "External user id must be supplied"), actual.ValidationDictionary);
        }
    }
}
