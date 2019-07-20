using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class UserAornPayeLockService : IUserAornPayeLockService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserAornPayeLockConfiguration _configuration;

        public UserAornPayeLockService(IUserRepository userRepository, EmployerAccountsConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration.UserAornPayeLock;
        }

        public async Task UpdateUserAornPayeAttempt(Guid userRef, bool success)
        {
            await _userRepository.UpdateAornPayeQueryAttempt(userRef, success);
        }

        public async Task<UserAornPayeStatus> UserAornPayeStatus(Guid userRef)
        {
            var lockLimit = DateTime.Now.AddMinutes(-_configuration.LockoutTimeSpanMinutes);
            var attempts = (await _userRepository.GetAornPayeQueryAttempts(userRef)).Where(a => a > lockLimit).OrderByDescending(a => a).ToList();

            if (attempts.Count >= _configuration.NumberOfPermittedAttempts)
            {
                for (var i = 0; i <= attempts.Count - _configuration.NumberOfPermittedAttempts; ++i)
                {
                    if ((attempts[i] - attempts[i + _configuration.NumberOfPermittedAttempts - 1]).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes)
                    {
                        return new UserAornPayeStatus
                        {
                            IsLocked = true,
                            RemainingLock = Convert.ToInt32((attempts.First().AddMinutes(_configuration.LockoutTimeSpanMinutes) - DateTime.Now).TotalMinutes),
                            RemainingAttempts = 0,
                            AllowedAttempts = _configuration.NumberOfPermittedAttempts
                        };
                    }
                }
            }

            return new UserAornPayeStatus
            {
                IsLocked = false,
                RemainingAttempts = _configuration.NumberOfPermittedAttempts -
                                    attempts.Count(a => (DateTime.Now - a).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes),
                AllowedAttempts = _configuration.NumberOfPermittedAttempts
            };
        }
    }
}
