using System;
using System.Diagnostics;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace SFA.DAS.EAS.Web
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            try
            {
                using (var serverManager = new ServerManager())
                {
                    var mainSite = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"];
                    if (mainSite == null)
                    {
                        mainSite = serverManager.Sites[0];
                        if (mainSite == null)
                        {
                            throw new NullReferenceException("WebRole not found on IIS");
                        }
                    }
                    var mainApplication = mainSite.Applications["/"];
                    mainApplication["preloadEnabled"] = true;

                    var mainApplicationPool = serverManager.ApplicationPools[mainApplication.ApplicationPoolName];
                    mainApplicationPool["startMode"] = "AlwaysRunning";

					serverManager.ApplicationPoolDefaults.ProcessModel.IdleTimeout = new System.TimeSpan(0);
                    serverManager.CommitChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Application Initialization failed: \n{e?.Message}");
            }
            return base.OnStart();
        }
    }
}