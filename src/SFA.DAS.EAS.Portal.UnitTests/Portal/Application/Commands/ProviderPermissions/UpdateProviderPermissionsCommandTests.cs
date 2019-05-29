using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Commands.ProviderPermissions;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.ProviderPermissions
{
    [Parallelizable]
    [TestFixture]
    public class UpdateProviderPermissionsCommandTests : FluentTest<UpdateProviderPermissionsCommandTestsFixture>
    {
    }

    public class UpdateProviderPermissionsCommandTestsFixture
    {
        public AddAccountProviderCommand AddAccountProviderCommand { get; set; }
        public IAccountDocumentService AccountDocumentService { get; set; }
        public IProviderApiClient ProviderApiClient { get; set; }
        public ILogger<AddAccountProviderCommand> Logger { get; set; }
        
        public UpdateProviderPermissionsCommandTestsFixture()
        {
            //AddAccountProviderCommand = new AddAccountProviderCommand();
        }
    }
}