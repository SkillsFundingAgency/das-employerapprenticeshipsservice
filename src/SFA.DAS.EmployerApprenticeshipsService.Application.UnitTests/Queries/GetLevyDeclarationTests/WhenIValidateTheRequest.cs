using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetLevyDeclarationTests
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
            var actual = _validator.Validate(new GetLevyDeclarationRequest {AccountId = 12587});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetLevyDeclarationRequest { AccountId = 0 });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
        }
    }
}
