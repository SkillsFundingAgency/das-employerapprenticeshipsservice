CREATE PROCEDURE [employer_financial].[GetEnglishFraction_ByEmpRef]
	@empref varchar(50)
as
select 
	* 
from [employer_financial].EnglishFraction
where EmpRef = @empref
