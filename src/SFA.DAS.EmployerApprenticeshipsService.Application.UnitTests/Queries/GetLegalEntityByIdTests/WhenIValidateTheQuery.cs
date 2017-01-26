using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityById;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLegalEntityByIdTests
{
    public class WhenIValidateTheQuery
    {
        private GetLegalEntityByIdValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetLegalEntityByIdValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheIdIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetLegalEntityByIdQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("Id", "Id has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheIdHasBeenPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetLegalEntityByIdQuery
            {
                Id = 123
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
