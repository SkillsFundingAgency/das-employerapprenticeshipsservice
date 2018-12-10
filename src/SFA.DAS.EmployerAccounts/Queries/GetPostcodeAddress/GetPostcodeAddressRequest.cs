using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressRequest : IAsyncRequest<GetPostcodeAddressResponse>
    {
        public string Postcode { get; set; }
    }
}
