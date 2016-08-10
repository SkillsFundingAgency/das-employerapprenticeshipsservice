CREATE PROCEDURE [dbo].[CreateEmployerAgreementTemplate]
	@text NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @templateId BIGINT;
	DECLARE @lastTemplateId BIGINT;

	SELECT TOP 1 @lastTemplateId = Id 
	FROM [dbo].[EmployerAgreementTemplate]
	ORDER BY Id DESC;  

	INSERT INTO [dbo].[EmployerAgreementTemplate]([Text]) VALUES (@text);

	UPDATE [dbo].[EmployerAgreement]
	SET ExpiredDate = GETDATE()
	WHERE TemplateId = @lastTemplateId;

	INSERT INTO [dbo].[EmployerAgreement](LegalEntityId, TemplateId, StatusId)
	SELECT LegalEntityId, @templateId, 1 
	FROM [dbo].[EmployerAgreement]
	WHERE TemplateId = @lastTemplateId;

	--insert into AccountEmployerAgreement with new Ids
END