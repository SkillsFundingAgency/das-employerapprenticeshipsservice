using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;

using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenICallCreateLegalEntityWhenNotAccountOwner : CreateLegalEntityCommandTests
    {
        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            Validator.Setup(x => x.ValidateAsync(It.IsAny<CreateLegalEntityCommand>())).ReturnsAsync(new ValidationResult() { IsUnauthorized = true });
        }

        [Test]
        public void ThenTheLegalEntityIsNotCreated()
        {
            //Act &
            //Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await CommandHandler.Handle(Command, CancellationToken.None));
        }
    }
}