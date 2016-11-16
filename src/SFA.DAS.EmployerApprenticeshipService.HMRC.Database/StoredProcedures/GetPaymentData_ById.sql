CREATE PROCEDURE [levy].[GetPaymentData_ById]
	@paymentId uniqueIdentifier
	
AS
	SELECT * from [levy].[Payment] where PaymentId = @paymentId

