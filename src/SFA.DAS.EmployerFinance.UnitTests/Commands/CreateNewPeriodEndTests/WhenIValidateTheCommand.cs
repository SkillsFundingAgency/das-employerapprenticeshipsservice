using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.CreateNewPeriodEndTests
{
    public class WhenIValidateTheCommand
    {
        private CreateNewPeriodEndCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateNewPeriodEndCommandValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new CreateNewPeriodEndCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("NewPeriodEnd","NewPeriodEnd has not been supplied"),actual.ValidationDictionary );
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new CreateNewPeriodEndCommand {NewPeriodEnd = new PeriodEnd() });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
