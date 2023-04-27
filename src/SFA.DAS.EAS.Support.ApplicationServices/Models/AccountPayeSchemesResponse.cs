using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models;

[ExcludeFromCodeCoverage]
public class AccountPayeSchemesResponse
{
    public Core.Models.Account Account { get; set; }
    public SearchResponseCodes StatusCode { get; set; }
}