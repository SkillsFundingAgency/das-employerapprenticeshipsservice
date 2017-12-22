using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountActivities;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountActivitiesTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountActivitiesQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountActivitiesQueryValidator();
        }

        [Test]
        public void ThenTheRequestIsValidIfAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetAccountActivitiesQuery
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
            var actual = _validator.Validate(new GetAccountActivitiesQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>(nameof(GetAccountActivitiesQuery.AccountId), "Account Id must be supplied"), actual.ValidationDictionary);
        }
    }
}
