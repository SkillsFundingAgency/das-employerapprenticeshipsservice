using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.ViewModels;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class VisualTestController : Controller
    {
        private List<string> _blacklist = new List<string>
        {
            "~/Views/Home/About.cshtml",
            "~/Views/Home/Contact.cshtml",
            "~/Views/VisualTest/Index.cshtml"
        };

        private Dictionary<string, object> _viewToModel;

        public VisualTestController()
        {
            var aggregationLineItem = new AggregationLineItem()
            {
                Id = 2,
                ActivityDate = new DateTime(2016, 05, 02),
                Amount = 12,
                EmpRef = "emp-123",
                EnglishFraction = 0.3m,
                LevyItemType = LevyItemType.TopUp
            };

            var invitationView = new InvitationView()
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
                RoleName = "Owner"
            };
            var aggregationLine = new AggregationLine
            {
                Amount = 12m,
                Id = "12345",
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

            var invitationViewModel = new InvitationViewModel()
            {
                AccountId = 123,
                Email = "bojack.horseman@horsingaround.com",
                Name = "Bocjack Horseman",
                Role = Role.Owner,
                Id = 1234,
                ExpiryDate = new DateTime(2017, 4, 1),
                IsUser = false,
                Status = InvitationStatus.Pending
            };

            var teamMember = new TeamMember()
            {
                Name = "Bojack Horseman",
                AccountId = 1234567890,
                Email = "a.b@com",
                Id = 123,
                Role = Role.Owner,
                Status = InvitationStatus.Pending,
                UserRef = "ab-123"
            };

            var payeView = new PayeView()
            {
                AccountId = 1234567890,
                LegalEntityName = "My account",
                EmpRef = "empref-39520"
            };

            var employerAccountPayeListViewModel = new EmployerAccountPayeListViewModel()
            {
                
                PayeSchemes = new List<PayeView>()
                {
                    payeView,
                    payeView,
                    payeView
                }
            };

            var userInvitationsViewModel = new UserInvitationsViewModel()
            {
                Invitations = new List<InvitationView>
                {
                    invitationView,
                    invitationView,
                    invitationView
                }
            };

            var orchestratorResponse = new OrchestratorResponse()
            {
                Status = System.Net.HttpStatusCode.Forbidden,
                Exception = new Exception("Too many spiders")
            };

            var employerTeamMembersViewModel = new EmployerTeamMembersViewModel()
            {
                HashedId = "1234567890",
                SuccessMessage = "It successfully applied the cream",
                TeamMembers = new List<TeamMember>
                {
                    teamMember,
                    teamMember,
                    teamMember
                }
            };

            var legalEntity = new LegalEntity()
            {
                Code = "MyLegalEntityCode",
                DateOfIncorporation = new DateTime(2009, 07, 01),
                Id = 123,
                Name = "My little legal entity",
                RegisteredAddress = "123 Fake street"
            };

            var confirmNewPayeScheme = new ConfirmNewPayeScheme()
            {
                AccessToken = "MyAccessToken",
                
                LegalEntities = new List<LegalEntity>
                        {
                            legalEntity,
                            legalEntity,
                            legalEntity
                        },
                LegalEntityCode = "MyLegalEntityCode",
                LegalEntityDateOfIncorporation = new DateTime(2009, 07, 01),
                LegalEntityId = 123,
                LegalEntityName = "My little legal entity",
                LegalEntityRegisteredAddress = "123 Fake street",
                PayeScheme = "mypaye-123",
                RefreshToken = "refresh-123"
            };

            var addNewPayeScheme = new AddNewPayeScheme()
            {
                AccessToken = "MyAccessToken",
                
                LegalEntities = new List<LegalEntity> { legalEntity, legalEntity, legalEntity },
                PayeScheme = "mypaye-123",
                RefreshToken = "refresh-123"
            };

            var employerAgreement = new EmployerAgreementView()
            {
                 
                Id = 012345678,
                LegalEntityId = 123,
                LegalEntityName = "My little legal entity",
                LegalEntityRegisteredAddress = "123 Fake Street",
                SignedByName = "Bojack Horseman",
                SignedDate = new DateTime(2016, 8, 12),
                Status = EmployerAgreementStatus.Pending,
                TemplateId = 3,
                TemplateText = "The good old template"
            };

            var employerAgreementViewModel = new EmployerAgreementViewModel()
            {
                EmployerAgreement = employerAgreement
            };

            var employerAgreementListViewModel = new EmployerAgreementListViewModel()
            {
                AccountId = 0123456789,
                EmployerAgreements = new List<EmployerAgreementView>
                {
                    employerAgreement,
                    employerAgreement,
                    employerAgreement
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
                {"~/Views/EmployerAccountPaye/Add.cshtml", new OrchestratorResponse<BeginNewPayeScheme> {Data = new BeginNewPayeScheme { HashedId = "3", ValidationFailed = true } } },
                {"~/Views/EmployerAccountPaye/AddNewLegalEntity.cshtml", confirmNewPayeScheme},
                {"~/Views/EmployerAccountPaye/ChooseCompany.cshtml", addNewPayeScheme },
                {"~/Views/EmployerAccountPaye/Confirm.cshtml", confirmNewPayeScheme },
                {"~/Views/EmployerAccountPaye/ConfirmPayeScheme.cshtml", new OrchestratorResponse<AddNewPayeScheme>() {Data = addNewPayeScheme } },
                {"~/Views/EmployerAccountPaye/ErrorConfrimPayeScheme.cshtml", new OrchestratorResponse<AddNewPayeScheme>() {Data = addNewPayeScheme } },
                {"~/Views/EmployerAccountPaye/Index.cshtml", new OrchestratorResponse<EmployerAccountPayeListViewModel>()
                {
                    Data = new EmployerAccountPayeListViewModel()
                    {
                        
                        PayeSchemes = new List<PayeView>()
                        {
                            payeView,
                            payeView,
                            payeView
                        }
                    }
                }},
                {"~/Views/EmployerAgreement/Index.cshtml", new OrchestratorResponse<EmployerAgreementListViewModel> {Data =  employerAgreementListViewModel } },
                {"~/Views/EmployerAgreement/View.cshtml", new OrchestratorResponse<EmployerAgreementViewModel> {Data = employerAgreementViewModel } },
                {"~/Views/EmployerTeam/Index.cshtml", new OrchestratorResponse<Account> {
                    Data = new Account() {
                        Id = 1234567890,
                        Name = "My account",
                        RoleId = 2
                    }}
                },
                {"~/Views/EmployerTeam/Cancel.cshtml", invitationView},
                {"~/Views/EmployerTeam/ChangeRole.cshtml", teamMember},
                {"~/Views/EmployerTeam/Invite.cshtml", new InviteTeamMemberViewModel() {
                        HashedId = "123",
                        Email = "bojack.horseman@horsingaround.com",
                        Name = "Bocjack Horseman",
                        Role = Role.Owner
                } },
                {"~/Views/EmployerTeam/Review.cshtml", invitationViewModel },
                {"~/Views/EmployerTeam/Remove.cshtml", invitationViewModel },
                {"~/Views/EmployerTeam/View.cshtml", new OrchestratorResponse<EmployerTeamMembersViewModel> {Data= employerTeamMembersViewModel }},
                {"~/Views/Home/FakeUserSignIn.cshtml", new SignInUserViewModel ()
                    {
                        AvailableUsers = new List<SignInUserModel>() {
                            new SignInUserModel() {Email = "a@b.com", FirstName = "Bojack", LastName = "Horseman", UserId="123", UserSelected= "foo" }
                        }
                    }},
                {"~/Views/Home/Index.cshtml", new OrchestratorResponse<UserAccountsViewModel>()
                    {
                        Data = new UserAccountsViewModel() {
                                Accounts = new Accounts()
                                {
                                    AccountList = new List<Account>() {
                                        new Account() {Id = 123, Name= "My account", RoleId = 1 }
                                    }
                                },
                                Invitations = 3
                            },

                    }
                },
                {"~/Views/Invitation/Index.cshtml", invitationView },
                {"~/Views/Invitation/All.cshtml", userInvitationsViewModel },
                {"~/Views/Invitation/View.cshtml", invitationView },
                {"~/Views/Shared/AccessDenied.cshtml", new OrchestratorResponse()},
                {"~/Views/Shared/GenericError.cshtml", orchestratorResponse }

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

        //GET: Notification
        public ActionResult Notification()
        {
            return View(new OrchestratorResponse() {
                FlashMessage = new FlashMessageViewModel()
                                {
                                    Headline = "sushiCorp Ltd.",
                                    Message = "You have reached peak tempura",
                                    SubMessage = "Congratulation!!1",
                                    Severity = FlashMessageSeverityLevel.Success
                                }
            });
        }
    }
}