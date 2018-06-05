CREATE TABLE [employer_financial].[TransactionLine]
(
	AccountId BIGINT NOT NULL,
	DateCreated DATETIME NOT NULL,
	SubmissionId BIGINT NULL,
	TransactionDate DATETIME NOT NULL,
	TransactionType TINYINT NOT NULL DEFAULT 0, 
	LevyDeclared DECIMAL(18,4) NULL, 
	Amount DECIMAL(18,4) NOT NULL DEFAULT 0, 
	EmpRef nVarchar(50) null,
	PeriodEnd nVarchar(50) null,
	UkPrn BIGINT null, 
	SfaCoInvestmentAmount DECIMAL(18, 4) NOT NULL DEFAULT 0, 
	EmployerCoInvestmentAmount DECIMAL(18, 4) NOT NULL DEFAULT 0,
	[EnglishFraction] DECIMAL(18, 5) NULL, 
	[TransferSenderAccountId] BIGINT NULL, 
	[TransferSenderAccountName] NVARCHAR(100) NULL,
	[TransferReceiverAccountId] BIGINT NULL, 
	[TransferReceiverAccountName] NVARCHAR(100) NULL
)
GO

CREATE INDEX [IX_TransactionLine_SubmissionId] ON [employer_financial].[TransactionLine] (SubmissionId)
GO
CREATE INDEX [IX_TransactionLine_AccountId] ON [employer_financial].[TransactionLine] (AccountId) INCLUDE (Ukprn,PeriodEnd,TransactionType)
GO
CREATE INDEX [IX_TransactionLine_Payment] on [employer_financial].[TransactionLine] (PeriodEnd,AccountId,Ukprn,TransactionDate, DateCreated)
GO
