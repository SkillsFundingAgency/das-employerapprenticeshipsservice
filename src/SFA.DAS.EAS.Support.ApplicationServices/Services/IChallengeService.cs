using System.Collections.Generic;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Services;

public interface IChallengeService
{
    List<int> GetPayeSchemesCharacters(IEnumerable<PayeSchemeModel> payeSchemes);
}