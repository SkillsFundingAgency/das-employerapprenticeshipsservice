using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models;

[ExcludeFromCodeCoverage]
public class ChallengePermissionResponse
{
    public ChallengePermissionResponse()
    {
        Characters = new List<int>();
    }

    public bool IsValid { get; set; }

    public string Id { get; set; }

    public string Url { get; set; }

    public List<int> Characters { get; set; }
}