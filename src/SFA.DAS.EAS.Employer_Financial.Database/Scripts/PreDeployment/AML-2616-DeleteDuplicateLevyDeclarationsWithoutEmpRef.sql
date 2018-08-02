-- Clean up the duplicates
DELETE ld FROM [employer_financial].[LevyDeclaration] ld
INNER JOIN (

		SELECT id
		FROM [employer_financial].[LevyDeclaration] ldd
		INNER JOIN (
			SELECT Min(Id) as MinId, [SubmissionId] 
			FROM employer_financial.LevyDeclaration 
			GROUP BY [SubmissionId] HAVING COUNT(*) > 1
		) d 
		ON d.[SubmissionId] = ldd.[SubmissionId]
		AND d.MinId <> ldd.Id

) dupes
	ON dupes.ID = ld.Id