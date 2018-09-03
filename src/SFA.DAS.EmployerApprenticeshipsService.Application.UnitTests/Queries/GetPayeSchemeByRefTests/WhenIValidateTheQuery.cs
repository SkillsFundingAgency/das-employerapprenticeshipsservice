using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPayeSchemeByRefTests
{
    /// <summary>
    ///  AML-2454: Copy to finance
    /// </summary>
    public class WhenIValidateTheQuery
    {
        private GetPayeSchemeByRefValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPayeSchemeByRefValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheHashedIdIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeByRefQuery { Ref = "ABC/123" });

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("Ref", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheRefIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeByRefQuery { HashedAccountId = "ABC123" });

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("Ref", "PayeSchemeRef has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheIdHasBeenPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeByRefQuery
            {
                HashedAccountId = "ABC123",
                Ref = "ABC/123"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
