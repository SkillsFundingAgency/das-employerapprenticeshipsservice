namespace SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;

public class RemovePayeFromAccountCommand : IRequest
{
    public RemovePayeFromAccountCommand(string hashedAccountId, string payeRef, string userId, bool removeScheme,string companyName)
    {
        HashedAccountId = hashedAccountId;
        PayeRef = payeRef;
        UserId = userId;
        RemoveScheme = removeScheme;
        CompanyName = companyName;
    }

    public string HashedAccountId { get;  }
    public string PayeRef { get;  }
    public string UserId { get;  }
    public bool RemoveScheme { get; }

    public string CompanyName { get;  }
}