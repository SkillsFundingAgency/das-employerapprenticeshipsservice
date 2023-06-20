namespace SFA.DAS.EmployerAccounts.Queries.GetContent;

public class GetContentRequest : IRequest<GetContentResponse>
{
    public string ContentType { get; set; }
    public bool UseLegacyStyles { get; set; }
}