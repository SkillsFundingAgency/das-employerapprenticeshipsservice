CREATE PROCEDURE [employer_financial].[GetAccountTransferBalance]
	@accountId BIGINT NOT NULL	
AS
	DECLARE @currentDate AS DATETIME = GETDATE()
	DECLARE @currentYear AS INT = DATEPART(year, @currentDate)
	DECLARE @financialYearStartDate AS DATETIME
	DECLARE @financialYearEndDate AS DATETIME

	BEGIN
	IF DATEPART(month, @currentDate) > 4
		BEGIN
			SELECT @financialYearStartDate = CONVERT(datetime, CONCAT(@currentYear - 1, '-04-18 00:00:00')),
			 @financialYearEndDate = CONVERT(datetime, CONCAT(@currentYear, '-03-18 23:59:59'))	
		END
	ELSE
		BEGIN
			SELECT @financialYearStartDate = CONVERT(datetime, CONCAT(@currentYear - 2, '-04-18 00:00:00')),
			 @financialYearEndDate = CONVERT(datetime, CONCAT(@currentYear - 1, '-03-18 23:59:59'))	
		END
	END

	SELECT SUM(Amount) * 0.1 FROM [employer_financial].[TransactionLine] 
	WHERE TransactionDate >= @financialYearStartDate
	AND TransactionDate <= @financialYearEndDate
	AND TransactionType = 1
	AND AccountId = @accountId
