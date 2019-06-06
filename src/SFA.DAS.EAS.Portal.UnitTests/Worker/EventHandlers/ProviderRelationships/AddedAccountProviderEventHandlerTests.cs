using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
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

//        [Test]
//        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProvider()
//        {
//            return TestAsync(f => f.ArrangeEmptyAccountDocument(), f => f.Handle(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
//        }
//
//        [Test]
//        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesContainProvider_ThenAccountDocumentIsSavedWithUpdatedProvider()
//        {
//            return TestAsync(f => f.ArrangeAccountDocumentContainsProvider(), f => f.Handle(), 
//                f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
//        }
//
//        [Test]
//        public Task Execute_WhenProviderApiReturnsProviderWithLegalButNotPrimaryAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
//        {
//            return TestAsync(f => f.ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress(), f => f.Handle(),
//                f => f.VerifyAccountDocumentSavedWithProviderWithLegalAddress());
//        }
//        
//        [Test]
//        public Task Execute_WhenProviderApiReturnsProviderWithoutPrimaryOrLegalAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
//        {
//            return TestAsync(f => f.ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress(), f => f.Handle(),
//                f => f.VerifyAccountDocumentSavedWithProviderWithoutAddress());
//        }
//        
//        [Test]
//        public Task Execute_WhenProviderApiThrows_ThenExceptionIsPropagated()
//        {
//            return TestExceptionAsync(f => f.ArrangeProviderApiThrowsException(), f => f.Handle(), 
//                (f, r) => r.Should().Throw<EntityNotFoundException>().WithMessage(Fix.ProviderApiExceptionMessage));
//        }
    }

    public class AddedAccountProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        AddedAccountProviderEvent, AddedAccountProviderEventHandler>
    {
        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
        public ApiProvider Provider { get; set; }
        public ApiProvider ExpectedProvider { get; set; }

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
            
            if (AccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = ExpectedMessage.AccountId,
                };
                expectedProvider = new PortalProvider();
                expectedAccount.Providers.Add(expectedProvider);
            }
            else
            {
                expectedAccount = AccountDocument.Account;
                expectedProvider = expectedAccount.Providers.Single(p => p.Ukprn == ExpectedMessage.ProviderUkprn);
            }

            expectedProvider.Name = ExpectedProvider.ProviderName;
            expectedProvider.Email = ExpectedProvider.Email;
            expectedProvider.Phone = ExpectedProvider.Phone;
            expectedProvider.Ukprn = ExpectedMessage.ProviderUkprn;

            mutateExpectedProvider?.Invoke(expectedProvider);
            
            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}