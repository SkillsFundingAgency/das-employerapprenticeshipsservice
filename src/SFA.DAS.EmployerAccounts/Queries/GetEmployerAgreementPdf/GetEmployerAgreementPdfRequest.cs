namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;

public class GetEmployerAgreementPdfRequest : IRequest<GetEmployerAgreementPdfResponse>
{
    public long AccountId { get; set; }
    public long LegalAgreementId { get; set; }
    public string UserId { get; set; }
}