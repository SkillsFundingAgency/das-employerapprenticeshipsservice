CREATE PROCEDURE [employer_financial].[GetAccountTransferBalance]
	@accountId BIGINT NOT NULL	
AS
	SET NOCOUNT ON

	DECLARE @currentDate AS DATETIME = GETDATE()
	DECLARE @currentYear AS INT = DATEPART(year, @currentDate)
	DECLARE @financialYearStartDate AS DATETIME
	DECLARE @financialYearEndDate AS DATETIME

	BEGIN
		IF DATEPART(month, @currentDate) > 4
			BEGIN
				SELECT @financialYearStartDate = DATEFROMPARTS(@currentYear - 1, 4, 18),
				 @financialYearEndDate = DATEFROMPARTS(@currentYear, 3, 19)
			END
		ELSE
			BEGIN
				SELECT @financialYearStartDate = DATEFROMPARTS(@currentYear - 2, 4, 18),
				 @financialYearEndDate = DATEFROMPARTS(@currentYear - 1, 3, 19)
			END
	END

	SELECT SUM(Amount) * 0.1 FROM [employer_financial].[TransactionLine] 
	WHERE TransactionDate >= @financialYearStartDate
	AND TransactionDate <= @financialYearEndDate
	AND TransactionType = 1
	AND AccountId = @accountId

	GO
