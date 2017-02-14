using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressRequest : IAsyncRequest<GetPostcodeAddressResponse>
    {
        public string Postcode { get; set; }
    }
}
