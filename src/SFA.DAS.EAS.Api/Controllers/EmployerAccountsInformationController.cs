using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.EAS.Api.Models;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange;

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
            
            if (!DateTime.TryParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateConverted) ||
                !DateTime.TryParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateConverted) ||
                toDateConverted.Equals(DateTime.MinValue) || 
                fromDateConverted.Equals(DateTime.MinValue))
            {
                _logger.Info($"API AccountsInformation - Invalid dates entered fromDate:{fromDate} toDate:{toDate}");
                return BadRequest();
            }

            try
            {
                var results = await _mediator.SendAsync(new GetPagedEmployerAccountsByDateRangeQuery
                {
                    FromDate = fromDateConverted,
                    ToDate = toDateConverted,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

                var data = new List<AccountInformationViewModel>();
                data.AddRange(results.Accounts.AccountList.Select(result => new AccountInformationViewModel
                {
                    DasAccountName = result.DasAccountName,
                    DateRegistered = result.DateRegistered,
                    OrganisationRegisteredAddress = result.OrganisationRegisteredAddress,
                    OrganisationSource = result.OrganisationSource,
                    OrganisationStatus = result.OrganisationStatus,
                    OrganisationName = result.OrganisationName,
                    OwnerEmail = result.OwnerEmail,
                    OrgansiationCreatedDate = result.OrgansiationCreatedDate
                }));

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
    }
}
