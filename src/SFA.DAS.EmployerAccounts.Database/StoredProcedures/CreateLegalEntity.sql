CREATE PROCEDURE [employer_account].[CreateLegalEntity]
	@code NVARCHAR(50), 
	@dateOfIncorporation DATETIME,
	@status varchar(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@sector NVARCHAR(100) NULL,
	@legalEntityId BIGINT OUTPUT
AS
BEGIN
	INSERT INTO [employer_account].[LegalEntity](Code, DateOfIncorporation, [Status], [Source], [PublicSectorDataSource], Sector)
	VALUES (@code, @dateOfIncorporation, @status, @source, @publicSectorDataSource,@sector);	
	SELECT @legalEntityId = SCOPE_IDENTITY();
END