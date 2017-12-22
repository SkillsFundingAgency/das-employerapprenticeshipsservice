using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountLatestActivities;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountLatestActivitiesTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountLatestActivitiesQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountLatestActivitiesQueryValidator();
        }

        [Test]
        public void ThenTheRequestIsValidIfAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountLatestActivitiesQuery
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
            var actual = _validator.Validate(new GetAccountLatestActivitiesQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetAccountLatestActivitiesQuery.AccountId), "Account Id must be supplied"), actual.ValidationDictionary);
        }
    }
}
