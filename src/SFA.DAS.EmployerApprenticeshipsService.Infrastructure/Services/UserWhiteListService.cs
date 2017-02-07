using System;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.WhileList;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class UserWhiteListService : AzureServiceBase<UserWhiteListLookUp>, IUserWhiteList
    {
        private readonly ICacheProvider _cacheProvider;
        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.WhiteList";

        public UserWhiteListService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }
        
        public bool IsEmailOnWhiteList(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var whiteList = GetList();

            return whiteList?.EmailPatterns?.Any(pattern => Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase)) ?? false;
        }

        public virtual UserWhiteListLookUp GetList()
        {
            var whiteListLookUp = _cacheProvider.Get<UserWhiteListLookUp>(nameof(UserWhiteListLookUp));

            if (whiteListLookUp != null)
                return whiteListLookUp;

            whiteListLookUp = GetDataFromStorage();

            if (whiteListLookUp?.EmailPatterns == null || !whiteListLookUp.EmailPatterns.Any())
                return null;

            _cacheProvider.Set(nameof(UserWhiteListLookUp), whiteListLookUp, new TimeSpan(0, 30, 0));

            return whiteListLookUp;
        }
        
    }
}
