using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.Models;

public class AccountContext
{
    public string HashedAccountId { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}