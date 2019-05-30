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
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.ProviderPermissions.UpdateProviderPermissionsCommandTestsFixture;
using Provider = SFA.DAS.Apprenticeships.Api.Types.Providers.Provider;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.ProviderPermissions
{
    [Parallelizable]
    [TestFixture]
    [Ignore("In progress")]
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
            return TestAsync(f => f.Execute(), f => f.VerifyAccountDocumentSavedWithProvider());
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
        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
        public Provider Provider { get; set; }
        public Provider ExpectedProvider { get; set; }
        public Mock<ILogger<AddAccountProviderCommand>> Logger { get; set; }
        public AddedAccountProviderEvent AddedAccountProviderEvent { get; set; }
        public AddedAccountProviderEvent ExpectedAddedAccountProviderEvent { get; set; }
        public const long Ukprn = 123;
        public const string ProviderApiExceptionMessage = "Test message";
        
        public UpdateProviderPermissionsCommandTestsFixture()
        {
            AccountDocumentService = new Mock<IAccountDocumentService>();

            //todo: better way to do this?
            var fixture = new Fixture();
            Provider = fixture.Create<Provider>();
            var providerAddresses = new List<ContactAddress>();
            fixture.AddManyTo(providerAddresses);
            var primaryAddressIndex = new Random().Next(providerAddresses.Count);
            providerAddresses.ForEach(a => a.ContactType = "CONTACTTYPE");
            providerAddresses[primaryAddressIndex].ContactType = "PRIMARY";
            Provider.Addresses = providerAddresses;
            ExpectedProvider = Provider.Clone();
            
            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Ukprn)).ReturnsAsync(Provider);
            
            Logger = new Mock<ILogger<AddAccountProviderCommand>>();

            AddAccountProviderCommand = new AddAccountProviderCommand(AccountDocumentService.Object, ProviderApiClient.Object, Logger.Object);

            //AddedAccountProviderEvent = Fixture.Create<AddedAccountProviderEvent>();
            AddedAccountProviderEvent = new AddedAccountProviderEvent(1, 1, Ukprn, Guid.NewGuid(), DateTime.UtcNow);
            ExpectedAddedAccountProviderEvent = AddedAccountProviderEvent.Clone();
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

        public bool AccountIsAsExpected(AccountDocument document)
        {
            var expectedPrimaryAddress = ExpectedProvider.Addresses.Single(a => a.ContactType == "PRIMARY");
            return document?.Account != null && document.IsEqual(new Account
            {
                Id = ExpectedAddedAccountProviderEvent.AccountId,
                Providers = new List<EAS.Portal.Client.Types.Provider>
                {
                    new EAS.Portal.Client.Types.Provider
                    {
                        Name = ExpectedProvider.ProviderName,
                        Email = ExpectedProvider.Email,
                        Phone = ExpectedProvider.Phone,
                        Postcode = expectedPrimaryAddress.PostCode,
                        Street = expectedPrimaryAddress.Street,
                        Town = expectedPrimaryAddress.Town,
                        Ukprn = Ukprn
                    }
                }
            });
        }
    }
}