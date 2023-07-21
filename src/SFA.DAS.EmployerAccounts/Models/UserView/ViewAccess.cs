namespace SFA.DAS.EmployerAccounts.Models.UserView;

public class ViewAccess
{
    public string ViewName { get; set; }

    public int Weighting { get; set; }

    public List<string> EmailAddresses { get; set; }
        
}