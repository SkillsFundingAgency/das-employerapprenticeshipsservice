using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public static class AccountTaskHelper
    {
        public static int GetTaskPriority(AccountTask task)
        {
            switch (task.Type)
            {
                case "LevyDeclarationDue": return 1;
                case "AgreementToSign": return 2;
                case "AddApprentices": return 3;
                case "ApprenticeChangesToReview": return 4;
                case "CohortRequestReadyForApproval": return 5;
                case "IncompleteApprenticeshipDetails": return 6;
                case "ReviewConnectionRequest": return 7;
                case "TransferRequestReceived": return 8;

                default: return int.MaxValue; //if its an usupported type we place it last
            }
        }

        public static bool IsSupportConsoleUser(IPrincipal user)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var requiredRoles = configuration.SupportConsoleUsers.Split(',');
            var currentRoles = ((ClaimsIdentity)user.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            foreach (var role in requiredRoles)
            {
                if (currentRoles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}