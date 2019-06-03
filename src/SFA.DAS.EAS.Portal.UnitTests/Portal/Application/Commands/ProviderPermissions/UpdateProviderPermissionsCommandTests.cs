using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.ProviderPermissions.UpdateProviderPermissionsCommandTestsFixture;
using ApiProvider = SFA.DAS.Apprenticeships.Api.Types.Providers.Provider;
using PortalProvider = SFA.DAS.EAS.Portal.Client.Types.Provider;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.ProviderPermissions
{
    [Parallelizable]
    [TestFixture]
    public class UpdateProviderPermissionsCommandTests : FluentTest<UpdateProviderPermissionsCommandTestsFixture>
    {
        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotExist_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.Execute(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.ArrangeEmptyAccountDocument(), f => f.Execute(), f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesContainProvider_ThenAccountDocumentIsSavedWithUpdatedProvider()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsProvider(), f => f.Execute(), 
                f => f.VerifyAccountDocumentSavedWithProviderWithPrimaryAddress());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderWithLegalButNotPrimaryAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
        {
            return TestAsync(f => f.ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress(), f => f.Execute(),
                f => f.VerifyAccountDocumentSavedWithProviderWithLegalAddress());
        }
        
        [Test]
        public Task Execute_WhenProviderApiReturnsProviderWithoutPrimaryOrLegalAddressAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProviderWithoutAddress()
        {
            return TestAsync(f => f.ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress(), f => f.Execute(),
                f => f.VerifyAccountDocumentSavedWithProviderWithoutAddress());
        }
        
        [Test]
        public Task Execute_WhenProviderApiThrows_ThenExceptionIsPropagated()
        {
            return TestExceptionAsync(f => f.ArrangeProviderApiThrowsException(), f => f.Execute(), 
                (f, r) => r.Should().Throw<EntityNotFoundException>().WithMessage(Fix.ProviderApiExceptionMessage));
        }
    }

    public class UpdateProviderPermissionsCommandTestsFixture
    {
        public AddAccountProviderCommand AddAccountProviderCommand { get; set; }
        public Mock<IAccountDocumentService> AccountDocumentService { get; set; }
        public AccountDocument AccountDocument { get; set; }
        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
        public ApiProvider Provider { get; set; }
        public ApiProvider ExpectedProvider { get; set; }
        public Mock<ILogger<AddAccountProviderCommand>> Logger { get; set; }
        public AddedAccountProviderEvent AddedAccountProviderEvent { get; set; }
        public AddedAccountProviderEvent ExpectedAddedAccountProviderEvent { get; set; }
        public Fixture Fixture { get; set; }
        public const long AccountId = 456;
        public const long Ukprn = 123;
        public const string ProviderApiExceptionMessage = "Test message";
        
        public UpdateProviderPermissionsCommandTestsFixture()
        {
            AccountDocumentService = new Mock<IAccountDocumentService>();

            Fixture = new Fixture();
            Provider = Fixture.Create<ApiProvider>();
            
            Provider.Addresses.RandomElement().ContactType = "PRIMARY";
            
            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Ukprn)).ReturnsAsync(Provider);
            
            Logger = new Mock<ILogger<AddAccountProviderCommand>>();

            AddAccountProviderCommand = new AddAccountProviderCommand(AccountDocumentService.Object, ProviderApiClient.Object, Logger.Object);

            AddedAccountProviderEvent = new AddedAccountProviderEvent(1, AccountId, Ukprn, Guid.NewGuid(), DateTime.UtcNow);
            ExpectedAddedAccountProviderEvent = AddedAccountProviderEvent.Clone();
        }

        public UpdateProviderPermissionsCommandTestsFixture ArrangeEmptyAccountDocument()
        {
            AccountDocument = new AccountDocument(AccountId) {IsNew = false};

            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }

        public UpdateProviderPermissionsCommandTestsFixture ArrangeAccountDocumentContainsProvider()
        {
            //note customization will stay in fixture
            long uniqueUkprnAddition = 0;
            Fixture.Customize<PortalProvider>(p => p.With(pr => pr.Ukprn, () => Ukprn + ++uniqueUkprnAddition));
            
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Providers.RandomElement().Ukprn = Ukprn;

            AccountDocument.Account.Id = AccountId;
            
            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }

        public UpdateProviderPermissionsCommandTestsFixture ArrangeApiReturnsProviderWithLegalButNotPrimaryAddress()
        {
            ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress();
            
            Provider.Addresses.RandomElement().ContactType = "LEGAL";

            return this;
        }
        
        public UpdateProviderPermissionsCommandTestsFixture ArrangeApiReturnsProviderWithoutPrimaryOrLegalAddress()
        {
            foreach (var providerAddress in Provider.Addresses)
            {
                providerAddress.ContactType = "CONTACTTYPE";
            }

            return this;
        }
        
        public UpdateProviderPermissionsCommandTestsFixture ArrangeProviderApiThrowsException()
        {
            ProviderApiClient.Setup(c => c.GetAsync(Ukprn))
                .ThrowsAsync(new EntityNotFoundException(ProviderApiExceptionMessage, null));
            
            return this;
        }

        public async Task Execute()
        {
            ExpectedProvider = Provider.Clone();

            await AddAccountProviderCommand.Execute(AddedAccountProviderEvent, CancellationToken.None);
        }

        public UpdateProviderPermissionsCommandTestsFixture VerifyAccountDocumentSavedWithProviderWithPrimaryAddress()
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

        public UpdateProviderPermissionsCommandTestsFixture VerifyAccountDocumentSavedWithProviderWithLegalAddress()
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
        
        public UpdateProviderPermissionsCommandTestsFixture VerifyAccountDocumentSavedWithProviderWithoutAddress()
        {
            VerifyAccountDocumentSavedWithProvider();
            
            return this;
        }
        
        public UpdateProviderPermissionsCommandTestsFixture VerifyAccountDocumentSavedWithProvider(Action<PortalProvider> mutateExpectedProvider = null)
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
                    Id = ExpectedAddedAccountProviderEvent.AccountId,
                };
                expectedProvider = new PortalProvider();
                expectedAccount.Providers.Add(expectedProvider);
            }
            else
            {
                expectedAccount = AccountDocument.Account;
                expectedProvider = expectedAccount.Providers.Single(p => p.Ukprn == Ukprn);
            }

            expectedProvider.Name = ExpectedProvider.ProviderName;
            expectedProvider.Email = ExpectedProvider.Email;
            expectedProvider.Phone = ExpectedProvider.Phone;
            expectedProvider.Ukprn = Ukprn;

            mutateExpectedProvider?.Invoke(expectedProvider);
            
            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}