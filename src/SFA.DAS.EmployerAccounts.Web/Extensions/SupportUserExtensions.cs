//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Notifications;
//using Newtonsoft.Json;
//using NLog;
//using Microsoft.AspNetCore.Authentication.WsFederation;
//using ILogger = Microsoft.Extensions.Logging.ILogger;

//namespace SFA.DAS.EmployerAccounts.Web.Extensions;

//public static class SupportUserExtensions
//{
//    private const string Staff = "Staff";
//    private const string Employer = "Employer";
//    private const string HashedAccountId = "HashedAccountId";
//    private const string ServiceClaimTier2Role = "ESF";
//    private const string Tier2User = "Tier2User";
//    private const string Tier1User = "Tier1User";
//    private const string ConsoleUser = "ConsoleUser";
//    private const string ServiceClaimTier1Role = "ESS";
//    private const string serviceClaimType = "http://service/service";
//    private static ILogger Logger;

//    public static IServiceCollection AddAndConfigureSupportUserAuthentications(this IServiceCollection services, SupportConsoleAuthenticationOptions options)
//    {
//        Logger = options.Logger;

//        //app.UseCookieAuthentication(new CookieAuthenticationOptions
//        //{
//        //    AuthenticationType = Staff,
//        //});

//        //app.UseCookieAuthentication(new CookieAuthenticationOptions
//        //{
//        //    AuthenticationType = Employer,
//        //    ExpireTimeSpan = new TimeSpan(0, 10, 0)
//        //});

//        var wsFederationOptions = new WsFederationOptions
//        {
//            AuthenticationType = Staff,
//            Wtrealm = options.AdfsOptions.Wtrealm,
//            MetadataAddress = options.AdfsOptions.MetadataAddress,
//            Events = Notifications(),
//            Notifications = Notifications(),
//            Wreply = options.AdfsOptions.Wreply
//        };

//        //https://skillsfundingagency.atlassian.net/wiki/spaces/ERF/pages/104010807/Staff+IDAMS
//        //services.AddWs (options ).UseWsFederationAuthentication(WsFederationAuthenticationOptions);
//        services.AddAuthentication().AddWsFederation(wsFederationOptions);

//        servicapp.Map($"/login/staff", SetAuthenticationContextForStaffUser());

//        //app.Map($"/login", SetAuthenticationContext());

//        return services;
//    }

//    private static WsFederationEvents Notifications()
//    {
//        return new WsFederationEvents
//        {
//            OnSecurityTokenValidated = OnSecurityTokenValidated,
//            OnSecurityTokenReceived = nx => OnSecurityTokenReceived(),
//            OnAuthenticationFailed = nx => OnAuthenticationFailed(nx),
//            OnMessageReceived = nx => OnMessageReceived(),
//            OnRedirectToIdentityProvider = nx => OnRedirectToIdentityProvider()
//        };
//    }

//    private static Task OnSecurityTokenValidated(SecurityTokenValidatedContext notification)
//    {
//        Logger.LogDebug("SecurityTokenValidated");


//        var claimsIdentity = notification.Principal;
//        notification.

//        Logger.LogDebug("Authentication Properties", new Dictionary<string, object>
//        {
//            {"claims", JsonConvert.SerializeObject(claimsIdentity.Claims.Select(x =>new {x.Value, x.ValueType, x.Type}))},
//            {"authentication-type", claimsIdentity.AuthenticationType},
//            {"role-type", claimsIdentity.RoleClaimType}
//        });

//        if (claimsIdentity.HasClaim(serviceClaimType, ServiceClaimTier2Role))
//        {
//            Logger.LogDebug("Adding Tier2 Role");
//            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Tier2User));

//            Logger.LogDebug("Adding ConsoleUser Role");
//            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
//        }
//        else if (notification.AuthenticationTicket.Identity.HasClaim(serviceClaimType, ServiceClaimTier1Role))
//        {
//            Logger.LogDebug("Adding Tier1 Role");
//            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Tier1User));

//            Logger.LogDebug("Adding ConsoleUser Role");
//            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
//        }
//        else
//        {
//            throw new SecurityTokenValidationException("Service Claim Type not available to identify the Role");
//        }

//        var firstName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
//        var lastName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
//        var userEmail = claimsIdentity.Claims.Single(x => x.Type == ClaimTypes.Upn).Value;

//        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));

//        if (!string.IsNullOrEmpty(userEmail))
//        {
//            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, userEmail));
//            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userEmail));
//        }
//        else
//        {
//            throw new SecurityTokenValidationException("Upn not available in the claims to identify UserEmail");
//        }

//        Logger.LogDebug("End of callback");
//        return Task.FromResult(0);
//    }

//    private static Task OnSecurityTokenReceived()
//    {
//        Logger.LogDebug("SecurityTokenReceived");
//        return Task.FromResult(0);
//    }

//    private static Task OnAuthenticationFailed(AuthenticationFailedContext nx)
//    {
//        var logReport = $"AuthenticationFailed, State: {nx.Response.StatusCode}, Exception: {nx.Exception.GetBaseException().Message}, Protocol Message: Wct {nx.ProtocolMessage.Wct},\r\nWfresh {nx.ProtocolMessage.Wfresh},\r\nWhr {nx.ProtocolMessage.Whr},\r\nWp {nx.ProtocolMessage.Wp},\r\nWpseudo{nx.ProtocolMessage.Wpseudo},\r\nWpseudoptr {nx.ProtocolMessage.Wpseudoptr},\r\nWreq {nx.ProtocolMessage.Wreq},\r\nWfed {nx.ProtocolMessage.Wfed},\r\nWreqptr {nx.ProtocolMessage.Wreqptr},\r\nWres {nx.ProtocolMessage.Wres},\r\nWreply{nx.ProtocolMessage.Wreply},\r\nWencoding {nx.ProtocolMessage.Wencoding},\r\nWtrealm {nx.ProtocolMessage.Wtrealm},\r\nWresultptr {nx.ProtocolMessage.Wresultptr},\r\nWauth {nx.ProtocolMessage.Wauth},\r\nWattrptr{nx.ProtocolMessage.Wattrptr},\r\nWattr {nx.ProtocolMessage.Wattr},\r\nWa {nx.ProtocolMessage.Wa},\r\nIsSignOutMessage {nx.ProtocolMessage.IsSignOutMessage},\r\nIsSignInMessage {nx.ProtocolMessage.IsSignInMessage},\r\nWctx {nx.ProtocolMessage.Wctx},\r\n";
//        Logger.LogDebug(logReport);
//        return Task.FromResult(0);
//    }

//    private static Task OnMessageReceived()
//    {
//        Logger.LogDebug("MessageReceived");
//        return Task.FromResult(0);
//    }

//    private static Task OnRedirectToIdentityProvider()
//    {
//        Logger.LogDebug("RedirectToIdentityProvider");
//        return Task.FromResult(0);
//    }

//    //private static Action<IAppBuilder> SetAuthenticationContextForStaffUser()
//    //{
//    //    return conf =>
//    //    {
//    //        conf.Run(context =>
//    //        {
//    //            //for first iteration of this work, allow deep linking from the support console to the teams view
//    //            //as this is the only action they will currently perform.

//    //           var hashedAccountId = context.Request.Query.Get(HashedAccountId);
//    //            var requestRedirect = string.IsNullOrEmpty(hashedAccountId) ? "/service/index" : $"/accounts/{hashedAccountId}/teams/view";

//    //            context.Authentication.Challenge(new AuthenticationProperties
//    //            {
//    //                RedirectUri = requestRedirect,
//    //                IsPersistent = true
//    //            },
//    //                Staff);

//    //            context.Response.StatusCode = 401;
//    //            return context.Response.WriteAsync(string.Empty);
//    //        });
//    //    };
//    //}

//    //private static Action<IAppBuilder> SetAuthenticationContext()
//    //{
//    //    return conf =>
//    //    {
//    //        conf.Run(context =>
//    //        {
//    //            context.Authentication.Challenge(new AuthenticationProperties
//    //            {
//    //                RedirectUri = "/service/index",
//    //                IsPersistent = true
//    //            },
//    //                CookieAuthenticationDefaults.AuthenticationScheme);

//    //            context.Response.StatusCode = 401;
//    //            return context.Response.WriteAsync(string.Empty);
//    //        });
//    //    };
//    //}
//}

////public class SupportConsoleAuthenticationOptions
////{
////    public ADFSOptions AdfsOptions { get; set; }
////    public ILogger Logger { get; set; }
////}

////public class ADFSOptions
////{
////    public string Wtrealm { get; set; }
////    public string MetadataAddress { get; set; }
////    public string Wreply { get; set; }
////    public string BaseUrl { get; set; }
////}