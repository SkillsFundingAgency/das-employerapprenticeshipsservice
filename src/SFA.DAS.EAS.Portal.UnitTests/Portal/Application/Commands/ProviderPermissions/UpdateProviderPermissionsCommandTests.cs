using System;
using System.Collections.Generic;
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
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesNotContainProvider_ThenAccountDocumentIsSavedWithNewProvider()
        {
            return TestAsync(f => f.Execute(), f => f.VerifyAccountDocumentSavedWithProvider());
        }

        [Test]
        public Task Execute_WhenProviderApiReturnsProviderAndAccountDoesContainProvider_ThenAccountDocumentIsSavedWithUpdatedProvider()
        {
            return TestAsync(f => f.ArrangeAccountDocumentContainsProvider(),
                f => f.Execute(), 
                f => f.VerifyAccountDocumentSavedWithProvider());
        }

        //todo: tests with multiple providers, inc. dupes?
        //todo: test with no primary address
        
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
            
            Provider.Addresses.Skip(new Random().Next(Provider.Addresses.Count()))
                .First().ContactType = "PRIMARY";
            
            ExpectedProvider = Provider.Clone();
            
            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Ukprn)).ReturnsAsync(Provider);
            
            Logger = new Mock<ILogger<AddAccountProviderCommand>>();

            AddAccountProviderCommand = new AddAccountProviderCommand(AccountDocumentService.Object, ProviderApiClient.Object, Logger.Object);

            //AddedAccountProviderEvent = Fixture.Create<AddedAccountProviderEvent>();
            AddedAccountProviderEvent = new AddedAccountProviderEvent(1, AccountId, Ukprn, Guid.NewGuid(), DateTime.UtcNow);
            ExpectedAddedAccountProviderEvent = AddedAccountProviderEvent.Clone();
        }

        public UpdateProviderPermissionsCommandTestsFixture ArrangeAccountDocumentContainsProvider()
        {
            //todo: builder using autofixture?
            //note customization will stay in fixture
            long uniqueUkprnAddition = 0;
            Fixture.Customize<PortalProvider>(p => p.With(pr => pr.Ukprn, () => Ukprn + ++uniqueUkprnAddition));
            
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Providers.Skip(new Random().Next(AccountDocument.Account.Providers.Count))
                .First().Ukprn = Ukprn;

            AccountDocument.Account.Id = AccountId;
            
            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;
            
            AccountDocumentService.Setup(s => s.Get(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
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
            await AddAccountProviderCommand.Execute(AddedAccountProviderEvent, CancellationToken.None);
        }

        public void VerifyAccountDocumentSavedWithProvider()
        {
            AccountDocumentService.Verify(
                s => s.Save(It.Is<AccountDocument>(d => AccountIsAsExpected(d)), It.IsAny<CancellationToken>()), Times.Once);

        }

        //todo: not keen on this. better way?
        public bool AccountIsAsExpected(AccountDocument document)
        {
            Account expectedAccount;
            PortalProvider expectedProvider;
            
            if (AccountDocument == null)
            {
                expectedAccount = new Account
                {
                    Id = ExpectedAddedAccountProviderEvent.AccountId,
                    Providers = new List<Provider>()
                };
                expectedProvider = new PortalProvider();
                expectedAccount.Providers.Add(expectedProvider);
            }
            else
            {
                expectedAccount = AccountDocument.Account;
                expectedProvider = expectedAccount.Providers.Single(p => p.Ukprn == Ukprn);
            }

            var expectedPrimaryAddress = ExpectedProvider.Addresses.Single(a => a.ContactType == "PRIMARY");
            
            expectedProvider.Name = ExpectedProvider.ProviderName;
            expectedProvider.Email = ExpectedProvider.Email;
            expectedProvider.Phone = ExpectedProvider.Phone;
            expectedProvider.Postcode = expectedPrimaryAddress.PostCode;
            expectedProvider.Street = expectedPrimaryAddress.Street;
            expectedProvider.Town = expectedPrimaryAddress.Town;
            expectedProvider.Ukprn = Ukprn;

            return document?.Account != null && document.Account.IsEqual(expectedAccount);
        }
    }
}