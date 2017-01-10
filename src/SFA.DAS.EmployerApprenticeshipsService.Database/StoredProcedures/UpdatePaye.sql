CREATE PROCEDURE [account].[UpdatePaye]
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50),
	@employerRefName VARCHAR(500) NULL
AS
BEGIN
	UPDATE 
		[account].[Paye] 
	SET 
		AccessToken = @accessToken, 
		RefreshToken = @refreshToken,
		Name = @employerRefName
	WHERE 
		Ref = @employerRef

END
