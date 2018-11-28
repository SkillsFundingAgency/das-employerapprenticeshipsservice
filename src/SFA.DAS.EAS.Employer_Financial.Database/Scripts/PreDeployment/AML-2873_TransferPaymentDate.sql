IF COL_LENGTH('employer_financial.TransactionLine', 'Id') IS NULL
BEGIN
BEGIN TRY
BEGIN TRANSACTION
    CREATE TABLE [employer_financial].[TransactionLine2]
	(
		Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
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
	

INSERT INTO [employer_financial].[TransactionLine2] SELECT * FROM [employer_financial].[TransactionLine]


EXEC sp_rename 'employer_financial.TransactionLine', 'TransactionLine3'
EXEC sp_rename 'employer_financial.TransactionLine2', 'TransactionLine'


CREATE INDEX [IX_TransactionLine_SubmissionId] ON [employer_financial].[TransactionLine] (SubmissionId)
CREATE INDEX [IX_TransactionLine_AccountId] ON [employer_financial].[TransactionLine] (AccountId) INCLUDE (Ukprn,PeriodEnd,TransactionType)
CREATE INDEX [IX_TransactionLine_Payment] on [employer_financial].[TransactionLine] (PeriodEnd,AccountId,Ukprn,TransactionDate, DateCreated)
CREATE UNIQUE INDEX [IX_TransactionLine_TransactionType_SubmissionId] ON [employer_financial].[TransactionLine] (SubmissionId) WHERE (TransactionType = 1);
CREATE INDEX [IX_TransactionLine_AccountId_DateCreated] ON [employer_financial].[TransactionLine] (AccountId, DateCreated) WITH (ONLINE = OFF)
CREATE INDEX [IX_TransactionLine_Account_TransactionType] ON [employer_financial].[TransactionLine] (AccountId, TransactionType) INCLUDE (DateCreated) WITH (ONLINE = OFF)


DROP TABLE employer_financial.TransactionLine3

IF @@TRANCOUNT > 0
 COMMIT TRANSACTION

END TRY
BEGIN CATCH
DECLARE @ErrorMsg nvarchar(max);
DECLARE @ErrorSeverity INT;
DECLARE @ErrorState INT;
SET @ErrorMsg = '[Code: ' + CAST(ERROR_NUMBER() AS VARCHAR) + ', Line: ' + CAST(ERROR_LINE() AS VARCHAR) + ' ] ' + ERROR_MESSAGE()
SET @ErrorSeverity = ERROR_SEVERITY()
SET @ErrorState = ERROR_STATE()

IF @@TRANCOUNT > 0
    ROLLBACK TRANSACTION

RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState)
END CATCH
END