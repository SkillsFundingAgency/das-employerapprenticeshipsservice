namespace SFA.DAS.EmployerAccounts.Audit;

public class InvalidContextException : Exception
{
    public InvalidContextException(string message)
        : base(message)
    {
    }
}
