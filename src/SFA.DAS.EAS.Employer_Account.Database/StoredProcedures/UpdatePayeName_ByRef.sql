CREATE PROCEDURE [employer_account].[UpdatePayeName_ByRef]
	@Ref varchar(16) = 0,
	@RefName varchar(500)
AS
	
	Update employer_account.Paye 
	set Name = @RefName 
	where Ref=@Ref
