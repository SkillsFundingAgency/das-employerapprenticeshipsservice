CREATE PROCEDURE [employer_financial].[GetAccountTransferAllowance]
	@accountId BIGINT = 0,
	@allowanceRatio FLOAT = 0
AS
	SET NOCOUNT ON

	DECLARE @currentDate AS DATETIME = GETDATE()
	DECLARE @currentYear AS INT = DATEPART(year, @currentDate)
	DECLARE @financialYearStartDate AS DATETIME
	DECLARE @financialYearEndDate AS DATETIME

	BEGIN
		IF DATEPART(month, @currentDate) > 4
			BEGIN
				SELECT @financialYearStartDate = DATEFROMPARTS(@currentYear - 1, 4, 20),
				 @financialYearEndDate = DATEFROMPARTS(@currentYear, 4, 20)
			END
		ELSE
			BEGIN
				SELECT @financialYearStartDate = DATEFROMPARTS(@currentYear - 2, 4, 20),
				 @financialYearEndDate = DATEFROMPARTS(@currentYear - 1, 4, 20)
			END
	END

	SELECT SUM(Amount) * @allowanceRatio FROM [employer_financial].[TransactionLine] 
	WHERE TransactionDate >= @financialYearStartDate
	AND TransactionDate < @financialYearEndDate
	AND TransactionType = 1
	AND AccountId = @accountId

	GO
