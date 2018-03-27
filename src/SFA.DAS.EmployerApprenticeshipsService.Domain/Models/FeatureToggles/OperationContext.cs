using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class OperationContext
    {
        public IAuthorizationContext AuthorisationContext { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
