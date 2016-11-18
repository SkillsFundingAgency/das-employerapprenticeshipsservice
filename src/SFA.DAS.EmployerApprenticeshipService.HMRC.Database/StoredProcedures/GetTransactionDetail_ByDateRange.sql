CREATE PROCEDURE [levy].[GetTransactionDetail_ByDateRange]
	@accountId bigint,
	@fromDate datetime,
	@toDate datetime

AS
select 
	tl.AccountId,
	tl.LevyDeclared as Amount,
	ef.Amount as EnglishFraction,
	ldt.Amount as TopUp,
	tl.EmpRef,
	tl.TransactionDate,
	tl.Amount as LineAmount,
	tl.TransactionType
from levy.TransactionLine tl
inner join levy.LevyDeclarationTopup ldt on ldt.SubmissionId = tl.SubmissionId
left join levy.EnglishFraction ef on ef.EmpRef = tl.EmpRef and ef.DateCalculated <= tl.TransactionDate
where	tl.TransactionDate >= @fromDate AND 
		tl.TransactionDate <= @toDate AND 
		tl.AccountId = @accountId 
