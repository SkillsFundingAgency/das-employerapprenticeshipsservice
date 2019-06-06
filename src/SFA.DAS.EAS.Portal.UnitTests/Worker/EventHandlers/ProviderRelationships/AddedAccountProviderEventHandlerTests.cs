using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.ProviderRelationships.AddedAccountProviderEventHandlerTestsFixture;
using ApiProvider = SFA.DAS.Apprenticeships.Api.Types.Providers.Provider;
using PortalProvider = SFA.DAS.EAS.Portal.Client.Types.Provider;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.ProviderRelationships
{
    [TestFixture]
    [Parallelizable]
    public class AddedAccountProviderEventHandlerTests : FluentTest<AddedAccountProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
        
        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        [Ignore("needs fixing")]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(), f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
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

    public class AddedAccountProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        AddedAccountProviderEvent, AddedAccountProviderEventHandler>
    {
        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
        public ApiProvider Provider { get; set; }
        public ApiProvider ExpectedProvider { get; set; }
        public const string ProviderApiExceptionMessage = "Test message";

        public AddedAccountProviderEventHandlerTestsFixture() 
            : base(false)
        {
            Provider = Fixture.Create<ApiProvider>();
            Provider.Addresses.RandomElement().ContactType = "PRIMARY";

            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Message.ProviderUkprn)).ReturnsAsync(Provider);

            Handler = new AddedAccountProviderEventHandler(
                AccountDocumentService.Object,
                MessageContextInitialisation.Object,
                Logger.Object,
                ProviderApiClient.Object);
            
            Message = new AddedAccountProviderEvent(1, AccountId, Message.ProviderUkprn, Guid.NewGuid(), DateTime.UtcNow);
        }

        public AddedAccountProviderEventHandlerTestsFixture ArrangeAccountDocumentContainsProvider()
        {
            //note customization will stay in fixture
            long uniqueUkprnAddition = 0;
            Fixture.Customize<PortalProvider>(p => p.With(pr => pr.Ukprn, () => Message.ProviderUkprn + ++uniqueUkprnAddition));
            
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Providers.RandomElement().Ukprn = Message.ProviderUkprn;

            AccountDocument.Account.Id = AccountId;
            
            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
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
            ProviderApiClient.Setup(c => c.GetAsync(Message.ProviderUkprn))
                .ThrowsAsync(new EntityNotFoundException(ProviderApiExceptionMessage, null));
            
            return this;
        }
        
        public override Task Handle()
        {
            ExpectedProvider = Provider.Clone();

            return base.Handle();
        }

        public AddedAccountProviderEventHandlerTestsFixture VerifyAccountDocumentSavedWithProviderWithPrimaryAddress()
        {
            var expectedLegalAddress = ExpectedProvider.Addresses.Single(a => a.ContactType == "PRIMARY");

            VerifyAccountDocumentSavedWithProvider(p =>
            {
                p.Street = expectedLegalAddress.Street;
                p.Town = expectedLegalAddress.Town;
                p.Postcode = expectedLegalAddress.PostCode;
            });

            return this;
        }

        public AddedAccountProviderEventHandlerTestsFixture VerifyAccountDocumentSavedWithProviderWithLegalAddress()
        {
            var expectedLegalAddress = ExpectedProvider.Addresses.Single(a => a.ContactType == "LEGAL");

            VerifyAccountDocumentSavedWithProvider(p =>
            {
                p.Street = expectedLegalAddress.Street;
                p.Town = expectedLegalAddress.Town;
                p.Postcode = expectedLegalAddress.PostCode;
            });

            return this;
        }
        
        public AddedAccountProviderEventHandlerTestsFixture VerifyAccountDocumentSavedWithProviderWithoutAddress()
        {
            VerifyAccountDocumentSavedWithProvider();
            
            return this;
        }
        
        public AddedAccountProviderEventHandlerTestsFixture VerifyAccountDocumentSavedWithProvider(Action<PortalProvider> mutateExpectedProvider = null)
        {
            AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d, mutateExpectedProvider)), It.IsAny<CancellationToken>()), Times.Once);
            
            return this;
        }

        public bool AccountIsAsExpected(AccountDocument document, Action<PortalProvider> mutateExpectedProvider = null)
        {
            Account expectedAccount;
            PortalProvider expectedProvider;
            
            if (OriginalAccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = OriginalMessage.AccountId,
                };
                expectedProvider = new PortalProvider();
                expectedAccount.Providers.Add(expectedProvider);
            }
            else
            {
                expectedAccount = OriginalAccountDocument.Account;
                expectedProvider = expectedAccount.Providers.Single(p => p.Ukprn == OriginalMessage.ProviderUkprn);
            }

            expectedProvider.Name = ExpectedProvider.ProviderName;
            expectedProvider.Email = ExpectedProvider.Email;
            expectedProvider.Phone = ExpectedProvider.Phone;
            expectedProvider.Ukprn = OriginalMessage.ProviderUkprn;

            mutateExpectedProvider?.Invoke(expectedProvider);
            
            if (document?.Account == null)
                return false;
            
            var (accountIsAsExpected, differences) = document.Account.IsEqual(expectedAccount);
            if (!accountIsAsExpected)
            {
                TestContext.WriteLine($"Saved account is not as expected: {differences}");
            }
            
            return accountIsAsExpected;
        }
    }
}