namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

public class GetPensionRegulatorRequest : IAsyncRequest<GetPensionRegulatorResponse>
{
    public string PayeRef { get; set; }
}