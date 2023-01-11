namespace SFA.DAS.EmployerAccounts.Web.Exceptions;

public class InvalidStateException : Exception
{
    public InvalidStateException(string message) : base(message) { }
}