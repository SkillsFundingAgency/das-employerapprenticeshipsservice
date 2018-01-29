using System;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransactionsDownloadRequestAndResponse
{
    public class GetTransactionsDownloadRequestAndResponseTests
    {
        [Test]
        public void WithAnInvalidStartMonthThenThenViewModelIsInValid()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = -1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 1,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.IsFalse(viewModel.StartDate.Valid);
            Assert.IsTrue(viewModel.EndDate.Valid);
            Assert.IsFalse(viewModel.Valid);
        }

        [Test]
        public void WithAnInvalidStartYearThenThenViewModelIsInValid()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = -1,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 1,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.IsFalse(viewModel.StartDate.Valid);
            Assert.IsTrue(viewModel.EndDate.Valid);
            Assert.IsFalse(viewModel.Valid);
        }

        [Test]
        public void WithAnInvalidEndMonthThenThenViewModelIsInValid()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = -1,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.IsTrue(viewModel.StartDate.Valid);
            Assert.IsFalse(viewModel.EndDate.Valid);
            Assert.IsFalse(viewModel.Valid);
        }

        [Test]
        public void WithAnInvalidEndYearThenViewModelIsInValid()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 12,
                    Year = -1,
                },

                Transactions = null
            };

            Assert.IsTrue(viewModel.StartDate.Valid);
            Assert.IsFalse(viewModel.EndDate.Valid);
            Assert.IsFalse(viewModel.Valid);
        }

        [Test]
        public void WithValidDatesThenViewModelIsValid()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 12,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.IsTrue(viewModel.StartDate.Valid);
            Assert.IsFalse(viewModel.EndDate.Valid);
            Assert.IsFalse(viewModel.Valid);
        }

        [Test]
        public void WithValidStartDatesInFutureThenViewModelIsInvalid()
        {
            var futureDate = DateTime.Today.AddMonths(1);

            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = DateTime.Today.Month,
                    Year = DateTime.Today.Year,
                },

                Transactions = null
            };

            Assert.IsFalse(viewModel.Valid);
            Assert.IsTrue(viewModel.StartDate.DateInFuture);
        }


        [Test]
        public void MaximumDateStringIsCorrect()
        {
            var expectedDateString = DateTime.Today;
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse();

            Assert.AreEqual(expectedDateString, viewModel.EndDate.MaximumDate);
            Assert.AreEqual(expectedDateString, viewModel.StartDate.MaximumDate);
        }

        [Test]
        public void WithValidStartDatesInFutureYearThenViewModelIsInvalid()
        {
            var futureDate = DateTime.Today.AddYears(1);

            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = DateTime.Today.Month,
                    Year = DateTime.Today.Year,
                },

                Transactions = null
            };

            Assert.IsFalse(viewModel.Valid);
            Assert.IsTrue(viewModel.StartDate.DateInFuture);
        }

        [Test]
        public void WithValidEndDatesInFutureThenViewModelIsInvalid()
        {
            var futureDate = DateTime.Today.AddMonths(1);

            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = DateTime.Today.Month,
                    Year = DateTime.Today.Year,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year,
                },

                Transactions = null
            };

            Assert.IsFalse(viewModel.Valid);
            Assert.IsTrue(viewModel.EndDate.DateInFuture);
        }

        [Test]
        public void WithInValidStartMonthThenFromDateStartDateDateTimeThrowsException()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = -1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 12,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.Throws<InvalidOperationException>(() => viewModel.StartDate.ToDateTime());
        }

        [Test]
        public void WithInValidFromYearThenStartDateToDateTimeThrowsException()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = -1,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 12,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.Throws<InvalidOperationException>(() => viewModel.StartDate.ToDateTime());
        }

        [Test]
        public void WithInValidEndMonthThenEndDateToDateTimeThrowsException()
        {
            //var viewModel = new TransactionsDownloadResultViewModel
            //{
            //    Account = null,
            //    StartDate = new TransactionsDownloadResultViewModel.TransactionsDownloadDateTimeViewModel
            //    {
            //        Month = 1,
            //        Year = 2000,
            //    },
            //    EndDate = new TransactionsDownloadResultViewModel.TransactionsDownloadDateTimeViewModel
            //    {
            //        Month = -1,
            //        Year = 2018,
            //    },
            //    Transactions = null
            //};
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = -1,
                    Year = 2018,
                },

                Transactions = null
            };

            Assert.Throws<InvalidOperationException>(() => viewModel.EndDate.ToDateTime());
        }

        [Test]
        public void WithInValidEndYearThenEndDateToDateTimeThrowsException()
        {
            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 12,
                    Year = -1,
                },

                Transactions = null
            };

            Assert.Throws<InvalidOperationException>(() => viewModel.EndDate.ToDateTime());
        }

        [Test]
        public void WithValidStartDatesInFutureThenStartDateToDateTimeThrowsException()
        {
            var futureDate = DateTime.Today.AddMonths(1);

            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = DateTime.Today.Month,
                    Year = DateTime.Today.Year,
                },

                Transactions = null
            };

            Assert.Throws<InvalidOperationException>(() => viewModel.StartDate.ToDateTime());
        }

        [Test]
        public void WithValidEndDatesInFutureThenEndDateToDateTimeThrowsException()
        {
            var futureDate = DateTime.Today.AddMonths(1);

            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = DateTime.Today.Month,
                    Year = DateTime.Today.Year,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year,
                },

                Transactions = null
            };

            Assert.Throws<InvalidOperationException>(() => viewModel.EndDate.ToDateTime());
        }

        [Test]
        public void WithValidDatesThenToDateTimeReturnsWithNoException()
        {

            var viewModel = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2000,
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = 1,
                    Year = 2018,
                },

                Transactions = null
            };


            viewModel.StartDate.ToDateTime();
            viewModel.EndDate.ToDateTime();

            Assert.Pass("No error occurred: we pass");
        }
    }
}
