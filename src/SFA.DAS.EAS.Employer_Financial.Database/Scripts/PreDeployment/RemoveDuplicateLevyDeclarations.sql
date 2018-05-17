-- Store the original data 
IF (NOT EXISTS (SELECT *  
					FROM INFORMATION_SCHEMA.TABLES  
					WHERE TABLE_SCHEMA = 'employer_financial'  
					AND  TABLE_NAME = 'LevyDeclarationNonUnique')) 
BEGIN 
	SELECT 	[Id]
			,[AccountId]
			,[empRef]
			,[LevyDueYTD]
			,[LevyAllowanceForYear]
			,[SubmissionDate]
			,[SubmissionId]
			,[PayrollYear]
			,[PayrollMonth]
			,[CreatedDate]
			,[EndOfYearAdjustment]
			,[EndOfYearAdjustmentAmount]
			,[DateCeased]
			,[InactiveFrom]
			,[InactiveTo]
			,[HmrcSubmissionId]
			,[NoPaymentForPeriod]
	INTO [employer_financial].[LevyDeclarationNonUnique] 
			FROM [employer_financial].[LevyDeclaration]

	-- Clean up the duplicates
	DELETE ld FROM [employer_financial].[LevyDeclaration] ld
	INNER JOIN (

		SELECT ldd.EmpRef, ldd.[SubmissionId], Max(ldd.Id) AS maxId 
		FROM [employer_financial].[LevyDeclaration] ldd
		INNER JOIN (
			SELECT [empRef], [SubmissionId] 
			FROM employer_financial.LevyDeclaration 
			GROUP BY [empRef], [SubmissionId] HAVING COUNT(*) > 1
		) d 
		ON d.empRef = ldd.[empRef] AND d.[SubmissionId] = ldd.[SubmissionId]
		GROUP BY ldd.[empRef], ldd.[SubmissionId]

	) dupes
		ON dupes.[empRef] = ld.[empRef]
		AND dupes.[SubmissionId] = ld.[SubmissionId]
		AND dupes.maxId != ld.Id
END 

