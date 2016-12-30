using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetCharity;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetCharity
{
    public class WhenIValidateTheRequest
    {
        private GetCharityQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetCharityQueryValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetCharityQueryRequest {RegistrationNumber = 1234});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetCharityQueryRequest());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("RegistrationNumber", "RegistrationNumber has not been supplied"),actual.ValidationDictionary );
        }
    }
}
