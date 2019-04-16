
DECLARE @senderAccountId bigint               = 0
DECLARE @senderAccountName nvarchar(100)      = 'Sender Name'
DECLARE @receiverAccountId bigint             = 1
DECLARE @receiverAccountName nvarchar(100)    = 'Receiver Name'
	
DECLARE @toDate datetime                      = GETDATE()
DECLARE @numberOfMonthsToCreate int           = 25
DECLARE @defaultMonthlyTransfer decimal(18,4) = 100
DECLARE @defaultNumberOfPaymentsPerMonth int  = 1

DECLARE @paymentsByMonth DataGen.PaymentGenerationSourceTable

INSERT @paymentsByMonth
SELECT * FROM DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTransfer, @defaultNumberOfPaymentsPerMonth)

SELECT * FROM @paymentsByMonth

EXEC DataGen.CreateTransfersForMonths @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @paymentsByMonth
