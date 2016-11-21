using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetApprenticeshipDetails;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetApprenticeshipDetails
{
    public class WhenIValidateTheRequest
    {
        private GetApprenticeshipDetailsQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetApprenticeshipDetailsQueryValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulatedAndTheMemberIsPartOfTheAccount()
        {
            //Act
            var actual = _validator.Validate(new GetApprenticeshipDetailsQuery
                {
                    ProviderId = 123,
                    ApprenticeshipId = 1,
                });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheValidtionDictionaryIsPopulatedWhenFieldsArentSupplied()
        {
            //Act
            var actual = _validator.Validate(new GetApprenticeshipDetailsQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("ProviderId", "Provider ID has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("ApprenticeshipId", "Apprenticeship ID has not been supplied"), actual.ValidationDictionary);
        }
    }
}
