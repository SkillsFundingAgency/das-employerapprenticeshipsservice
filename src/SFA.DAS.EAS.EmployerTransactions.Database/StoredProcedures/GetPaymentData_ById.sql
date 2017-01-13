CREATE PROCEDURE [employer_transactions].[GetPaymentData_ById]
	@paymentId uniqueIdentifier
	
AS
	SELECT * from [employer_transactions].[Payment] where PaymentId = @paymentId

