CREATE TABLE [employer_financial].[Payment]
(	
	[PaymentId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Ukprn] BIGINT NOT NULL,	
	[Uln] BIGINT NOT NULL,
    [AccountId] BIGINT NOT NULL,
    [ApprenticeshipId] BIGINT NOT NULL,
    [DeliveryPeriodMonth] INT NOT NULL,
    [DeliveryPeriodYear] INT NOT NULL,
    [CollectionPeriodId] char(20) NOT NULL,
    [CollectionPeriodMonth] int NOT NULL,
    [CollectionPeriodYear] int NOT NULL,
	[EvidenceSubmittedOn] DATETIME NOT NULL,
    [EmployerAccountVersion] VARCHAR(50) NOT NULL,
    [ApprenticeshipVersion] VARCHAR(25) NOT NULL,
    [FundingSource] VARCHAR(25) NOT NULL,
    [TransactionType] VARCHAR(25) NOT NULL,
    [Amount] decimal(18,5) not null default 0,
	[PeriodEnd] VARCHAR(25) not null, 
    [PaymentMetaDataId] BIGINT NOT NULL,     
    [DateImported] DATETIME NULL
	)

GO

CREATE INDEX [IX_Payment_AccountId] ON [employer_financial].[Payment] (AccountId) INCLUDE (Ukprn,PeriodEnd,FundingSource)
GO

CREATE INDEX [IX_Payment_PaymentMetaDataId] ON [employer_financial].[Payment] ([PaymentMetaDataId])
GO

CREATE INDEX [IX_Payment_FundingComp] ON [employer_financial].[Payment] (AccountId, Ukprn, FundingSource, PaymentMetaDataId)
GO

CREATE NONCLUSTERED INDEX [IX_Payment_ULN] ON employer_financial.Payment (Uln) WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_Payment_AccountId_Amount_ApprenticeshipId]
ON employer_financial.Payment (PeriodEnd) 
INCLUDE (
	AccountId,
	Amount,
	ApprenticeshipId,
	ApprenticeshipVersion,
	CollectionPeriodId,
	CollectionPeriodMonth, 
	CollectionPeriodYear,
	DeliveryPeriodMonth,
	DeliveryPeriodYear,
	EmployerAccountVersion,
	EvidenceSubmittedOn, 
	FundingSource,
	PaymentMetaDataId,
	TransactionType,
	Ukprn,
	Uln
)
WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_Payment_AccountIdUkprnPeriodEndUln] ON [employer_financial].[Payment] ([AccountId], [Ukprn], [PeriodEnd], [Uln]) INCLUDE ([FundingSource], [PaymentMetaDataId]) WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_Payment_AccountIdCollectionPeriodMonthCollectionPeriodYear] ON [employer_financial].[Payment] ([AccountId], [CollectionPeriodMonth], [CollectionPeriodYear]) INCLUDE ([Amount], [ApprenticeshipId], [CollectionPeriodId], [DeliveryPeriodMonth], [DeliveryPeriodYear], [FundingSource], [PaymentMetaDataId], [PeriodEnd], [Ukprn], [Uln]) WITH (ONLINE = ON)
GO