CREATE TABLE [employer_financial].TransactionLine
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
)
GO

CREATE INDEX [IX_TransactionLine_submissionId] ON [employer_financial].[TransactionLine] (SubmissionId)
