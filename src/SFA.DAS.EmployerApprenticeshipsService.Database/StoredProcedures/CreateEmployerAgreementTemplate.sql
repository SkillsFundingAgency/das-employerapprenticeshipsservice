CREATE PROCEDURE [account].[CreateEmployerAgreementTemplate]
	@ref NVARCHAR(50),
	@text NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [account].[EmployerAgreementTemplate]([Text], CreatedDate, Ref) 
	VALUES (@text, GETDATE(), @ref);
END