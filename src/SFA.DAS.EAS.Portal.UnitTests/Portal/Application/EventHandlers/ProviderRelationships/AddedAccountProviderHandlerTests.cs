using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.EventHandlers.ProviderRelationships;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.ProviderRelationships.AddedAccountProviderHandlerTestsFixture;
using ApiProvider = SFA.DAS.Apprenticeships.Api.Types.Providers.Provider;
using PortalProvider = SFA.DAS.EAS.Portal.Client.Types.Provider;
using Newtonsoft.Json;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.ProviderRelationships
{
    [TestFixture, Parallelizable]
    public class AddedAccountProviderHandlerTests : FluentTest<AddedAccountProviderHandlerTestsFixture>
    {
        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(Fix.AccountId), f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesContainProvider_ThenAccountDocumentIsSavedWithUpdatedProvider()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsProvider(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderWithLegalButNotPrimaryAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
        {
            return TestAsync(f => f.ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithProviderWithLegalAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderWithoutPrimaryOrLegalAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
        {
            return TestAsync(f => f.ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithProviderWithoutAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiThrows_ThenExceptionIsPropagated()
        {
            return TestExceptionAsync(f => f.ArrangeProviderApiThrowsException(), f => f.Handle(),
                (f, r) => r.Should().Throw<EntityNotFoundException>().WithMessage(Fix.ProviderApiExceptionMessage));
        }       
    }

    public class AddedAccountProviderHandlerTestsFixture : EventHandlerTestsFixture<AddedAccountProviderEvent, AddedAccountProviderHandler>
    {
        public Mock<IAccountDocumentService> MockAccountDocumentService { get; set; }
        public Mock<IProviderApiClient> MockProviderApiClient { get; set; }

        public ApiProvider Provider { get; set; }
        public ApiProvider ExpectedProvider { get; set; }

        public AccountDocument AccountDocument { get; set; }
        public AccountDocument OriginalAccountDocument { get; set; }
        

        public const long AccountId = 123L;
        public const string ProviderApiExceptionMessage = "Test message";

        public AddedAccountProviderHandlerTestsFixture()
        {            
            Provider = Fixture.Create<ApiProvider>();
            Provider.Addresses.RandomElement().ContactType = "PRIMARY";

            MockProviderApiClient = new Mock<IProviderApiClient>();
            MockProviderApiClient.Setup(m => m.GetAsync(Event.ProviderUkprn)).ReturnsAsync(Provider);

            MockAccountDocumentService = new Mock<IAccountDocumentService>();
            MockAccountDocumentService.Setup(m => m.GetOrCreate(AccountId, CancellationToken)).ReturnsAsync(new AccountDocument(AccountId));
            
            Handler = new AddedAccountProviderHandler(MockAccountDocumentService.Object, MockProviderApiClient.Object);

            Event = new AddedAccountProviderEvent(1, AccountId, Event.ProviderUkprn, Guid.NewGuid(), DateTime.UtcNow);
        }

        public override Task Handle()
        {
            ExpectedProvider = Provider.Clone();
            OriginalAccountDocument = AccountDocument.Clone();

            return base.Handle();
        }

        public AddedAccountProviderHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            AccountDocument = JsonConvert.DeserializeObject<AccountDocument>($"{{\"Account\": {{\"Id\": {accountId} }}}}");

            MockAccountDocumentService.Setup(s => s.GetOrCreate(accountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public AddedAccountProviderHandlerTestsFixture ArrangeAccountDocumentContainsProvider()
        {
            //note customization will stay in fixture
            long uniqueUkprnAddition = 0;
            Fixture.Customize<PortalProvider>(p => p.With(pr => pr.Ukprn, () => Event.ProviderUkprn + ++uniqueUkprnAddition));

            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Providers.RandomElement().Ukprn = Event.ProviderUkprn;

            AccountDocument.Account.Id = AccountId;

            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;

            MockAccountDocumentService.Setup(s => s.GetOrCreate(AccountId, CancellationToken)).ReturnsAsync(AccountDocument);

            return this;
        }

        public AddedAccountProviderHandlerTestsFixture ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress()
        {
            ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress();

            Provider.Addresses.RandomElement().ContactType = "LEGAL";

            return this;
        }

        public AddedAccountProviderHandlerTestsFixture ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress()
        {
            foreach (var providerAddress in Provider.Addresses)
            {
                providerAddress.ContactType = "CONTACTTYPE";
            }

            return this;
        }

        public AddedAccountProviderHandlerTestsFixture ArrangeProviderApiThrowsException()
        {
            MockProviderApiClient.Setup(c => c.GetAsync(Event.ProviderUkprn))
                .ThrowsAsync(new EntityNotFoundException(ProviderApiExceptionMessage, null));

            return this;
        }

        public void VerifyAccountDocumentSavedWithProviderWithPrimaryAddress()
        {
            var expectedLegalAddress = ExpectedProvider.Addresses.Single(a => a.ContactType == "PRIMARY");

            VerifyAccountDocumentSavedWithProvider(p =>
            {
                p.Street = expectedLegalAddress.Street;
                p.Town = expectedLegalAddress.Town;
                p.Postcode = expectedLegalAddress.PostCode;
            });
        }

        public void VerifyAccountDocumentSavedWithProviderWithLegalAddress()
        {
            var expectedLegalAddress = ExpectedProvider.Addresses.Single(a => a.ContactType == "LEGAL");

            VerifyAccountDocumentSavedWithProvider(p =>
            {
                p.Street = expectedLegalAddress.Street;
                p.Town = expectedLegalAddress.Town;
                p.Postcode = expectedLegalAddress.PostCode;
            });
        }

        public void VerifyAccountDocumentSavedWithProviderWithoutAddress()
        {
            VerifyAccountDocumentSavedWithProvider();
        }

        public void VerifyAccountDocumentSavedWithProvider(Action<PortalProvider> mutateExpectedProvider = null)
        {
            MockAccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d, mutateExpectedProvider)), It.IsAny<CancellationToken>()), Times.Once);
        }

        public bool AccountIsAsExpected(AccountDocument document, Action<PortalProvider> mutateExpectedProvider = null)
        {
            var expectedAccount = GetExpectedAccount(OriginalEvent.AccountId);
            var expectedProvider = GetExpectedProvider(expectedAccount, OriginalEvent.ProviderUkprn);

            expectedProvider.Name = ExpectedProvider.ProviderName;
            expectedProvider.Email = ExpectedProvider.Email;
            expectedProvider.Phone = ExpectedProvider.Phone;

            mutateExpectedProvider?.Invoke(expectedProvider);

            return AccountIsAsExpected(expectedAccount, document);
        }

        public bool AccountIsAsExpected(Account expectedAccount, AccountDocument savedAccountDocument)
        {
            if (savedAccountDocument?.Account == null)
                return false;

            var (accountIsAsExpected, differences) = savedAccountDocument.Account.IsEqual(expectedAccount);
            if (!accountIsAsExpected)
            {
                TestContext.WriteLine($"Saved account is not as expected: {differences}");
            }

            return accountIsAsExpected;
        }

        public Account GetExpectedAccount(long accountId)
        {
            if (OriginalAccountDocument == null)
            {
                return new Account
                {
                    Id = accountId,
                };
            }
            return OriginalAccountDocument.Account;
        }

        public Provider GetExpectedProvider(Account expectedAccount, long ukprn)
        {
            if (OriginalAccountDocument != null && OriginalAccountDocument.Account.Providers.Any())
                return expectedAccount.Providers.Single(o => o.Ukprn == ukprn);

            var expectedProvider = new Provider
            {
                Ukprn = ukprn
            };
            expectedAccount.Providers.Add(expectedProvider);

            return expectedProvider;
        }
    }
}
