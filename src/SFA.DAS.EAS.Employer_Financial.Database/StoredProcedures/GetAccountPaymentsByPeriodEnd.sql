
CREATE PROCEDURE [employer_financial].[GetAccountPaymentsByPeriodEnd]
	@AccountId BIGINT,
	@PeriodEnd varchar(20)
AS
	SELECT * FROM [employer_financial].[Payment]
	WHERE AccountId = @AccountId
	AND PeriodEnd = @PeriodEnd

