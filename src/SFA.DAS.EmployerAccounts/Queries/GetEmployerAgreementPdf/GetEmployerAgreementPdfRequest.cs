namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;

public class GetEmployerAgreementPdfRequest : IRequest<GetEmployerAgreementPdfResponse>
{
    public string HashedAccountId { get; set; }
    public string HashedLegalAgreementId { get; set; }
    public string UserId { get; set; }
}