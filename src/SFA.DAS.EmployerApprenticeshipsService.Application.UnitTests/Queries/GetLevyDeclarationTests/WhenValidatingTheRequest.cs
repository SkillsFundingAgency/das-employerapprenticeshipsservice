using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Queries.GetLevyDeclarationTests
{
    public class WhenValidatingTheRequest
    {
        private GetLevyDeclarationQueryValidator _getLevyDeclarationQueryValidator;

        [SetUp]
        public void Arrange()
        {
            _getLevyDeclarationQueryValidator = new GetLevyDeclarationQueryValidator();
        }

        [Test]
        public void ThenTheDicitionaryIsPopulatedWhenTheIdIsMissing()
        {
            //Act
            var actual = _getLevyDeclarationQueryValidator.Validate(new GetLevyDeclarationQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.Contains(new KeyValuePair<string, string>("Id", "The Id field has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTheDictionaryIsNotPopulatedIfAllFieldsAreSupplied()
        {
            //Act
            var actual = _getLevyDeclarationQueryValidator.Validate(new GetLevyDeclarationQuery {Id = "123456"});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }
    }
}
