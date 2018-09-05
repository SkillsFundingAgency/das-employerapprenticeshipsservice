using MediatR;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Activities.Client;
using SFA.DAS.Validation;
using TransactionLine = SFA.DAS.EAS.Domain.Models.Transaction.TransactionLine;

namespace SFA.DAS.EAS.Web.Orchestrators
{
	public class EmployerAccountTransactionsOrchestrator
	{
		private readonly ICurrentDateTime _currentTime;
		private readonly ILog _logger;
		private readonly IMediator _mediator;

		protected EmployerAccountTransactionsOrchestrator()
		{

		}

		public EmployerAccountTransactionsOrchestrator(IMediator mediator, ICurrentDateTime currentTime, ILog logger)
		{
			_mediator = mediator;
			_currentTime = currentTime;
			_logger = logger;
		}
       

		public virtual async Task<OrchestratorResponse<FinanceDashboardViewModel>> GetFinanceDashboardViewModel(
			string hashedId, int year, int month, string externalUserId)
		{
			var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
			{
				HashedAccountId = hashedId,
				UserId = externalUserId
			});

			if (employerAccountResult.Account == null)
			{
				return new OrchestratorResponse<FinanceDashboardViewModel>
				{
					Data = new FinanceDashboardViewModel()
				};
			}

			employerAccountResult.Account.HashedId = hashedId;

			var data =
				await
					_mediator.SendAsync(new GetEmployerAccountTransactionsQuery
					{
						ExternalUserId = externalUserId,
						Year = year,
						Month = month,
						HashedAccountId = hashedId
					});


			var latestLineItem = data?.Data?.TransactionLines?.FirstOrDefault();

			var currentBalance = latestLineItem?.Balance ?? 0;

			return new OrchestratorResponse<FinanceDashboardViewModel>
			{
				Data = new FinanceDashboardViewModel
				{
					Account = employerAccountResult.Account,
					CurrentLevyFunds = currentBalance
				}
			};
		}


		public virtual async Task<OrchestratorResponse<TransactionViewResultViewModel>> GetAccountTransactions(
			string hashedId, int year, int month, string externalUserId)
		{
			var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
			{
				HashedAccountId = hashedId,
				UserId = externalUserId
			});

			if (employerAccountResult.Account == null)
			{
				return new OrchestratorResponse<TransactionViewResultViewModel>
				{
					Data = new TransactionViewResultViewModel(_currentTime.Now)
				};
			}

			var aggregratedTransactions =
				await
					_mediator.SendAsync(new GetEmployerAccountTransactionsQuery
					{
						ExternalUserId = externalUserId,
						Year = year,
						Month = month,
						HashedAccountId = hashedId
					});

			var viewModel = BuildTransactionViewModel(aggregratedTransactions.Data, year, month);

			return new OrchestratorResponse<TransactionViewResultViewModel>
			{
				Data = new TransactionViewResultViewModel(_currentTime.Now)
				{
					Account = employerAccountResult.Account,
					Model = viewModel,
					Month = aggregratedTransactions.Month,
					Year = aggregratedTransactions.Year,
					AccountHasPreviousTransactions = aggregratedTransactions.AccountHasPreviousTransactions
				}
			};
		}

		private TransactionViewModel BuildTransactionViewModel(AggregationData aggregationData, int year, int month)
		{
			var viewModel = new TransactionViewModel
			{
				Data = new AggregationData
				{
					AccountId = aggregationData.AccountId,
					HashedAccountId = aggregationData.HashedAccountId,
				}
			};
			SetCurrentBalance(viewModel, aggregationData.TransactionLines, year, month);
			SetTransactionLines(viewModel, aggregationData.TransactionLines);
			return viewModel;
		}

		private void SetCurrentBalance(TransactionViewModel viewModel, ICollection<TransactionLine> transactionLines, int year, int month)
		{
			var latestLineItem = transactionLines.FirstOrDefault();

			if (latestLineItem != null)
			{
				viewModel.CurrentBalance = latestLineItem.Balance;
				viewModel.CurrentBalanceCalcultedOn = latestLineItem.TransactionDate;
			}
			else
			{
				viewModel.CurrentBalance = 0;
				viewModel.CurrentBalanceCalcultedOn = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
			}
		}

		private void SetTransactionLines(TransactionViewModel viewModel, ICollection<TransactionLine> transactionLines)
		{
			viewModel.Data.TransactionLines = AggregateLevyTransactions(transactionLines);
		}

		private static ICollection<TransactionLine> AggregateLevyTransactions(ICollection<TransactionLine> transactions)
		{
			var result = new List<TransactionLine>();

			var aggregatedLevyTransactions = transactions
				.Where(t => t.TransactionType == TransactionItemType.Declaration)
				.GroupBy(t => t.DateCreated.Date)
				.Select(grp =>
				{
					var firstLevyTransactionInDay = grp.First();
					return new TransactionLine
					{
						AccountId = firstLevyTransactionInDay.AccountId,
						DateCreated = firstLevyTransactionInDay.DateCreated,
						Balance = firstLevyTransactionInDay.Balance,
						Amount = grp.Sum(ltl => ltl.Amount),
						TransactionType = TransactionItemType.Declaration,
						Description = firstLevyTransactionInDay.Description,
						PayrollDate = firstLevyTransactionInDay.PayrollDate,
						PayrollMonth = firstLevyTransactionInDay.PayrollMonth,
						PayrollYear = firstLevyTransactionInDay.PayrollYear
					};
				});

			return transactions
				.Where(t => t.TransactionType != TransactionItemType.Declaration)
				.Union(aggregatedLevyTransactions)
				.ToList();
		}
	}
}