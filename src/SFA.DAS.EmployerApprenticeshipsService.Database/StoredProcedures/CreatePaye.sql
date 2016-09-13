CREATE PROCEDURE [account].[CreatePaye]
	@legalEntityId BIGINT,
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50)
AS
BEGIN
	INSERT INTO [account].[Paye](Ref, LegalEntityId, AccessToken, RefreshToken) 
	VALUES (@employerRef, @legalEntityId, @accessToken, @refreshToken);
END
