using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Services;

public class UserAornPayeLockService : IUserAornPayeLockService
{
    private readonly IUserRepository _userRepository;
    private readonly UserAornPayeLockConfiguration _configuration;
    private readonly ILogger<UserAornPayeLockService> _logger;

    public UserAornPayeLockService(
        IUserRepository userRepository, 
        EmployerAccountsConfiguration configuration,
        ILogger<UserAornPayeLockService> logger)
    {
        _logger = logger;
        _userRepository = userRepository;
        _configuration = configuration.UserAornPayeLock;
    }

    public async Task<bool> UpdateUserAornPayeAttempt(string userRef, bool success)
    {
        try
        {
            await _userRepository.UpdateAornPayeQueryAttempt(userRef, success);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred when calling the Aorn Lock Service.");
            return false;
        }
            
        return true;
    }

    public async Task<UserAornPayeStatus> UserAornPayeStatus(string userRef)
    {
        try
        {
            var lockLimit = DateTime.UtcNow.AddMinutes(-_configuration.LockoutTimeSpanMinutes);
            var attempts = (await _userRepository.GetAornPayeQueryAttempts(userRef)).Where(a => a > lockLimit).OrderByDescending(a => a).ToList();

            if (attempts.Count >= _configuration.NumberOfPermittedAttempts)
            {
                var block = HasUserExceededMaximumAttempts(attempts);
                if (block != null) return block;
            }

            return new UserAornPayeStatus
            {
                IsLocked = false,
                RemainingAttempts = _configuration.NumberOfPermittedAttempts -
                                    attempts.Count(a => (DateTime.UtcNow - a).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes),
                AllowedAttempts = _configuration.NumberOfPermittedAttempts
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred when calling the Aorn Lock Service.");

            return new UserAornPayeStatus
            {
                IsLocked = true,
                RemainingAttempts = 0,
                AllowedAttempts = _configuration.NumberOfPermittedAttempts
            };
        }
    }

    private UserAornPayeStatus HasUserExceededMaximumAttempts(IList<DateTime> attempts)
    {
        for (var i = 0; i <= attempts.Count - _configuration.NumberOfPermittedAttempts; ++i)
        {
            if ((attempts[i] - attempts[i + _configuration.NumberOfPermittedAttempts - 1]).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes)
            {
                return new UserAornPayeStatus
                {
                    IsLocked = true,
                    RemainingLock = Convert.ToInt32((attempts.First().AddMinutes(_configuration.LockoutTimeSpanMinutes) - DateTime.UtcNow).TotalMinutes),
                    RemainingAttempts = 0,
                    AllowedAttempts = _configuration.NumberOfPermittedAttempts
                };
            }
        }

        return null;
    }
}