namespace SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;

public class GetHmrcEmployerInformationResponse
{
    public HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation EmployerLevyInformation { get; set; }
    public string Empref { get; set; }
    public bool EmprefNotFound { get; set; }
}