using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models;

[ExcludeFromCodeCoverage]
public class AccountFinanceResponse
{
    public Core.Models.Account Account { get; set; }

    public decimal Balance { get; set; }

    public SearchResponseCodes StatusCode { get; set; }
}