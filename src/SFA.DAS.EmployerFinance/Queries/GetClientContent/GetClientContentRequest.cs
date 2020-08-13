using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetClientContent
{
    public class GetClientContentRequest : IAsyncRequest<GetClientContentResponse>
    {
        public string ContentType { get; set; }
        public bool UseLegacyStyles { get; set; }
    }
}
