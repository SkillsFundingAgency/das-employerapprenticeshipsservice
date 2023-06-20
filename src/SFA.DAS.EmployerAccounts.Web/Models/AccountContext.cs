using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.Models;

public class AccountContext
{
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    public string HashedAccountId { get; set; }
}