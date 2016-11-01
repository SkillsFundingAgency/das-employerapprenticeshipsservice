using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetHmrcLevyDeclarationTests
{
    public class WhenValidatingTheRequest
    {
        private GetHMRCLevyDeclarationQueryValidator _getHMRCLevyDeclarationQueryValidator;

        [SetUp]
        public void Arrange()
        {
            _getHMRCLevyDeclarationQueryValidator = new GetHMRCLevyDeclarationQueryValidator();
        }

        [Test]
        public void ThenTheDicitionaryIsPopulatedWhenTheIdIsMissing()
        {
            //Act
            var actual = _getHMRCLevyDeclarationQueryValidator.Validate(new GetHMRCLevyDeclarationQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.Contains(new KeyValuePair<string, string>("EmpRef", "The EmpRef field has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTheDictionaryIsNotPopulatedIfAllFieldsAreSupplied()
        {
            //Act
            var actual = _getHMRCLevyDeclarationQueryValidator.Validate(new GetHMRCLevyDeclarationQuery {EmpRef = "123456"});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }
    }
}
