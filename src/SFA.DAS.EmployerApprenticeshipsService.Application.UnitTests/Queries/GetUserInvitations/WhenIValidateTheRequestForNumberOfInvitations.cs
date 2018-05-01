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
            var actual = _validator.Validate(new GetNumberOfUserInvitationsQuery {ExternalUserId = Guid.NewGuid()});

            //Assert
            Assert.IsTrue(actual.IsValid());
            
        }

        [Test]
        public void ThenIfTheUserIdIsAnEmptyGuidItIsNotValid()
        {
            //Act
            var actual = _validator.Validate(new GetNumberOfUserInvitationsQuery {ExternalUserId = Guid.Empty});
            
            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenIfFieldsArentPopulatedTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetNumberOfUserInvitationsQuery { ExternalUserId = Guid.Empty });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }

            
    }
}
