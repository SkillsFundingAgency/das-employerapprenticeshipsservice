namespace SFA.DAS.EmployerAccounts.Web.Authentication;

public static class PolicyNames
{
    public static string HasEmployerOwnerAccount => nameof(HasEmployerOwnerAccount);
    public static string HasEmployerViewerTransactorOwnerAccount => nameof(HasEmployerViewerTransactorOwnerAccount);
}