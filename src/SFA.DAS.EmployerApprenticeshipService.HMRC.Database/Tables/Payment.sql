﻿CREATE TABLE [levy].[Payment]
(
	[Id] INT IDENTITY (1,1) NOT NULL PRIMARY KEY,
	[PaymentId] UNIQUEIDENTIFIER NOT NULL,
	[Ukprn] BIGINT NOT NULL,
	[Uln] BIGINT NOT NULL,
    [AccountId] BIGINT NOT NULL,
    [ApprenticeshipId] BIGINT NOT NULL,
    [DeliveryPeriodMonth] INT NOT NULL,
    [DeliveryPeriodYear] INT NOT NULL,
    [CollectionPeriodId] char(8) NOT NULL,
    [CollectionPeriodMonth] int NOT NULL,
    [CollectionPeriodYear] int NOT NULL,
	[EvidenceSubmittedOn] DATETIME NOT NULL,
    [EmployerAccountVersion] VARCHAR(50) NOT NULL,
    [ApprenticeshipVersion] VARCHAR(10) NOT NULL,
    [FundingSource] VARCHAR(25) NOT NULL,
    [TransactionType] VARCHAR(25) NOT NULL,
    [Amount] decimal(18,5) not null default 0,
	[PeriodEnd] VARCHAR(25) not null
)
