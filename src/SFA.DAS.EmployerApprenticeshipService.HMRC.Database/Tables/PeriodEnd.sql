CREATE TABLE [levy].[PeriodEnd]
(
	[Id] INT IDENTITY(0,1) NOT NULL PRIMARY KEY,
	[PeriodEndId] nvarchar(20) NOT NULL,
	[CalendarPeriodMonth] int not null,
	[CalendarPeriodYear] int not null,
	[AccountDataValidAt] Datetime not null,
	[CommitmentDataValidAt] Datetime not null,
	[CompletionDateTime] Datetime not null,
	[PaymentsForPeriod] nvarchar(250) not null
)
