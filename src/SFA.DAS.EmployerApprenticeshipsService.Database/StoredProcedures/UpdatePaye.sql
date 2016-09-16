CREATE PROCEDURE [account].[UpdatePaye]
	@legalEntityId BIGINT,
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50)
AS
BEGIN
	UPDATE 
		[account].[Paye] 
	SET 
		LegalEntityId = @legalEntityId, 
		AccessToken = @accessToken, 
		RefreshToken = @refreshToken
	WHERE 
		Ref = @employerRef

END
