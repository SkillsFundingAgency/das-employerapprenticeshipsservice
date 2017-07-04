using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetOrganisationTests
{
    public class WhenIValidateTheQuery
    {
        private GetOrganisationsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetOrganisationsValidator();
        }

        [Test]
        public void ThenTrueIsRetrunedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationsRequest {SearchTerm = "Test Company"});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationsRequest { SearchTerm = "" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("SearchTerm","SearchTerm has not been supplied"), actual.ValidationDictionary);
        }
    }
}
