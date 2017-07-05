CREATE PROCEDURE [employer_financial].[GetEnglishFraction_ByEmpRef]
	@accountId bigint,
	@empref varchar(50)
as
select * from
(
	select 
		ef.Id,
		ef.DateCalculated,
		COALESCE(efo.Amount, ef.Amount) AS Amount,
		ef.EmpRef
	from [employer_financial].EnglishFraction ef
	outer apply
	(
		select top 1 Amount
		from [employer_financial].[EnglishFractionOverride] o
		where o.AccountId = @accountId and o.EmpRef = @empref and o.DateFrom <= DateCalculated
		order by o.DateFrom desc
	) efo
	where EmpRef = @empref
	union
	select 
		o.Id,
		o.DateFrom AS DateCalculated,
		o.Amount,
		o.EmpRef
	from [employer_financial].[EnglishFractionOverride] o
	where o.AccountId = @accountId and o.EmpRef = @empref and o.DateFrom <= GETDATE()
) x
order by x.DateCalculated desc
