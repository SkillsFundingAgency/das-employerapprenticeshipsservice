namespace SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;

public class GetSignedEmployerAgreementPdfRequest : IRequest<GetSignedEmployerAgreementPdfResponse>
{
    public long AccountId { get; set; }
    public string UserId { get; set; }
    public long LegalAgreementId { get; set; }
}