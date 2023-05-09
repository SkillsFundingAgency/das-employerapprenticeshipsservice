using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Web.Models;

[ExcludeFromCodeCoverage]
public class ChallengeViewModel
{
    public string Id { get; set; }

    public string Url { get; set; }

    public List<int> Characters { get; set; }

    public bool HasError { get; set; }
}