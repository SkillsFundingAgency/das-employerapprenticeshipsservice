using System;
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
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;
using Fix = SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.ProviderPermissions.UpdateProviderPermissionsCommandTestsFixture;

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
        
        [Test]
        public Task Execute_WhenProviderApiThrows_ThenExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeProviderApiReturnsNotFound(), f => f.Execute(), (f, r) => r.Should().Throw<EntityNotFoundException>());
        }
        
        [Test]
        public Task Execute_WhenProviderApiReturnsNotFound_ThenExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.Execute(), 
                (f, r) => r.Should().Throw<Exception>().WithMessage(Fix.ExpectedProviderNotFoundExceptionMessage));
        }
    }

    public class UpdateProviderPermissionsCommandTestsFixture
    {
        public AddAccountProviderCommand AddAccountProviderCommand { get; set; }
        public Mock<IAccountDocumentService> AccountDocumentService { get; set; }
        public Mock<IProviderApiClient> ProviderApiClient { get; set; }
//        public Mock<Provider> Provider { get; set; }
        public Provider Provider { get; set; }
        public Provider ExpectedProvider { get; set; }
        public Mock<ILogger<AddAccountProviderCommand>> Logger { get; set; }
        public AddedAccountProviderEvent AddedAccountProviderEvent { get; set; }
        public const long Ukprn = 123;
        public const string ExpectedProviderNotFoundExceptionMessage = "Provider with UKPRN=123 not found";
        
        public UpdateProviderPermissionsCommandTestsFixture()
        {
            AccountDocumentService = new Mock<IAccountDocumentService>();

//            Provider = new Mock<Provider>();
//            Provider.SetupGet(p => p.ProviderName).Returns();
                
//            var primaryAddress = provider.Addresses.SingleOrDefault(a => a.ContactType == "PRIMARY");
//            
//            accountProvider.Name = provider.ProviderName;
//            accountProvider.Email = provider.Email;
//            accountProvider.Phone = provider.Phone;
//            accountProvider.Street = primaryAddress?.Street;
//            accountProvider.Town = primaryAddress?.Town;
//            accountProvider.Postcode = primaryAddress?.PostCode;

            var fixture = new Fixture();
            Provider = fixture.Create<Provider>();
            ExpectedProvider = Provider.Clone();
            
            ProviderApiClient = new Mock<IProviderApiClient>();
            ProviderApiClient.Setup(c => c.GetAsync(Ukprn)).ReturnsAsync(Provider);
            
            Logger = new Mock<ILogger<AddAccountProviderCommand>>();

            AddAccountProviderCommand = new AddAccountProviderCommand(AccountDocumentService.Object, ProviderApiClient.Object, Logger.Object);

            //AddedAccountProviderEvent = Fixture.Create<AddedAccountProviderEvent>();
            AddedAccountProviderEvent = new AddedAccountProviderEvent(1, 1, Ukprn, Guid.NewGuid(), DateTime.UtcNow);
        }

        public UpdateProviderPermissionsCommandTestsFixture ArrangeProviderApiReturnsNotFound()
        {
            return this;
        }

        public UpdateProviderPermissionsCommandTestsFixture ArrangeProviderApiThrowsException()
        {
            return this;
        }

        public async Task Execute()
        {
            await AddAccountProviderCommand.Execute(AddedAccountProviderEvent, CancellationToken.None);
        }

        public void VerifyAccountDocumentSavedWithProvider()
        {
            //todo:
        }
    }
}