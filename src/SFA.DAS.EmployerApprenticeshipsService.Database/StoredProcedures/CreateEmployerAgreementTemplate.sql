CREATE PROCEDURE [dbo].[CreateEmployerAgreementTemplate]
	@text NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @templateId BIGINT;
	DECLARE @lastTemplateId BIGINT;

	--get the current template id
	SELECT TOP 1 @lastTemplateId = Id 
	FROM [dbo].[EmployerAgreementTemplate]
	ORDER BY Id DESC;  

	--create new template and get template id
	INSERT INTO [dbo].[EmployerAgreementTemplate]([Text], CreatedDate) VALUES (@text, GETDATE());
	SELECT @templateId = SCOPE_IDENTITY();

	--mark agreements using previous template as expired
	UPDATE [dbo].[EmployerAgreement]
	SET ExpiredDate = GETDATE(),
		StatusId = 3
	WHERE TemplateId = @lastTemplateId;

	--insert new record for each legalentity with new template
	INSERT INTO [dbo].[EmployerAgreement](LegalEntityId, TemplateId, StatusId)
	SELECT LegalEntityId, @templateId, 1 
	FROM [dbo].[EmployerAgreement]
	WHERE TemplateId = @lastTemplateId;

	--insert into AccountEmployerAgreement with new Ids
	INSERT INTO [dbo].[AccountEmployerAgreement]
	SELECT aea.AccountId, t.Id
	FROM [dbo].[AccountEmployerAgreement] aea
		JOIN [dbo].[EmployerAgreement] ea
			ON ea.Id = aea.EmployerAgreementId
		JOIN 
		(
			SELECT *
			FROM [dbo].[EmployerAgreement]
			WHERE TemplateId = @templateId
		) t
		ON t.LegalEntityId = ea.LegalEntityId
	WHERE ea.TemplateId = @lastTemplateId;
END