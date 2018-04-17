CREATE PROCEDURE [employer_financial].[GetAccountPaymentIds]
	@AccountId BIGINT = 0	
AS
	SELECT PaymentId FROM [employer_financial].[Payment] where AccountId = @AccountId

