using System;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateCommitment;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateCommitmentTests
{
    [TestFixture]
    public class WhenValidatingTheCreateCommitmentCommand
    {
        private CreateCommitmentCommandValidator _validator;
        private CreateCommitmentCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateCommitmentCommandValidator();

            _validCommand = new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    LegalEntityCode = "001",
                    LegalEntityName = "Test Legal Entity Name",
                    EmployerAccountId = 2,
                    EmployerAccountName = "Test Account Name"
                }
            };
        }

        [Test]
        public void WhenCommandIsNullIsInvalid()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate(null));
        }

        [Test]
        public void WhenCommitmentIsNullIsInvalid()
        {
            var result = _validator.Validate(new CreateCommitmentCommand { Commitment = null });

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void WhenEmployerAccountIdIsNotPositiveNumberIsInvalid(long accoundId)
        {
            _validCommand.Commitment.EmployerAccountId = accoundId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase("")]
        [TestCase("   ")]
        public void WhenLegalEntityIdIsNotPositiveNumberIsInvalid(string legalEntityId)
        {
            _validCommand.Commitment.LegalEntityCode = legalEntityId;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void WhenLegalEntityNameIsNotSetIsInvalid(string legalEntityName)
        {
            _validCommand.Commitment.LegalEntityName = legalEntityName;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void WhenProviderIdIsValidAndNoProviderNameSetIsInvalid(string providerName)
        {
            _validCommand.Commitment.ProviderId = 2;
            _validCommand.Commitment.ProviderName = providerName;
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void WhenProviderNameIsValidAndProviderIdInNoIsInvalid()
        {
            _validCommand.Commitment.ProviderId = -1;
            _validCommand.Commitment.ProviderName = "Test Provider Name";
            var result = _validator.Validate(_validCommand);

            Assert.IsFalse(result.IsValid());
        }
    }
}
