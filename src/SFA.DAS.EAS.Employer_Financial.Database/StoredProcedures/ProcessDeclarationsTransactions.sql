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
		x.TopUp as Amount
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
	where
		x.LevyDueYTD is not null AND x.LastSubmission = 1
	union all
	select
		x.AccountId,
		DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		((x.EndOfYearAdjustmentAmount * isnull(x.EnglishFraction,0))* isnull(x.TopUpPercentage,0) * -1) as Amount
	FROM 
		[employer_financial].[GetLevyDeclarationAndTopUp] x
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
			DATEFROMPARTS(DatePart(yyyy,x.CreatedDate),DatePart(MM,x.CreatedDate),DATEPART(dd,x.CreatedDate)) as DateCreated,
			x.SubmissionId as SubmissionId,
			x.SubmissionDate as TransactionDate,
			1 as TransactionType,
			
			x.LevyDeclaredInMonth as LevyDeclared,
			x.TotalAmount as Amount,
			
			x.EmpRef as EmpRef,
			null as PeriodEnd,
			null as UkPrn,
			0 as SfaCoInvestmentAmount,
			0 as EmployerCoInvestmentAmount
		FROM 
			[employer_financial].[GetLevyDeclarationAndTopUp] x
		where
			x.LevyDueYTD is not null AND x.LastSubmission = 1
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
			null as UkPrn,
			0 as SfaCoInvestmentAmount,
			0 as EmployerCoInvestmentAmount
		FROM 
			[employer_financial].[GetLevyDeclarationAndTopUp] x
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