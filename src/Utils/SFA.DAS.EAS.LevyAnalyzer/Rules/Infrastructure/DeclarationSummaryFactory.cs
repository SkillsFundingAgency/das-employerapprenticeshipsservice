using System;
using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.ExtensionMethods;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    public class DeclarationSummaryFactory : IDeclarationSummaryFactory
    {
        private readonly IHmrcDateService _hmrcDateService;

        private class StateEvaluationDetails
        {
            public StateEvaluationDetails()
            {
                State = DeclarationState.Normal;
            }

            public Account Account { get; set; }
            public LevyDeclaration Declaration { get; set; }
            public TransactionLine Transaction { get; set; }
            public int Index { get; set; }
            public DeclarationState State { get; private set; }

            public void AddState(DeclarationState state)
            {
                State = State | state;
            }
        }

        private readonly Predicate<StateEvaluationDetails>[] _stateEvaluators;

        public DeclarationSummaryFactory(IHmrcDateService hmrcDateService)
        {
            _hmrcDateService = hmrcDateService;

            // Load delegates for assessing declaration state.
            _stateEvaluators = new Predicate<StateEvaluationDetails>[]
            {
                IsInvalid,
                IsMissingTransaction,
                IsLate,
                IsPeriod12,
                IsSuperseded
            };
        }

        public DeclarationSummary Create(Account account, int index)
        {
            var declaration = account.LevyDeclarations[index];
            account.TryGetMatchingTransaction(declaration, out var matchingTransaction);

            var evaluationDetails = new StateEvaluationDetails
            {
                Account = account,
                Declaration = declaration,
                Transaction = matchingTransaction,
                Index = index
            };

            // Chain-of-responsibility - evaluate all until one returns false
            var allEvaluated = _stateEvaluators.All(evaluator => evaluator(evaluationDetails));

            var declarationSummary = new DeclarationSummary(
                evaluationDetails.Declaration, 
                evaluationDetails.Transaction, 
                evaluationDetails.State);

            return declarationSummary;
        }

        private bool IsInvalid(StateEvaluationDetails stateEvaluationDetails)
        {
            if (!stateEvaluationDetails.Declaration.IsValid())
            {
                stateEvaluationDetails.AddState(DeclarationState.Invalid);
                return false;
            }

            return true;
        }

        private bool IsMissingTransaction(StateEvaluationDetails stateEvaluationDetails)
        {
            if (stateEvaluationDetails.Transaction == null)
            {
                stateEvaluationDetails.AddState(DeclarationState.NoTransaction);
            }

            return true;
        }

        private bool IsLate(StateEvaluationDetails stateEvaluationDetails)
        {
            if (stateEvaluationDetails.Declaration.IsLate(_hmrcDateService))
            {
                stateEvaluationDetails.AddState(DeclarationState.LateSubmission);
            }

            return true;
        }

        private bool IsPeriod12(StateEvaluationDetails stateEvaluationDetails)
        {
            if (stateEvaluationDetails.Declaration.PayrollMonth.HasValue &&
                stateEvaluationDetails.Declaration.PayrollMonth.Value == 12)
            {
                stateEvaluationDetails.AddState(DeclarationState.IsPeriod12);
            }

            return true;
        }

        private bool IsSuperseded(StateEvaluationDetails stateEvaluationDetails)
        {
            int r = stateEvaluationDetails.Index+1;
            var account = stateEvaluationDetails.Account;

            if (string.IsNullOrWhiteSpace(stateEvaluationDetails.Declaration.PayrollYear) ||
                stateEvaluationDetails.Declaration.PayrollMonth == null)
            {
                return true;
            }

            var periodDates = _hmrcDateService.GetDateRangeForPayrollPeriod(
                stateEvaluationDetails.Declaration.PayrollYear,
                stateEvaluationDetails.Declaration.PayrollMonth.Value);

            // Did we get another on-time submission for the same payroll period? (Declarations are in submission date order)
            while (r < account.LevyDeclarations.Length &&
                   (account.LevyDeclarations[r].SubmissionDate == null ||
                    account.LevyDeclarations[r].SubmissionDate < periodDates.EndDate))
            {
                var possibleLaterSubmission = account.LevyDeclarations[r];
                if (AreDeclarationsForTheSamePeriod(stateEvaluationDetails.Declaration, possibleLaterSubmission))
                {
                    stateEvaluationDetails.AddState(DeclarationState.WasSuperseded);
                }
                r++;
            }

            return true;
        }

        private bool AreDeclarationsForTheSamePeriod(LevyDeclaration declaration1, LevyDeclaration declaration2)
        {
            return declaration1.PayrollYear == declaration2.PayrollYear
                   && declaration1.PayrollMonth.HasValue
                   && declaration2.PayrollMonth.HasValue
                   && declaration1.PayrollMonth.Value == declaration2.PayrollMonth.Value;
        }
    }
}