using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ProcessNotificationTests
{
    public class WhenIValidateProcessNotificationCommand
    {
        private ProcessNotificationCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new ProcessNotificationCommandValidator();
        }

        [Test]
        public void ThenTheDictionaryIsPopulatedWhenFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new ProcessNotificationCommand());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheCommandIsValidWhenTheParametersAreSupplied()
        {
            //Act
            var actual = _validator.Validate(new ProcessNotificationCommand {Id = 1});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }
    }
}
