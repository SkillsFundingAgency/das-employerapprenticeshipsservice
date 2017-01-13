CREATE PROCEDURE [employer_transactions].[GetEnglishFraction_ByEmpRef]
	@empref varchar(50)
as
select 
	* 
from [employer_transactions].EnglishFraction
where EmpRef = @empref
