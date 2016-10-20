using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            using (var serverManager = new ServerManager())
            {
                var mainSite = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"];
                var mainApplication = mainSite.Applications["/"];
                mainApplication["preloadEnabled"] = true;

                var mainApplicationPool = serverManager.ApplicationPools[mainApplication.ApplicationPoolName];
                mainApplicationPool["startMode"] = "AlwaysRunning";

                serverManager.CommitChanges();
            }
            return base.OnStart();
        }
    }
}