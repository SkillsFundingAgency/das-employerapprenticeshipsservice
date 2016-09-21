using System;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.WhileList;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class UserWhiteListFileBasedService : FileSystemRepository, IUserWhiteList
    {
        private readonly ICacheProvider _cacheProvider;

        public UserWhiteListFileBasedService(ICacheProvider cacheProvider) : base("WhiteList")
        {
            _cacheProvider = cacheProvider;
        }
        
        public bool IsEmailOnWhiteList(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var whiteList = GetList();

            return whiteList?.EmailPatterns?.Any(pattern => Regex.IsMatch(email, pattern)) ?? true;
        }

        private UserWhiteListLookUp GetList()
        {
            var whiteListLookUp = _cacheProvider.Get<UserWhiteListLookUp>(nameof(UserWhiteListLookUp));

            if (whiteListLookUp != null)
                return whiteListLookUp;

            whiteListLookUp = ReadFileByIdSync<UserWhiteListLookUp>("user_white_list");

            if (whiteListLookUp?.EmailPatterns == null || !whiteListLookUp.EmailPatterns.Any())
                return null;

            _cacheProvider.Set(nameof(UserWhiteListLookUp), whiteListLookUp, new TimeSpan(0, 30, 0));

            return whiteListLookUp;
        }
    }
}
