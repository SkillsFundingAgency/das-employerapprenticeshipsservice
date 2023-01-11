namespace SFA.DAS.EmployerAccounts.Configuration;

public class RecruitClientApiConfiguration : IRecruitClientApiConfiguration
{
    public string ApiBaseUrl { get; set; }       
    public string IdentifierUri { get; set; }

}