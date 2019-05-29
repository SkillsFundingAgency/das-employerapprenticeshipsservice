using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.ProviderRelationships;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.ProviderRelationships
{
    [TestFixture]
    [Parallelizable]
    public class AddedAccountProviderEventHandlerTests : FluentTest<AddedAccountProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task x()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyCommandExecuted());
        }
    }

    public class AddedAccountProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        AddedAccountProviderEvent, AddedAccountProviderEventHandler, IAddAccountProviderCommand>
    {
//        protected override AddAccountProviderCommand CreateCommand()
//        {
//            var addAccountProviderCommand = new Mock<AddAccountProviderCommand>();
//            //todo: doc & logger can be done by base
////            IAccountDocumentService accountDocumentService,
////                IProviderApiClient providerApiClient,
////            ILogger<AddAccountProviderCommand> logger)
//            return addAccountProviderCommand.Object;
//        }
    }
}