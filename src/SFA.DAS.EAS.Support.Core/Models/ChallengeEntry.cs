using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Core.Models;

[ExcludeFromCodeCoverage]
public class ChallengeEntry
{
    public string Id { get; set; }

    public string Url { get; set; }

    public string Challenge1 { get; set; }

    public string Challenge2 { get; set; }

    public string Balance { get; set; }

    public int FirstCharacterPosition { get; set; }

    public int SecondCharacterPosition { get; set; }
}