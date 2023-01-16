namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

public class GetPensionRegulatorRequest : IRequest<GetPensionRegulatorResponse>
{
    public string PayeRef { get; set; }
}