using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Web.Models;

[ExcludeFromCodeCoverage]
public class FinanceViewModel
{
    public Core.Models.Account Account { get; set; }

    public decimal Balance { get; set; }

    public string SearchUrl { get; set; }
}