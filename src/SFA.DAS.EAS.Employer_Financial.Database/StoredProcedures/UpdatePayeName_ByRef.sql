CREATE PROCEDURE [employer_financial].[UpdatePayeName_ByRef]
	@Ref varchar(16) = 0,
	@RefName varchar(500)
AS
	
	Update [employer_financial].AccountPaye 
	set Name = @RefName 
	where EmpRef = @Ref
