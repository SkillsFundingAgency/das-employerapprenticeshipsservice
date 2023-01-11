namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

public class GetEmployerAgreementByIdRequest : IAsyncRequest<GetEmployerAgreementByIdResponse>
{
    public string HashedAgreementId { get; set; }
}