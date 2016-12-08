using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.EAS.Api.Startup))]

namespace SFA.DAS.EAS.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           ConfigureAuth(app);
        }
    }
}
