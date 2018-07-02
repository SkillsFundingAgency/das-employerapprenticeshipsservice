using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetCharity
{
    public class GetCharityQueryRequest : IAsyncRequest<GetCharityQueryResponse>
    {
        public int RegistrationNumber { get; set; }
    }
}
