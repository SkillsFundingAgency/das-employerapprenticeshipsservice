using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(IMediator mediator, ILogger logger): base(mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> Create(
            string hashedId, string externalUserId, string name, string code, string address, DateTime incorporatedDate)
        {
            var response = new OrchestratorResponse<EmployerAgreementViewModel>();

            try
            {
                var request = new GetLatestEmployerAgreementTemplateRequest
                {
                    HashedAccountId = hashedId,
                    UserId = externalUserId
                };

                var templateResponse = await _mediator.SendAsync(request);

                response.Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        LegalEntityName = name,
                        LegalEntityCode = code,
                        LegalEntityRegisteredAddress = address,
                        LegalEntityIncorporatedDate = incorporatedDate,
                        Status = EmployerAgreementStatus.Pending,
                        TemplateRef = templateResponse.Template.Ref,
                        TemplateText = templateResponse.Template.Text
                    }
                };
            }
            catch (UnauthorizedAccessException)
            {
                response.Status = HttpStatusCode.Unauthorized;
            }
            catch (InvalidRequestException)
            {
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }

        public async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(string hashedId,
            string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Data = new EmployerAgreementListViewModel
                    {
                        HashedAccountId = hashedId,
                        EmployerAgreements = response.EmployerAgreements
                    }
                };
            }
            catch (Exception)
            {
                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetById(
            string agreementid, string hashedId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAgreementRequest
                {
                    HashedAgreementId = agreementid,
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = response.EmployerAgreement
                    }
                };
            }
            catch (Exception)
            {
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetByLegalEntityCode(
            long accountId, string legalEntityCode, string externalUserId)
        {
            var response = await _mediator.SendAsync(new GetLegalEntityAgreementRequest
            {
                AccountId = accountId,
                LegalEntityCode = legalEntityCode
            });

            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = response.EmployerAgreement
                }
            };
        }

        public async Task<OrchestratorResponse> SignAgreement(string agreementid, string hashedId, string externalUserId,
            DateTime signedDate)
        {
            try
            {
                await _mediator.SendAsync(new SignEmployerAgreementCommand
                {
                    HashedAgreementId = agreementid,
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId,
                    SignedDate = signedDate
                });

                return new OrchestratorResponse();
            }
            catch (Exception)
            {
                return new OrchestratorResponse
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

        public virtual async Task<OrchestratorResponse<FindOrganisationViewModel>> FindLegalEntity(string hashedLegalEntityId, OrganisationType orgType,
            string searchTerm, string userIdClaim)
        {
            var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedLegalEntityId,
                UserId = userIdClaim
            });


            //todo: if code is the same AND of the same type
            if (accountEntities.Entites.LegalEntityList.Any(
                x => x.Code.Equals(searchTerm, StringComparison.CurrentCultureIgnoreCase)))
            {
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindOrganisationViewModel(),
                    Status = HttpStatusCode.Conflict
                };
            }

            switch (orgType)
            {
                case OrganisationType.Charities:
                    return await FindCharity(hashedLegalEntityId, searchTerm);
                case OrganisationType.CompaniesHouse:
                    return await FindLimitedCompany(hashedLegalEntityId, searchTerm);
                case OrganisationType.PublicBodies:
                    return await FindPublicBody(hashedLegalEntityId, searchTerm);
                default:
                    throw new NotImplementedException("Organisation Type Not Implemented");
            }



        }

        private async Task<OrchestratorResponse<FindOrganisationViewModel>> FindLimitedCompany(string hashedLegalEntityId, string companiesHouseNumber)
        {

            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = companiesHouseNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindOrganisationViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            _logger.Info($"Returning Data for {companiesHouseNumber}");

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindOrganisationViewModel
                {
                    HashedLegalEntityId = hashedLegalEntityId,
                    CompanyNumber = response.CompanyNumber,
                    CompanyName = response.CompanyName,
                    DateOfIncorporation = response.DateOfIncorporation,
                    RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}",
                    CompanyStatus = response.CompanyStatus
                }
            };
        }


        private async Task<OrchestratorResponse<FindOrganisationViewModel>> FindPublicBody(string hashedLegalEntityId, string searchTerm)
        {
            var publicBodyResult = await _mediator.SendAsync(new GetPublicSectorOrganisationQuery()
            {
                SearchTerm = searchTerm,
                PageNumber = 1,
                PageSize = 1000
            });

            if (publicBodyResult == null || !publicBodyResult.Organisaions.Data.Any())
            {
                _logger.Warn("No response from GetPublicSectorOrgainsationQuery");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindPublicBodyViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            var publicBody = publicBodyResult.Organisaions.Data.First();

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindPublicBodyViewModel
                {
                    Results = publicBodyResult.Organisaions,
                    CompanyName = publicBody.Name
                },
                Status = HttpStatusCode.OK
            };
           
        }

        private async Task<OrchestratorResponse<FindOrganisationViewModel>> FindCharity(string hashedLegalEntityId, string registrationNumber)
        {
            int charityRegistrationNumber;
            if (!Int32.TryParse(registrationNumber, out charityRegistrationNumber))
            {
                _logger.Warn("Non-numeric registration number");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            GetCharityQueryResponse charityResult;
            try
            {
                charityResult = await _mediator.SendAsync(new GetCharityQueryRequest
                {
                    RegistrationNumber = charityRegistrationNumber
                });
            }
            catch (HttpRequestException)
            {
                _logger.Warn("Charity not found");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            if (charityResult == null)
            {
                _logger.Warn("No response from GetAccountLegalEntitiesRequest");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel(),
                    Status = HttpStatusCode.NotFound
                };
            }

            var charity = charityResult.Charity;

            if (charity.IsRemoved)
            {
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindCharityViewModel
                    {
                        IsRemovedError = true
                    },
                    Status = HttpStatusCode.NotFound
                };
            }

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindCharityViewModel
                {
                    HashedLegalEntityId = hashedLegalEntityId,
                    CompanyNumber = charity.RegistrationNumber.ToString(),
                    CompanyName = charity.Name,
                    RegisteredAddress = $"{charity.Address1}, {charity.Address2}, {charity.Address3}, {charity.Address4}, {charity.Address5}",
                }
            };
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(
            CreateNewLegalEntity request)
        {
            if (request.SignedAgreement && !request.UserIsAuthorisedToSign)
            {
                var response = await _mediator.SendAsync(new GetLatestEmployerAgreementTemplateRequest
                {
                    HashedAccountId = request.HashedAccountId,
                    UserId = request.ExternalUserId
                });

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            LegalEntityName = request.Name,
                            LegalEntityCode = request.Code,
                            LegalEntityRegisteredAddress = request.Address,
                            LegalEntityIncorporatedDate = request.IncorporatedDate,
                            Status = EmployerAgreementStatus.Pending,
                            TemplateRef = response.Template.Ref,
                            TemplateText = response.Template.Text,
                            LegalEntityStatus = request.LegalEntityStatus
                        }
                    },
                    Status = HttpStatusCode.BadRequest
                };
            }

            var createLegalEntityResponse = await _mediator.SendAsync(new CreateLegalEntityCommand
            {
                HashedAccountId = request.HashedAccountId,
                LegalEntity = new LegalEntity
                {
                    Name = request.Name,
                    Code = request.Code,
                    RegisteredAddress = request.Address,
                    DateOfIncorporation = request.IncorporatedDate,
                    CompanyStatus = request.LegalEntityStatus
                },
                SignAgreement = request.UserIsAuthorisedToSign && request.SignedAgreement,
                SignedDate = request.SignedDate,
                ExternalUserId = request.ExternalUserId
            });

            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = createLegalEntityResponse.AgreementView
                },
                Status = HttpStatusCode.OK
            };
        }

        public async Task<OrchestratorResponse<AddLegalEntityViewModel>> GetAddLegalEntityViewModel(string hashedAccountId, string externalUserId)
        {
            var userRole = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = userRole.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized 
            };

        }
    }
}