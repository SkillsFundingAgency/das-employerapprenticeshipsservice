using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.PayeRefData;

public sealed class SavePayeRefData : IRequest
{
    public SavePayeRefData(EmployerAccountPayeRefData payeRefData)
    {
        PayeRefData = payeRefData ?? throw new ArgumentNullException(nameof(payeRefData));
    }

    public EmployerAccountPayeRefData PayeRefData { get;  }
}