CREATE PROCEDURE [employer_financial].[GetPaymentData_ById]
	@paymentId uniqueIdentifier
	
AS
	SELECT * from [employer_financial].[Payment] where PaymentId = @paymentId

