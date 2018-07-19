﻿using System.Threading.Tasks;

namespace SFA.DAS.Authenication
{
    public interface IAuthenticationService
    {
        string GetClaimValue(string key);
        bool IsUserAuthenticated();
        void SignOutUser();
        bool TryGetClaimValue(string key, out string value);
        Task UpdateClaims();
    }
}
