BEGIN TRY
BEGIN TRANSACTION

DECLARE @Frameworks as table(
        FrameworkCode int,
        ProgrammeType int,
        PathwayCode int,
        PathwayName varchar(max),
        ApprenticeshipCourseName varchar(max));

INSERT INTO @Frameworks (FrameworkCode, ProgrammeType, PathwayCode, PathwayName, ApprenticeshipCourseName)
SELECT FrameworkCode, ProgrammeType, PathwayCode, PathwayName, ApprenticeshipCourseName
FROM (
    SELECT    FrameworkCode, ProgrammeType, PathwayCode, PathwayName, ApprenticeshipCourseName,
            ROW_NUMBER() OVER (
                PARTITION BY FrameworkCode,ProgrammeType, PathwayCode
                ORDER BY FrameworkCode,ProgrammeType, PathwayCode)  AS RowNumber
    FROM    [employer_financial].PaymentMetaData
    WHERE    PathwayName IS NOT NULL
            AND    ApprenticeshipCourseName IS NOT NULL) AS T1
WHERE T1.RowNumber = 1
ORDER BY 1, 2, 3;

		UPDATE Bad
		SET Bad.ApprenticeshipCourseName = FW.ApprenticeshipCourseName,
		Bad.PathwayName = FW.PathwayName
		FROM [employer_financial].[PaymentMetaData] AS Bad
		JOIN @Frameworks AS FW
		ON FW.FrameworkCode = Bad.FrameworkCode
		AND FW.ProgrammeType = Bad.ProgrammeType
		AND FW.PathwayCode = Bad.PathwayCode
		WHERE Bad.PathwayName IS NULL
		AND Bad.ApprenticeshipCourseName IS NULL;
 
 IF @@TRANCOUNT > 0
 COMMIT TRANSACTION

END TRY
BEGIN CATCH
DECLARE @ErrorMessage nvarchar(max);
DECLARE @ErrorSeverityLevel INT;
DECLARE @ErrorStatus INT;
SET @ErrorMessage = ERROR_NUMBER() + ERROR_LINE() + ERROR_MESSAGE();
SET @ErrorSeverityLevel = ERROR_SEVERITY();
SET @ErrorStatus = ERROR_STATE();

RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
END CATCH
