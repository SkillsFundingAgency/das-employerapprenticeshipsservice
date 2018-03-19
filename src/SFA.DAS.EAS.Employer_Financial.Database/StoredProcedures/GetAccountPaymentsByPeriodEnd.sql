
CREATE PROCEDURE [employer_financial].[GetAccountPaymentsByPeriodEnd]
	@accountId BIGINT,
	@PeriodEnd varchar(20)
AS
	SELECT * FROM [employer_financial].[Payment]
	WHERE AccountId = @accountId
	AND PeriodEnd = @PeriodEnd

