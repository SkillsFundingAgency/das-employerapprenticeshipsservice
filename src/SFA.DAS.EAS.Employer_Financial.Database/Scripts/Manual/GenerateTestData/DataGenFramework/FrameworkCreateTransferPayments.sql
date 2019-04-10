
DECLARE @senderAccountId BIGINT               = 0
DECLARE @senderAccountName NVARCHAR(100)      = 'Sender Name'
DECLARE @receiverAccountId BIGINT             = 1
DECLARE @receiverAccountName NVARCHAR(100)    = 'Receiver Name'
	
DECLARE @toDate DATETIME                      = GETDATE()
declare @numberOfMonthsToCreate INT           = 25
declare @defaultMonthlyTransfer DECIMAL(18,4) = 100
declare @defaultNumberOfPaymentsPerMonth INT  = 1

declare @paymentsByMonth DataGen.PaymentGenerationSourceTable

insert @paymentsByMonth
select * from DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTransfer, @defaultNumberOfPaymentsPerMonth)

select * from @paymentsByMonth

exec DataGen.CreateTransfersForMonths @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @paymentsByMonth
