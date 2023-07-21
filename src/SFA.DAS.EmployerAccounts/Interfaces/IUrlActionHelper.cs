namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IUrlActionHelper
{
    string EmployerAccountsAction(string path);
    string EmployerCommitmentsV2Action(string path);
    string LevyTransfersMatchingAction(string path);
    string ReservationsAction(string path);
    string EmployerFinanceAction(string path);
    string EmployerIncentivesAction(string path = "");
    string EmployerProjectionsAction(string path);
    string EmployerRecruitAction(string path = "");
    string ProviderRelationshipsAction(string path);
    string LegacyEasAccountAction(string path);
    string LegacyEasAction(string path);
    string EmployerFeedbackAction(string path);
    string EmployerProfileAddUserDetails(string path);
}