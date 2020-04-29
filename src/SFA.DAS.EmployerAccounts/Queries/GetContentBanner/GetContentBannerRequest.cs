using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetContentBanner
{
    public class GetContentBannerRequest : IAsyncRequest<GetContentBannerResponse>
    {
        public int BannerId { get; set; }
        public bool UseCDN { get; set; }
    }
}
