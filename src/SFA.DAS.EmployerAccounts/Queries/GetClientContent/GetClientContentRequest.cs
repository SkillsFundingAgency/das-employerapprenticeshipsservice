using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetClientContent
{
    public class GetClientContentRequest : IAsyncRequest<GetClientContentResponse>
    {
        public string ContentType { get; set; }
        public string ClientId { get; set; }
    }

    public enum ContentType
    {
        Banner = 1
    }
}
