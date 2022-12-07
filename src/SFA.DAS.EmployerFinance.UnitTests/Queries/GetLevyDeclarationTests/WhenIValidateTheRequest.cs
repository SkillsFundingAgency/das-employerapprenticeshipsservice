using NUnit.Framework;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetLevyDeclarationTests
{
    public class WhenIValidateTheRequest
    {
        private GetLevyDeclarationValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetLevyDeclarationValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetLevyDeclarationRequest { HashedAccountId = "12587" });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetLevyDeclarationRequest { HashedAccountId = "" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
        }
    }
}
