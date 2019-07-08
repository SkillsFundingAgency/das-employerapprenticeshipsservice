CREATE PROCEDURE [employer_account].[CreatePaye]
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50),
	@employerRefName VARCHAR(500) NULL,
	@aorn VARCHAR(50) NULL
AS
BEGIN
	INSERT INTO [employer_account].[Paye](Ref, AccessToken, RefreshToken, Name, Aorn) 
	VALUES (@employerRef, @accessToken, @refreshToken, @employerRefName, @aorn);
END
