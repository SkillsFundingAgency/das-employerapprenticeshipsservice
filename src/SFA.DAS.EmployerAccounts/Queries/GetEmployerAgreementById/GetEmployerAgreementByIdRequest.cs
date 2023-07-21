namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

public class GetEmployerAgreementByIdRequest : IRequest<GetEmployerAgreementByIdResponse>
{
    public long AgreementId { get; set; }
}