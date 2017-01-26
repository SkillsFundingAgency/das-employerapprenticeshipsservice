using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPayeSchemeByRefTests
{
    public class WhenIValidateTheQuery
    {
        private GetPayeSchemeByRefValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPayeSchemeByRefValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheIdIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeByRefQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("Ref", "Ref has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheIdHasBeenPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeByRefQuery
            {
                Ref = "ABC/123"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
