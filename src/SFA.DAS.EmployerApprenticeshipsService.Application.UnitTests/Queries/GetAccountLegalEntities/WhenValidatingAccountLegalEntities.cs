using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetAccountLegalEntities
{
    public class WhenValidatingAccountLegalEntities
    {
        private GetAccountLegalEntitiesValidator _validator;

        [SetUp]
        public void Arange()
        {
            _validator = new GetAccountLegalEntitiesValidator();
        }

        [Test]
        public void ThenFalseIsReturnedIfTheFieldsArentPopulated()
        {
            //Act
            var result = _validator.Validate(new GetAccountLegalEntitiesRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("Id","Account Id has not been supplied"),result.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("UserId","User Id has not been supplied"),result.ValidationDictionary );
        }

        [Test]
        public void ThenFalseIsReturnedIfTheUserIdIsNotAGuid()
        {
            //Act
            var result = _validator.Validate(new GetAccountLegalEntitiesRequest {Id=12345,UserId = "12345"});

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("UserId", "User Id has not been supplied in the correct format"), result.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedIfTheFieldsArePopulated()
        {
            //Act
            var result = _validator.Validate(new GetAccountLegalEntitiesRequest { Id = 12345, UserId = Guid.NewGuid().ToString() });

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}