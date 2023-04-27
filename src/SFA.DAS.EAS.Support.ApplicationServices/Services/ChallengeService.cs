using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services;

public class ChallengeService : IChallengeService
{
    public List<int> GetPayeSchemesCharacters(IEnumerable<PayeSchemeModel> payeSchemes)
    {
        if(payeSchemes == null || !payeSchemes.Any())
        {
            return new List<int>();
        }

        var schemes = payeSchemes
            .Select(p =>p.ObscuredPayeRef.Substring(1, p.ObscuredPayeRef.Length - 2).Replace("/", string.Empty))
            .ToList();

        var range = GetMinimumNumberOfCharacters(schemes);

        var response = GetRandomPositions(range + 1);

        response.Sort();

        return response;
    }

    private static List<int> GetRandomPositions(int range)
    {
        var r = new Random();
        var random1 = r.Next(1, range);

        int random2;

        do
        {
            random2 = r.Next(1, range);
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