using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetContent
{
    public class GetContentRequest : IAsyncRequest<GetContentResponse>
    {
        public string ContentType { get; set; }
        public bool UseLegacyStyles { get; set; }
    }
}
