using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.Account;

public class CreateAccountParams
{
    public long UserId { get; set; }
    public string EmployerNumber { get; set; }
    public string EmployerName { get; set; }
    public string EmployerRegisteredAddress { get; set; }
    public DateTime? EmployerDateOfIncorporation { get; set; }
    public string EmployerRef { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string CompanyStatus { get; set; }
    public string EmployerRefName { get; set; }
    public short Source { get; set; }
    public short? PublicSectorDataSource { get; set; }
    public string Sector { get; set; }
    public string Aorn { get; set; }
    public AgreementType AgreementType { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}