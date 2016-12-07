using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateEnglishFractionCalculationDateCommandTests
{
    public class WhenIValidateTheCommand
    {
        private CreateEnglishFractionCalculationDateCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateEnglishFractionCalculationDateCommandValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new CreateEnglishFractionCalculationDateCommand
            {
                DateCalculated = new DateTime(2016, 01, 01)
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryIsPopulatedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new CreateEnglishFractionCalculationDateCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("DateCalculated", "DateCalculated has not been supplied"), actual.ValidationDictionary);
        }
    }
}
