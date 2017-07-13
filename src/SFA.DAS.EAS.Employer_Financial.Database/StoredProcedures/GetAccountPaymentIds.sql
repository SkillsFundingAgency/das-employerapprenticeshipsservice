CREATE PROCEDURE [employer_financial].[GetAccountPaymentIds]
	@accountId BIGINT = 0	
AS
	SELECT PaymentId FROM [employer_financial].[Payment] where AccountId = @accountId

