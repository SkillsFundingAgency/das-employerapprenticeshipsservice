using NUnit.Framework;
using System;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateTask;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateTaskTests
{
    [TestFixture]
    public class WhenValidatingCreateTaskCommand
    {
        private CreateTaskCommandValidator _validator;
        private CreateTaskCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateTaskCommandValidator();

            _validCommand = new CreateTaskCommand
            {
                ProviderId = 1L
            };
        }

        [Test]
        public void WhenCommandIsNullIsInvalid()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate(null));
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void WhenProviderIdIsNotPositiveNumberIsInvalid(long providerId)
        {
            _validCommand.ProviderId = providerId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }
    }
}
