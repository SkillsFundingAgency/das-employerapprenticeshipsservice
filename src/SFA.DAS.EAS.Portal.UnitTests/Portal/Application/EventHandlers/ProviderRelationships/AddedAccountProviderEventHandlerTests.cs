using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.EAS.Portal.Application.EventHandlers.ProviderRelationships;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.ProviderRelationships.AddedAccountProviderEventHandlerTestsFixture;
using ApiProvider = SFA.DAS.Apprenticeships.Api.Types.Providers.Provider;
using PortalProvider = SFA.DAS.EAS.Portal.Client.Types.Provider;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers.ProviderRelationships
{
    [TestFixture, Parallelizable]
    public class AddedAccountProviderEventHandlerTests : FluentTest<AddedAccountProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist(Fix.AccountId),
                f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(Fix.AccountId),
                f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
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
            return TestAsync(f => f.ArrangeEmptyAccountDocument(Fix.AccountId)
                    .ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithProviderWithLegalAddress());
        }
        
        [Test]
        public Task Execute_WhenProviderApiReturnsProviderWithoutPrimaryOrLegalAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(Fix.AccountId)
                    .ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress(), f => f.Handle(),
                f => f.VerifyAccountDocumentSavedWithProviderWithoutAddress());
        }
        
        [Test]
        public Task Execute_WhenProviderApiThrows_ThenExceptionIsPropagated()
        {
            return TestExceptionAsync(f => f.ArrangeAccountDoesNotExist(Fix.AccountId)
                    .ArrangeProviderApiThrowsException(),
                f => f.Handle(), 
                (f, r) => r.Should().Throw<EntityNotFoundException>().WithMessage(
                    Fix.ProviderApiExceptionMessage));
        }
    }

    public class AddedAccountProviderEventHandlerTestsFixture
    {
        public AccountEventHandlerTestHelper<AddedAccountProviderEvent, AddedAccountProviderEventHandler> Helper { get; set; }

        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
        public ApiProvider Provider { get; set; }
        public ApiProvider ExpectedProvider { get; set; }
        public const long AccountId = 123L;
        public const string ProviderApiExceptionMessage = "Test message";
        
        public AddedAccountProviderEventHandlerTestsFixture()
        {
            Helper = new AccountEventHandlerTestHelper<AddedAccountProviderEvent, AddedAccountProviderEventHandler>(false);
            
            Provider = Helper.Fixture.Create<ApiProvider>();
            Provider.Addresses.RandomElement().ContactType = "PRIMARY";

            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Helper.Message.ProviderUkprn))
                .ReturnsAsync(Provider);
            
            Helper.Handler = new AddedAccountProviderEventHandler(
                Helper.AccountDocumentService.Object,
                Helper.Logger.Object,
                ProviderApiClient.Object);
            
            Helper.Message = new AddedAccountProviderEvent(
                1, AccountId, Helper.Message.ProviderUkprn,
                Guid.NewGuid(), DateTime.UtcNow);
        }
        
        public AddedAccountProviderEventHandlerTestsFixture ArrangeAccountDoesNotExist(long accountId)
        {
            Helper.ArrangeAccountDoesNotExist(accountId);

            return this;
        }
        
        public AddedAccountProviderEventHandlerTestsFixture ArrangeEmptyAccountDocument(long accountId)
        {
            Helper.ArrangeEmptyAccountDocument(accountId);

            return this;
        }

        public AddedAccountProviderEventHandlerTestsFixture ArrangeAccountDocumentContainsProvider()
        {
            //note customization will stay in fixture
            long uniqueUkprnAddition = 0;
            Helper.Fixture.Customize<PortalProvider>(
                p => p.With(pr => pr.Ukprn,
                    () => Helper.Message.ProviderUkprn + ++uniqueUkprnAddition));
            
            Helper.AccountDocument = Helper.Fixture.Create<AccountDocument>();

            Helper.AccountDocument.Account.Providers.RandomElement().Ukprn = 
                Helper.Message.ProviderUkprn;

            Helper.AccountDocument.Account.Id = AccountId;
            
            Helper.AccountDocument.Deleted = null;
            Helper.AccountDocument.Account.Deleted = null;
            
            Helper.AccountDocumentService.Setup(
                s => s.GetOrCreate(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Helper.AccountDocument);
            
            return this;
        }

        public AddedAccountProviderEventHandlerTestsFixture ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress()
        {
            ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress();
            
            Provider.Addresses.RandomElement().ContactType = "LEGAL";

            return this;
        }
        
        public AddedAccountProviderEventHandlerTestsFixture ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress()
        {
            foreach (var providerAddress in Provider.Addresses)
            {
                providerAddress.ContactType = "CONTACTTYPE";
            }

            return this;
        }
        
        public AddedAccountProviderEventHandlerTestsFixture ArrangeProviderApiThrowsException()
        {
            ProviderApiClient.Setup(c => c.GetAsync(Helper.Message.ProviderUkprn))
                .ThrowsAsync(new EntityNotFoundException(ProviderApiExceptionMessage, null));
            
            return this;
        }
        
        public Task Handle()
        {
            ExpectedProvider = Provider.Clone();

            return Helper.Handle();
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
            Helper.AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(
                    d => AccountIsAsExpected(d, mutateExpectedProvider)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        public bool AccountIsAsExpected(AccountDocument document, Action<PortalProvider> mutateExpectedProvider = null)
        {
            var expectedAccount = Helper.GetExpectedAccount(Helper.OriginalMessage.AccountId);
            var expectedProvider = GetExpectedProvider(expectedAccount, Helper.OriginalMessage.ProviderUkprn);

            expectedProvider.Name = ExpectedProvider.ProviderName;
            expectedProvider.Email = ExpectedProvider.Email;
            expectedProvider.Phone = ExpectedProvider.Phone;

            mutateExpectedProvider?.Invoke(expectedProvider);

            return Helper.AccountIsAsExpected(expectedAccount, document);
        }
        
        public Provider GetExpectedProvider(Account expectedAccount, long ukprn)
        {
            if (Helper.OriginalAccountDocument != null
                && Helper.OriginalAccountDocument.Account.Providers.Any())
            {
                return expectedAccount.Providers.Single(o => o.Ukprn == ukprn);
            }

            var expectedProvider = new Provider
            {
                Ukprn = ukprn
            };
            expectedAccount.Providers.Add(expectedProvider);

            return expectedProvider;
        }
    }
}