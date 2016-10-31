using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserInvitations
{
    public class WhenIValidateTheRequestForNumberOfInvitations
    {
        private GetNumberOfUserInvitationsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetNumberOfUserInvitationsValidator();   
        }

        [Test]
        public void ThenIfAllFieldsArePopulatedItIsValid()
        {
            //Act
            var actual = _validator.Validate(new GetNumberOfUserInvitationsQuery {UserId = Guid.NewGuid().ToString()});

            //Assert
            Assert.IsTrue(actual.IsValid());
            
        }

        [Test]
        public void ThenIfTheUserIdIsNotAGuidItIsNotValid()
        {
            //Act
            var actual = _validator.Validate(new GetNumberOfUserInvitationsQuery { UserId = "someId" });
            
            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId is not in the correct format"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenIfFieldsArentPopulatedTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetNumberOfUserInvitationsQuery { UserId = "" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
        }

            
    }
}
