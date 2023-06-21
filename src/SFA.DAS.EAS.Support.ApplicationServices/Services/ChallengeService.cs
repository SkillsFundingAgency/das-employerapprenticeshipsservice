using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services;

public class ChallengeService : IChallengeService
{
    public List<int> GetPayeSchemesCharacters(IEnumerable<PayeSchemeModel> payeSchemes)
    {
        var payeSchemeModels = payeSchemes as PayeSchemeModel[] ?? payeSchemes.ToArray();
        
        if (!payeSchemeModels.Any())
        {
            return new List<int>();
        }

        var schemes = payeSchemeModels
            .Select(p => p.ObscuredPayeRef.Substring(1, p.ObscuredPayeRef.Length - 2).Replace("/", string.Empty))
            .ToList();

        var range = GetMinimumNumberOfCharacters(schemes);

        var response = GetRandomPositions(range + 1);

        response.Sort();

        return response;
    }

    private static List<int> GetRandomPositions(int range)
    {
        var random1 = RandomNumberGenerator.GetInt32(1, range);

        int random2;

        do
        {
            random2 = RandomNumberGenerator.GetInt32(1, range);
        } while (random1 == random2);

        return new List<int>
        {
            random1,
            random2
        };
    }

    private static int GetMinimumNumberOfCharacters(IEnumerable<string> schemes)
    {
        return schemes.Select(scheme => scheme.Length).Min();
    }
}