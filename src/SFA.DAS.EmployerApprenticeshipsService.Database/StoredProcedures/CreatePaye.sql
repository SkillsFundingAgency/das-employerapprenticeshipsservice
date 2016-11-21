CREATE PROCEDURE [account].[CreatePaye]
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50)
AS
BEGIN
	INSERT INTO [account].[Paye](Ref, AccessToken, RefreshToken) 
	VALUES (@employerRef, @accessToken, @refreshToken);
END
