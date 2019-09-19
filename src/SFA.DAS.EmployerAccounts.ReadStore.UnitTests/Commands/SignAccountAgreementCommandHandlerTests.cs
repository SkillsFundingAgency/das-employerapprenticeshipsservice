using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Commands
{
    internal class SignAccountAgreementCommandHandlerTests : FluentTest<SignAccountAgreementCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenItsNewData_ThenShouldAddNewDocumentToRepositoryWithCorrectValues()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.AccountSignedAgreementsRepository.Verify(x => x.Add(It.Is<AccountSignedAgreement>(p =>
                        p.AccountId == f.AccountId &&
                        p.AgreementType == f.AgreementType &&
                        p.AgreementVersion == f.AgreementVersion &&
                        p.Id != Guid.Empty
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenItsExistingData_ThenShouldNotAddNewDocumentToRepository()
        {
            return TestAsync(f => f.AddExistingAccountSignedAgreement(), f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.AccountSignedAgreementsRepository.Verify(x => x.Add(It.IsAny<AccountSignedAgreement>(), null,
                    It.IsAny<CancellationToken>()), Times.Never));
        }
    }

    internal class SignAccountAgreementCommandHandlerTestsFixture
    {
        public string FirstMessageId = "firstMessageId";
        public string MessageId = "reinvokeMessageId";
        public long AccountId = 333333;
        public int AgreementVersion = 3;
        public string AgreementType = "Levyyyyy";

        public Mock<IAccountSignedAgreementsRepository> AccountSignedAgreementsRepository;
        public List<AccountSignedAgreement> AccountSignedAgreements = new List<AccountSignedAgreement>();

        public SignAccountAgreementCommand Command;
        public SignAccountAgreementCommandHandler Handler;

        public SignAccountAgreementCommandHandlerTestsFixture()
        {
            AccountSignedAgreementsRepository = new Mock<IAccountSignedAgreementsRepository>();
            AccountSignedAgreementsRepository.SetupInMemoryCollection(AccountSignedAgreements);

            Handler = new SignAccountAgreementCommandHandler(AccountSignedAgreementsRepository.Object);

            Command = new SignAccountAgreementCommand(AccountId, AgreementVersion, AgreementType);
        }

        public SignAccountAgreementCommandHandlerTestsFixture AddExistingAccountSignedAgreement()
        {
            AccountSignedAgreements.Add(new AccountSignedAgreement(AccountId, AgreementVersion, AgreementType, Guid.NewGuid()));
            return this;
        }
    }
}