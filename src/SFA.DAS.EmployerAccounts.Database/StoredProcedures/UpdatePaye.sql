CREATE PROCEDURE [employer_account].[UpdatePaye]
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50),
	@employerRefName VARCHAR(500) NULL,
	@aorn VARCHAR(50) NULL
AS
BEGIN
	UPDATE 
		[employer_account].[Paye] 
	SET 
		AccessToken = @accessToken, 
		RefreshToken = @refreshToken,
		Name = @employerRefName,
		Aorn = @aorn
	WHERE 
		Ref = @employerRef

END
