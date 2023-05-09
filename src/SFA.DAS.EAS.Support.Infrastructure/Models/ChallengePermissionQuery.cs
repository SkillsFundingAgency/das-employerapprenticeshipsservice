using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Infrastructure.Models;

[ExcludeFromCodeCoverage]
public class ChallengePermissionQuery
{
    public string Id { get; set; }

    public string Url { get; set; }

    public string ChallengeElement1 { get; set; }

    public string ChallengeElement2 { get; set; }

    public string Balance { get; set; }

    public int FirstCharacterPosition { get; set; }

    public int SecondCharacterPosition { get; set; }
}