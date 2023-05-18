using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices;

public class ChallengeHandler : IChallengeHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeService _challengeService;

    public ChallengeHandler(IAccountRepository accountRepository, IChallengeService challengeService,
        IChallengeRepository challengeRepository)
    {
        _accountRepository = accountRepository;
        _challengeService = challengeService;
        _challengeRepository = challengeRepository;
    }

    public async Task<ChallengeResponse> Get(string id)
    {
        var response = new ChallengeResponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };

        var record = await _accountRepository.Get(id, AccountFieldsSelection.PayeSchemes);

        if (record == null)
        {
            return response;
        }
        
        response.StatusCode = SearchResponseCodes.Success;
        response.Account = record;
        response.Characters = _challengeService.GetPayeSchemesCharacters(record.PayeSchemes);

        return response;
    }

    public async Task<ChallengePermissionResponse> Handle(ChallengePermissionQuery message)
    {
        var challengeResponse = await Get(message.Id);

        var isValidInput = !(string.IsNullOrEmpty(message.Balance)
                        || string.IsNullOrEmpty(message.ChallengeElement1)
                        || string.IsNullOrEmpty(message.ChallengeElement2)
                        || !int.TryParse(message.Balance.Split('.')[0].Replace("£", string.Empty), out int balance)
                        || message.ChallengeElement1.Length != 1
                        || message.ChallengeElement2.Length != 1
                       );

        var response = new ChallengePermissionResponse
        {
            Id = message.Id,
            Url = message.Url,
            IsValid = false
        };

        if (challengeResponse.StatusCode != SearchResponseCodes.Success)
        {
            return response;
        }
        
        if (isValidInput)
        {
            response.IsValid = await _challengeRepository.CheckData(challengeResponse.Account, message);
        }

        response.Characters = challengeResponse.Characters;

        return response;
    }
}