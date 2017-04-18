CREATE PROCEDURE [employer_financial].[ProcessDeclarationsTransactions]
AS


--Add the topup from the declaration
INSERT INTO [employer_financial].LevyDeclarationTopup
	select mainUpdate.* from
	(

	select 
		x.AccountId,
		DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		case y.PayrollMonth when 1 then  (y.LevyDueYTD* isnull(y.EnglishFraction,0))* isnull(y.TopUpPercentage,0) 
		else ((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * isnull(y.EnglishFraction,0)) * isnull(y.TopUpPercentage,0) 
		end as Amount
	FROM 
		[employer_financial].[GetLevyDeclarations] x
	left join 
		[employer_financial].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	union all
	select
		x.AccountId,
		DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		((x.EndOfYearAdjustmentAmount * isnull(x.EnglishFraction,0))* isnull(x.TopUpPercentage,0) * -1) as Amount
	FROM 
		[employer_financial].[GetLevyDeclarations] x
	where
		x.LevyDueYTD is not null and x.EndOfYearAdjustment = 1
	) mainUpdate
	inner join (
		select submissionId from [employer_financial].LevyDeclaration
	EXCEPT
		select SubmissionId from [employer_financial].LevyDeclarationTopup
	) dervx on dervx.submissionId = mainUpdate.SubmissionId


-- Create Declarations


INSERT INTO [employer_financial].TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId,
			DATEFROMPARTS(DatePart(yyyy,y.CreatedDate),DatePart(MM,y.CreatedDate),DATEPART(dd,y.CreatedDate)) as DateCreated,
			x.SubmissionId as SubmissionId,
			x.SubmissionDate as TransactionDate,
			1 as TransactionType,
			
			case y.PayrollMonth when 1 then  (y.LevyDueYTD)
			else (y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) end as LevyDeclared,
			case y.PayrollMonth when 1 then  (x.LevyDueYTD * ISNULL(y.EnglishFraction,0)) + ldt.amount
			else ((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * ISNULL(y.EnglishFraction,0)) + ldt.amount end as Amount,
			
			x.EmpRef as EmpRef,
			null as PeriodEnd,
			null as UkPrn
		FROM 
			[employer_financial].[GetLevyDeclarations] x
		inner join
			[employer_financial].[LevyDeclarationTopup] ldt on ldt.submissionId = x.submissionId
		left join 
			[employer_financial].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
		where
			y.LevyDueYTD is not null
	union all	
		select 
			x.AccountId,
			DATEFROMPARTS(DatePart(yyyy,x.CreatedDate),DatePart(MM,x.CreatedDate),DATEPART(dd,x.CreatedDate)) as DateCreated,
			x.SubmissionId as SubmissionId,
			x.SubmissionDate as TransactionDate,
			1 as TransactionType,
			x.LevyDueYTD as LevyDeclared,
			((x.endofyearadjustmentamount * ISNULL(x.EnglishFraction,0)) - ldt.amount) * -1 as Amount,
			x.EmpRef as EmpRef,
			null as PeriodEnd,
			null as UkPrn
		FROM 
			[employer_financial].[GetLevyDeclarations] x
		inner join
			[employer_financial].[LevyDeclarationTopup] ldt on ldt.submissionId = x.submissionId
		where x.EndOfYearAdjustment = 1
	) mainUpdate
	inner join (
		select submissionId from [employer_financial].LevyDeclaration
	EXCEPT
		select SubmissionId from [employer_financial].TransactionLine where TransactionType = 1
	) dervx on dervx.submissionId = mainUpdate.SubmissionId
GO