CREATE PROCEDURE [levy].[GetTransactionDetail_ByDateRange]
	@accountId bigint,
	@fromDate datetime,
	@toDate datetime

AS
select 
	* 
from levy.TransactionLine tl
where	tl.TransactionDate >= @fromDate AND 
		tl.TransactionDate <= @toDate AND 
		tl.AccountId = @accountId 
