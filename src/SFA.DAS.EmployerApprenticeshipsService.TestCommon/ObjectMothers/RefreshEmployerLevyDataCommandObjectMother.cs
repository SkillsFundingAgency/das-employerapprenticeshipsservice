using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class RefreshEmployerLevyDataCommandObjectMother
    {
        public static RefreshEmployerLevyDataCommand Create(string empRef,long accountId = 1)
        {

            var refreshEmployerLevyDataCommand = new RefreshEmployerLevyDataCommand
            {
                AccountId = accountId,
                EmployerLevyData = new List<EmployerLevyData> {
                    new EmployerLevyData
                {
                EmpRef = empRef,
                Declarations = new DasDeclarations
                {
                    Declarations = new List<DasDeclaration>
                    {
                        new DasDeclaration
                        {
                            Id = "1",
                            LevyDueYtd = 10,
                            SubmissionDate = DateTime.UtcNow.AddMonths(-3)
                        },
                        new DasDeclaration
                        {
                            Id = "2",
                            LevyDueYtd = 70,
                            SubmissionDate = DateTime.UtcNow.AddMonths(-2)
                        },
                        new DasDeclaration
                        {
                            Id = "3",
                            NoPaymentForPeriod = true,
                            SubmissionDate = DateTime.UtcNow.AddMonths(-1)
                        },
                        new DasDeclaration
                        {
                            Id = "4",
                            LevyDueYtd = 80,
                            SubmissionDate = DateTime.UtcNow
                        }
                    }
                }
                }
               }

            };


            return refreshEmployerLevyDataCommand;
        }

        public static RefreshEmployerLevyDataCommand CreateLevyDataWithFutureSubmissions(string empRef, DateTime submissionStartDate, long accountId = 1)
        {
            var refreshEmployerLevyDataCommand = new RefreshEmployerLevyDataCommand
            {
                AccountId = accountId,
                EmployerLevyData = new List<EmployerLevyData> {
                    new EmployerLevyData
                {
                EmpRef = empRef,
                Declarations = new DasDeclarations
                {
                    Declarations = new List<DasDeclaration>
                    {
                        new DasDeclaration
                        {
                            Id = "1",
                            PayrollYear = GetPayrollYearFromDate(submissionStartDate.AddMonths(-3)),
                            PayrollMonth = GetPayrollMonthFromDate(submissionStartDate.AddMonths(-3)),
                            LevyDueYtd = 10,
                            SubmissionDate = submissionStartDate.AddMonths(-3)
                        },
                        new DasDeclaration
                        {
                            Id = "2",
                            LevyDueYtd = 70,
                            PayrollYear = GetPayrollYearFromDate(submissionStartDate.AddMonths(-2)),
                            PayrollMonth = GetPayrollMonthFromDate(submissionStartDate.AddMonths(-2)),
                            SubmissionDate = submissionStartDate.AddMonths(-2)
                        },
                        new DasDeclaration
                        {
                            Id = "3",
                            LevyDueYtd = 75,
                            PayrollYear = GetPayrollYearFromDate(submissionStartDate.AddMonths(-1)),
                            PayrollMonth = GetPayrollMonthFromDate(submissionStartDate.AddMonths(-1)),
                            SubmissionDate = submissionStartDate.AddMonths(-1)
                        },
                        new DasDeclaration
                        {
                            Id = "4",
                            LevyDueYtd = 80,
                            PayrollYear = GetPayrollYearFromDate(submissionStartDate),
                            PayrollMonth = GetPayrollMonthFromDate(submissionStartDate),
                            SubmissionDate = submissionStartDate
                        },
                        new DasDeclaration
                        {
                            Id = "5",
                            LevyDueYtd = 90,
                            PayrollYear = GetPayrollYearFromDate(submissionStartDate.AddMonths(1)),
                            PayrollMonth = GetPayrollMonthFromDate(submissionStartDate.AddMonths(1)),
                            SubmissionDate = submissionStartDate.AddMonths(1)
                        },

                    }
                }
                }
               }

            };


            return refreshEmployerLevyDataCommand;
        }

        public static RefreshEmployerLevyDataCommand CreateEndOfYearAdjustment(string empRef, long accountId = 1)
        {
            var refreshEmployerLevyDataCommand = new RefreshEmployerLevyDataCommand
            {
                AccountId = accountId,
                EmployerLevyData = new List<EmployerLevyData> {
                    new EmployerLevyData
                {
                EmpRef = empRef,
                Declarations = new DasDeclarations
                {
                    Declarations = new List<DasDeclaration>
                    {
                        new DasDeclaration
                        {
                            Id = "1",
                            LevyDueYtd = 10,
                            PayrollYear = "16-17",
                            PayrollMonth = 12,
                            SubmissionDate = new DateTime(2017,05,01)
                        }
                        
                    }
                }
                }
               }

            };


            return refreshEmployerLevyDataCommand;
        }

        private static short GetPayrollMonthFromDate(DateTime date)
        {
            var month = (short)date.Month;
            short monthToUse;
            if (month >= 4)
            {
                monthToUse = (short) (month - 3);
            }
            else
            {
                monthToUse = (short)(month + 9);
            }


            return monthToUse;
        }

        private static string GetPayrollYearFromDate(DateTime date)
        {
            return date.Month >= 4 ? $"{date.ToString("yy")}-{date.AddYears(1).ToString("yy")}" : $"{date.AddYears(-1).ToString("yy")}-{date.ToString("yy")}";
        }
    }
}
