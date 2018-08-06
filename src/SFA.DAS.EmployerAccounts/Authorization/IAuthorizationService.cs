using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationContext GetAuthorizationContext();
        Task<IAuthorizationContext> GetAuthorizationContextAsync();
        AuthorizationResult GetAuthorizationResult(FeatureType featureType);
        Task<AuthorizationResult> GetAuthorizationResultAsync(FeatureType featureType);
        bool IsAuthorized(FeatureType featureType);
        Task<bool> IsAuthorizedAsync(FeatureType featureType);
        void ValidateMembership();
    }
}
