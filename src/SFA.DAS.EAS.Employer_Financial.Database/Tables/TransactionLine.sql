CREATE TABLE [employer_financial].[TransactionLine]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    AccountId BIGINT NOT NULL,
    DateCreated DATETIME NOT NULL,
    SubmissionId BIGINT NULL,
    TransactionDate DATETIME NOT NULL,
    TransactionType TINYINT NOT NULL DEFAULT 0, 
    LevyDeclared DECIMAL(18,4) NULL, 
    Amount DECIMAL(18,4) NOT NULL DEFAULT 0, 
    EmpRef nVarchar(50) null,
    PeriodEnd nVarchar(50) null,
    Ukprn BIGINT null, 
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

CREATE UNIQUE INDEX [IX_TransactionLine_TransactionType_SubmissionId] ON [employer_financial].[TransactionLine] (SubmissionId) WHERE (TransactionType = 1);
GO

CREATE INDEX [IX_TransactionLine_AccountId_DateCreated] ON [employer_financial].[TransactionLine] (AccountId, DateCreated) WITH (ONLINE = ON)
GO

CREATE INDEX [IX_TransactionLine_Account_TransactionType] ON [employer_financial].[TransactionLine] (AccountId, TransactionType) INCLUDE (DateCreated) WITH (ONLINE = ON)
GO