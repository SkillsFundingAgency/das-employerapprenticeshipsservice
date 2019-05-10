CREATE TYPE [employer_financial].[ExpiredFundsTable] AS TABLE
(
	[CalendarPeriodYear] INT NOT NULL,
	[CalendarPeriodMonth] INT NOT NULL,
	[Amount] DECIMAL(18,4) NOT NULL
)
