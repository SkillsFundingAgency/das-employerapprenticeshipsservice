using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.DismissMonthlyTaskReminderTests
{
    public class WhenTheCommandIsValidated
    {
        private DismissMonthlyTaskReminderCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new DismissMonthlyTaskReminderCommandValidator();
        }

        [Test]
        public void ThenItShouldPassValidation()
        {
            //Arrange
            var command = new DismissMonthlyTaskReminderCommand
            {
                HashedAccountId = "ABC123",
                ExternalUserId = "DEF456",
                TaskType = TaskType.LevyDeclarationDue
            };
            
            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenItShouldFailValidationIfInValid()
        {
            //Arrange
            var command = new DismissMonthlyTaskReminderCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.HashedAccountId)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.ExternalUserId)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.TaskType)));
        }
    }
}
