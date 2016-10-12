using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateApprenticeship;
using SFA.DAS.Commitments.Api.Types;
using Moq;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeshipTests
{
    [TestFixture]
    public sealed class WhenValidatingCreateApprenticeshipCommand
    {
        private CreateApprenticeshipCommand _validCommand;
        private CreateApprenticeshipCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _handler = new CreateApprenticeshipCommandHandler(Mock.Of<ICommitmentsApi>());
            _validCommand = new CreateApprenticeshipCommand
            {
                AccountId = 123,
                Apprenticeship = new Apprenticeship()
            };
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void AccountIdMustBeGreaterThanZeroElseExceptionThrown(long accountId)
        {
            var command = new CreateApprenticeshipCommand { AccountId = accountId, Apprenticeship = new Apprenticeship { CommitmentId = 123 } };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void CommitmentIdMustBeGreaterThanZeroElseExceptionThrown(long commitmentId)
        {
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = commitmentId } };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [Test]
        public void OnlyValidCommitmentIdAndAccountIdNeedsToBePopulated()
        {
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = 321 } };

            Assert.DoesNotThrowAsync(() => _handler.Handle(command));
        }

        [Test]
        public void ULNMustBeNumericAnd10DigitsInLength()
        {
            var apprenticeship = new Apprenticeship { CommitmentId = 123, ULN = "0001234567" };
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = apprenticeship };

            Assert.DoesNotThrowAsync(() =>_handler.Handle(command));
        }

        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        [TestCase("")]
        public void ULNThatIsNotNumericAnd10DigitsInLengthThrowsException(string uln)
        {
            var apprenticeship = new Apprenticeship { CommitmentId = 123, ULN = uln };
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = apprenticeship };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        public void ULNThatStartsWithAZeroThrowsException()
        {
            var apprenticeship = new Apprenticeship { CommitmentId = 123, ULN = "0123456789" };
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = apprenticeship };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [TestCase(123.12)]
        [TestCase(123.1)]
        [TestCase(123.0)]
        [TestCase(123)]
        [TestCase(123.000)]
        public void CostMustBeNumericAndHave2DecimalPlaces(decimal cost)
        {
            var apprenticeship = new Apprenticeship { CommitmentId = 123, Cost = cost };
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = apprenticeship };

            Assert.DoesNotThrowAsync(() => _handler.Handle(command));
        }

        [TestCase(123.1232)]
        [TestCase(0.001)]
        public void CostThatIsNotAMax2DecimalPlacesThrowsException(decimal cost)
        {
            var apprenticeship = new Apprenticeship { CommitmentId = 123, Cost = cost };
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = apprenticeship };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [TestCase(0)]
        [TestCase(-0)]
        [TestCase(-123.12)]
        [TestCase(-123)]
        [TestCase(-123.1232)]
        [TestCase(-0.001)]
        public void CostThatIsZeroOrNegativeNumberThrowsException(decimal cost)
        {
            var apprenticeship = new Apprenticeship { CommitmentId = 123, Cost = cost };
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = apprenticeship };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }
    }
}
