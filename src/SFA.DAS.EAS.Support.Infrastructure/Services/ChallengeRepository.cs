using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public class ChallengeRepository : IChallengeRepository
{
    private readonly IFinanceRepository _financeRepository;

    public ChallengeRepository(IFinanceRepository financeRepository)
    {
        _financeRepository = financeRepository;
    }

    public async Task<bool> CheckData(Core.Models.Account record, ChallengePermissionQuery message)
    {
        var balance = await _financeRepository.GetAccountBalance(message.Id);

        var validPayeSchemesData = CheckPayeSchemesData(record.PayeSchemes, message);

        if (!decimal.TryParse(message.Balance.Replace("£", string.Empty), out decimal messageBalance))
        {
            return false;
        }

        return Math.Round(balance) == Math.Round(Convert.ToDecimal(messageBalance)) && validPayeSchemesData;
    }

    private static bool CheckPayeSchemesData(IEnumerable<PayeSchemeModel> recordPayeSchemes, ChallengePermissionQuery message)
    {
        var challengeInput = new List<string>
        {
            message.ChallengeElement1.ToLower(),
            message.ChallengeElement2.ToLower()
        };

        var list = recordPayeSchemes.Select(x => x.PayeRefWithOutSlash);
        var index1 = message.FirstCharacterPosition;
        var index2 = message.SecondCharacterPosition;

        return list.Any(x => x[index1].ToString().ToLower() == challengeInput[0] &&
                             x[index2].ToString().ToLower() == challengeInput[1]);
    }
}