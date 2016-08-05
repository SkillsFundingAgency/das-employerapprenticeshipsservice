using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetHmrcEmployerInformationTests
{
    public class WhenValidatingTheRequest
    {
        private GetHmrcEmployerInformationValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetHmrcEmployerInformationValidator();            
        }

        [Test]
        public void ThenMessageIsValidWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetHmrcEmployerInformationQuery {AuthToken = "someValue"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenTheDictionaryIsPopulatedWithEmptyFields()
        {
            //Act
            var actual = _validator.Validate(new GetHmrcEmployerInformationQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AuthToken","AuthToken has not been supplied"),actual.ValidationDictionary );
        }
    }
}
