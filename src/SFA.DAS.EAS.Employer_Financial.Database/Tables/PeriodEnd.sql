CREATE TABLE [employer_financial].[PeriodEnd]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PeriodEndId] nvarchar(20) NOT NULL,
	[CalendarPeriodMonth] int not null,
	[CalendarPeriodYear] int not null,
	[AccountDataValidAt] Datetime null,
	[CommitmentDataValidAt] Datetime null,
	[CompletionDateTime] Datetime not null,
	[PaymentsForPeriod] nvarchar(250) not null
)

GO

CREATE INDEX [IX_PeriodEnd_PeriodEndId] ON [employer_financial].[PeriodEnd] ([PeriodEndId])
