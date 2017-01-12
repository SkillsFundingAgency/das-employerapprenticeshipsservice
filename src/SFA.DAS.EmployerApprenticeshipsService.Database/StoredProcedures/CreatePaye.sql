CREATE PROCEDURE [account].[CreatePaye]
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50),
	@employerRefName VARCHAR(500) NULL
AS
BEGIN
	INSERT INTO [account].[Paye](Ref, AccessToken, RefreshToken, Name) 
	VALUES (@employerRef, @accessToken, @refreshToken, @employerRefName);
END
