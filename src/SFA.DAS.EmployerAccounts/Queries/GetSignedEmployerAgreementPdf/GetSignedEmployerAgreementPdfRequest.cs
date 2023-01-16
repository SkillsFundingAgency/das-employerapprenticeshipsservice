namespace SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;

public class GetSignedEmployerAgreementPdfRequest : IRequest<GetSignedEmployerAgreementPdfResponse>
{
    public string HashedAccountId { get; set; }
    public string UserId { get; set; }
    public string HashedLegalAgreementId { get; set; }
}