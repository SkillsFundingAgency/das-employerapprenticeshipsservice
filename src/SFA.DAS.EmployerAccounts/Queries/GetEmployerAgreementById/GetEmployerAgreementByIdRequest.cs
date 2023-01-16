namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

public class GetEmployerAgreementByIdRequest : IRequest<GetEmployerAgreementByIdResponse>
{
    public string HashedAgreementId { get; set; }
}