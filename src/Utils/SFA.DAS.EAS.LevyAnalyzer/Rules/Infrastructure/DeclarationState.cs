using System;

namespace SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure
{
    [Flags]
    public enum DeclarationState
    {
        Normal = 0,

        /// <summary>
        ///     The declaration is missing some critical data which means it can not be processed
        /// </summary>
        Invalid = 1, 

        /// <summary>
        ///     The declaration was submitted after the SFA cut-off date for the period
        /// </summary>
        LateSubmission = 2,

        /// <summary>
        ///     The submission was a period 12 submission
        /// </summary>
        IsPeriod12 = 4,

        /// <summary>
        ///     The declaration was on-time but was superseded by a later on-time submission in the same period
        /// </summary>
        WasSuperseded = 8,

        /// <summary>
        ///     Indicates that the declaration does not have a transaction. This is true even for late declarations.
        /// </summary>
        NoTransaction = 16,

        IsYearEndAdjustment = LateSubmission | IsPeriod12
    }
}