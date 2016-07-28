using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class VisualTestController : Controller
    {
        private List<string> _blacklist = new List<string>
        {
            "~/Views/Home/About.cshtml",
            "~/Views/Home/Contact.cshtml",
            "~/Views/VisualTest/Index.cshtml"
        };

        private AggregationLineItem aggregationLineItem;
        private AggregationLine aggregationLine;
        private InvitationView invitationView;
        private Dictionary<string, object> _viewToModel;

        public VisualTestController()
        {
            aggregationLineItem = new AggregationLineItem()
            {
                Id = 2,
                ActivityDate = new DateTime(2016, 05, 02),
                Amount = 12,
                EmpRef = "emp-123",
                EnglishFraction = 0.3m,
                LevyItemType = LevyItemType.TopUp
            };

            invitationView = new InvitationView()
            {
                AccountName = "My account",
                Name = "Bojack Horseman",
                Status = InvitationStatus.Pending,
                Id = 123,
                Email = "a@b.com",
                ExpiryDate = new DateTime(2017, 1, 1),
                ExternalUserId = Guid.NewGuid(),
                InternalUserId = 123,
                RoleId = 213,
                RoleName = "Chief Executor"
            };
            aggregationLine = new AggregationLine
            {
                Amount = 12m,
                Id = "1",
                Balance = 12m,
                LevyItemType = LevyItemType.TopUp,
                Year = 2016,
                Month = 5,
                Items = new List<AggregationLineItem>
                {
                    aggregationLineItem,
                    aggregationLineItem,
                    aggregationLineItem
                }

            };
            _viewToModel = new Dictionary<string, object>
            {
                {"~/Views/EmployerAccount/Summary.cshtml", new SummaryViewModel()
                    {
                        CompanyName = "sushiCorp Ltd.",
                        CompanyNumber = "1234567890",
                        DateOfIncorporation = new DateTime(2016, 05, 16),
                        EmployerRef = "emp-123",
                        RegisteredAddress = "123 Fake St."
                    }},
                {"~/Views/EmployerAccount/VerifyEmployer.cshtml", new SelectEmployerViewModel()
                    {
                        CompanyName = "sushiCorp Ltd.",
                        CompanyNumber = "0123456789",
                        DateOfIncorporation = new DateTime(2016, 05, 16),
                        RegisteredAddress = "123 Fake St."
                    }},
                {"~/Views/EmployerAccountTransactions/Index.cshtml", new TransactionViewModel() {
                    CurrentBalance = 12m,
                    CurrentBalanceCalcultedOn = new DateTime(2016,05,16),
                    Data = new AggregationData()
                    {
                        AccountId = 1234567890,
                        Data = new List<AggregationLine>
                        {
                            aggregationLine,
                            aggregationLine,
                            aggregationLine
                        }
                    }
                }},
                {"~/Views/EmployerAccountTransactions/Detail.cshtml", new TransactionLineItemViewModel()
                    {
                        CurrentBalance = 12.0m,
                        CurrentBalanceCalcultedOn = new DateTime(2016, 05, 16),
                        LineItem = aggregationLine
                    }},
                {"~/Views/EmployerTeam/Index.cshtml", new EmployerTeamMembersViewModel()
                    {
                        AccountId = 1234567890,
                        TeamMembers = new List<TeamMember>()
                        {
                            new TeamMember() {AccountId = 1234567890, Email = "a.b@com", Id=123, Role = Role.Owner, Status = InvitationStatus.Pending, UserRef = "ab-123" }
                        }
                    }},
                {"~/Views/EmployerTeam/Cancel.cshtml", invitationView},
                {"~/Views/EmployerTeam/Invite.cshtml", new InviteTeamMemberViewModel() {
                        AccountId = 123,
                        Email = "bojack.horseman@horsingaround.com",
                        Name = "Bocjack Horseman",
                        Role = Role.Owner
                } },
                {"~/Views/EmployerTeam/Review.cshtml", new InvitationViewModel() {
                    AccountId = 123,
                    Email = "bojack.horseman@horsingaround.com",
                    Name = "Bocjack Horseman",
                    Role = Role.Owner,
                    Id = 1234,
                    ExpiryDate = new DateTime(2017,4,1),
                    IsUser = false,
                    Status = InvitationStatus.Pending
                } },
                {"~/Views/Home/FakeUserSignIn.cshtml", new SignInUserViewModel ()
                    {
                        AvailableUsers = new List<SignInUserModel>() {
                            new SignInUserModel() {Email = "a@b.com", FirstName = "Bojack", LastName = "Horseman", UserId="123", UserSelected= "foo" }
                        }
                    }},
                {"~/Views/Home/Index.cshtml", new UserAccountsViewModel()
                    {
                        Accounts = new Accounts()
                        {
                            AccountList = new List<Account>() {
                                new Account() {Id = 123, Name= "My account" }
                            }
                        },
                        Invitations = new List<InvitationView>()
                        {
                            invitationView,
                            invitationView,
                            invitationView
                        }
                    }},
                {"~/Views/Invitation/Index.cshtml", invitationView}
            };
        }
        

        // GET: VisualTest
        public ActionResult Index()
        {
            var dir = Directory.GetFiles(string.Format("{0}/Views", HostingEnvironment.ApplicationPhysicalPath),
            "*.cshtml", SearchOption.AllDirectories);

            ViewData["Views"] = dir
                .Where(x => !x.Contains("_"))
                .Select(x => "~" + x
                    .Replace(HostingEnvironment.ApplicationPhysicalPath, String.Empty)
                    .Replace("\\", "/"))
                .Where(x => !_blacklist.Contains(x))
                .ToList();

            ViewData["ViewToModel"] = _viewToModel;
            return View();
        }
    }
}