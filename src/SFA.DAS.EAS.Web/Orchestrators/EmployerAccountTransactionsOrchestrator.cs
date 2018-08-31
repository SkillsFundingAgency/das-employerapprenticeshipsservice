﻿using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.FindAccountCoursePayments;
using SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
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

		public async Task<OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>>
			FindAccountLevyDeclarationTransactions(
				string hashedId, DateTime fromDate, DateTime toDate, string externalUserId)
		{
			var data = await _mediator.SendAsync(new FindEmployerAccountLevyDeclarationTransactionsQuery
			{
				HashedAccountId = hashedId,
				FromDate = fromDate,
				ToDate = toDate,
				ExternalUserId = externalUserId
			});

			foreach (var transaction in data.Transactions)
			{
				var payeSchemeData = await _mediator.SendAsync(new GetPayeSchemeByRefQuery
				{
					HashedAccountId = hashedId,
					Ref = transaction.EmpRef
				});

				transaction.PayeSchemeName = payeSchemeData?.PayeScheme?.Name ?? string.Empty;
			}

			return new OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>
			{
				Status = HttpStatusCode.OK,
				Data = new TransactionLineViewModel<LevyDeclarationTransactionLine>
				{
					Amount = data.Total,
					SubTransactions = data.Transactions,
					TransactionDate = data.Transactions.First().DateCreated
				}
			};
		}

		public async Task<OrchestratorResponse<ProviderPaymentsSummaryViewModel>> GetProviderPaymentSummary(
			string hashedId, long ukprn, DateTime fromDate, DateTime toDate, string externalUserId)
		{
			try
			{
				var data = await _mediator.SendAsync(new FindAccountProviderPaymentsQuery
				{
					HashedAccountId = hashedId,
					UkPrn = ukprn,
					FromDate = fromDate,
					ToDate = toDate,
					ExternalUserId = externalUserId,
				});

				var courseGroups =
					data.Transactions.GroupBy(x => new {x.CourseName, x.CourseLevel, x.PathwayName, x.CourseStartDate});

				var coursePaymentSummaries = courseGroups.Select(x =>
				{
					var levyPayments = x.Where(p => p.TransactionType == TransactionItemType.Payment).ToList();

					return new CoursePaymentSummaryViewModel
					{
						CourseName = x.Key.CourseName,
						CourseLevel = x.Key.CourseLevel,
						PathwayName = x.Key.PathwayName,
						PathwayCode = levyPayments.Max(p => p.PathwayCode),
						CourseStartDate = x.Key.CourseStartDate,
						LevyPaymentAmount = levyPayments.Sum(p => p.LineAmount),
						EmployerCoInvestmentAmount = levyPayments.Sum(p => p.EmployerCoInvestmentAmount),
						SFACoInvestmentAmount = levyPayments.Sum(p => p.SfaCoInvestmentAmount)
					};
				}).ToList();

				return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
				{
					Status = HttpStatusCode.OK,
					Data = new ProviderPaymentsSummaryViewModel
					{
						HashedAccountId = hashedId,
						UkPrn = ukprn,
						ProviderName = data.ProviderName,
						PaymentDate = data.DateCreated,
						FromDate = fromDate,
						ToDate = toDate,
						CoursePayments = coursePaymentSummaries,
						LevyPaymentsTotal = coursePaymentSummaries.Sum(p => p.LevyPaymentAmount),
						SFACoInvestmentsTotal = coursePaymentSummaries.Sum(p => p.SFACoInvestmentAmount),
						EmployerCoInvestmentsTotal = coursePaymentSummaries.Sum(p => p.EmployerCoInvestmentAmount),
						PaymentsTotal = coursePaymentSummaries.Sum(p => p.TotalAmount)
					}
				};
			}
			catch (NotFoundException e)
			{
				return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
				{
					Status = HttpStatusCode.NotFound,
					Exception = e
				};
			}
			catch (InvalidRequestException e)
			{
				return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
				{
					Status = HttpStatusCode.BadRequest,
					Exception = e
				};
			}
			catch (UnauthorizedAccessException e)
			{
				return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
				{
					Status = HttpStatusCode.Unauthorized,
					Exception = e
				};
			}
		}

		public async Task<OrchestratorResponse<PaymentTransactionViewModel>> FindAccountPaymentTransactions(
			string hashedId, long ukprn, DateTime fromDate, DateTime toDate, string externalUserId)
		{
			try
			{
				var data = await _mediator.SendAsync(new FindAccountProviderPaymentsQuery
				{
					HashedAccountId = hashedId,
					UkPrn = ukprn,
					FromDate = fromDate,
					ToDate = toDate,
					ExternalUserId = externalUserId
				});

				var courseGroups = data.Transactions.GroupBy(x => new {x.CourseName, x.CourseLevel, x.CourseStartDate});

				var coursePaymentGroups = courseGroups.Select(x => new ApprenticeshipPaymentGroup
				{
					ApprenticeCourseName = x.Key.CourseName,
					CourseLevel = x.Key.CourseLevel,
					CourseStartDate = x.Key.CourseStartDate,
					Payments = x.ToList()
				}).ToList();


				return new OrchestratorResponse<PaymentTransactionViewModel>
				{
					Status = HttpStatusCode.OK,
					Data = new PaymentTransactionViewModel
					{
						ProviderName = data.ProviderName,
						TransactionDate = data.TransactionDate,
						Amount = data.Total,
						SubTransactions = data.Transactions,
						CoursePaymentGroups = coursePaymentGroups
					}
				};
			}
			catch (NotFoundException e)
			{
				return new OrchestratorResponse<PaymentTransactionViewModel>
				{
					Status = HttpStatusCode.NotFound,
					Exception = e
				};
			}
			catch (InvalidRequestException e)
			{
				return new OrchestratorResponse<PaymentTransactionViewModel>
				{
					Status = HttpStatusCode.BadRequest,
					Exception = e
				};
			}
			catch (UnauthorizedAccessException e)
			{
				return new OrchestratorResponse<PaymentTransactionViewModel>
				{
					Status = HttpStatusCode.Unauthorized,
					Exception = e
				};
			}
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