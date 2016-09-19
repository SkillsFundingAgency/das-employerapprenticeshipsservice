using System;
using System.Linq;
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

        public UserWhiteListLookUp GetList()
        {
            var whiteList = _cacheProvider.Get<UserWhiteListLookUp>(nameof(UserWhiteListLookUp));

            if (whiteList != null)
                return whiteList;

            whiteList = ReadFileByIdSync<UserWhiteListLookUp>("user_white_list");

            if (whiteList?.Emails != null && whiteList.Emails.Any())
            {
                _cacheProvider.Set(nameof(UserWhiteListLookUp), whiteList, new TimeSpan(0, 30, 0));
            }

            return whiteList;
        }
    }
}
