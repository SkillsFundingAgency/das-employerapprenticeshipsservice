using System;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    /// <summary>
    ///     Holds key data saved with the rule results when an account has failed some rules.
    ///     This allows some diagnosis from just the saved results file.
    /// </summary>
    public class DeclarationSummary
    {
        private readonly LevyDeclaration _declaration;
        private readonly TransactionLine _transaction;

        public DeclarationSummary(LevyDeclaration declaration, TransactionLine transaction, DeclarationState state)
        {
            _declaration = declaration;
            _transaction = transaction;
            State = state;
        }

        public long AccountId => _declaration.AccountId;
        public string EmpRef => _declaration.EmpRef;
        public long SubmissionId => _declaration.SubmissionId;
        public DateTime? SubmissionDate => _declaration.SubmissionDate;
        public decimal? YearToDateAmount => _declaration.LevyDueYTD;
        public decimal? LevyDeclared => _transaction?.LevyDeclared;
        public string PayrollYear => _declaration.PayrollYear;
        public byte? PayrollPeriod => _declaration.PayrollMonth;
        public decimal? EnglishFraction => _transaction?.EnglishFraction;
        public DeclarationState State { get; }
    }
}