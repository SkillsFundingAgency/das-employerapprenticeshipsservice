namespace SFA.DAS.EmployerAccounts.Models;

public class ProviderInvitation
{
    public string EmployerOrganisation { get; set; }

    public string EmployerFirstName { get; set; }

    public string EmployerLastName { get; set; }

    public string EmployerEmail { get; set; }

    public int Status { get; set; }

    public string Ukprn { get; set; }

    public DateTime SentDate { get; set; }

    public string ProviderOrganisationName { get; set; }

    public string ProviderUserFullName { get; set; }
}