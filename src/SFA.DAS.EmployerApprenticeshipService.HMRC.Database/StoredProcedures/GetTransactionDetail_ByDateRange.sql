CREATE PROCEDURE [levy].[GetTransactionDetail_ByDateRange]
	@accountId bigint,
	@fromDate datetime,
	@toDate datetime

AS
select 
	tl.AccountId,
	tl.Amount as Amount,
	ld.LevyDueYTD as LevyDueYTD,
	ldt.Amount as TopUp,
	tl.EmpRef,
	tl.TransactionDate
from levy.TransactionLine tl
inner join levy.LevyDeclaration ld on ld.SubmissionId = tl.SubmissionId
inner join levy.LevyDeclarationTopup ldt on ldt.SubmissionId = tl.SubmissionId
where	tl.TransactionDate >= @fromDate AND 
		tl.TransactionDate <= @toDate AND 
		tl.AccountId = @accountId 
