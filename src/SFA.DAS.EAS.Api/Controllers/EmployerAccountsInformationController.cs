using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accountsinformation")]
    public class EmployerAccountsInformationController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public EmployerAccountsInformationController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("", Name = "AccountsInformationIndex")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> Index(string fromDate, string toDate, int pageSize = 1000, int pageNumber = 1)
        {
            DateTime fromDateConverted;
            DateTime toDateConverted;

            if (!ValidateAndConvertDates(fromDate, toDate, out fromDateConverted, out toDateConverted))
            {
                _logger.Info($"API AccountsInformation - Invalid dates entered fromDate:{fromDate} toDate:{toDate}");
                return BadRequest();
            }

            try
            {
                var results = await _mediator.SendAsync(new GetPagedEmployerAccountsByDateRangeQuery
                {
                    FromDate = fromDateConverted,
                    ToDate = toDateConverted.AddDays(1).AddSeconds(-1),
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

                var data = new List<AccountInformationViewModel>();
                data.AddRange(results.Accounts.AccountList.Select(ConvertAccountInformationToViewModel));

                var returnResult = new PagedApiResponseViewModel<AccountInformationViewModel>
                {
                    Data = data,
                    Page = pageNumber,
                    TotalPages = (results.Accounts.AccountsCount/pageSize) + 1
                };
                return Ok(returnResult);
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, "Invalid Request for EmployerAccountsInformationController");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            
            return BadRequest();
        }

        private static AccountInformationViewModel ConvertAccountInformationToViewModel(AccountInformation result)
        {
            return new AccountInformationViewModel
            {
                DasAccountName = result.DasAccountName,
                DateRegistered = result.DateRegistered,
                OrganisationRegisteredAddress = result.OrganisationRegisteredAddress,
                OrganisationSource = result.OrganisationSource,
                OrganisationStatus = result.OrganisationStatus,
                OrganisationName = result.OrganisationName,
                OwnerEmail = result.OwnerEmail,
                OrgansiationCreatedDate = result.OrgansiationCreatedDate,
                DasAccountId = result.DasAccountId,
                OrganisationNumber = result.OrganisationNumber,
                PayeSchemeName= result.PayeSchemeName,
                OrganisationId = result.OrganisationId
            };
        }

        private bool ValidateAndConvertDates(string fromDate, string toDate, out DateTime fromDateConverted, out DateTime toDateConverted)
        {
            toDateConverted = DateTime.Now;

            if (!DateTime.TryParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateConverted) ||
                !DateTime.TryParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateConverted) ||
                toDateConverted.Equals(DateTime.MinValue) || fromDateConverted.Equals(DateTime.MinValue))
            {
                return false;
            }
            return true;
        }
    }
}
