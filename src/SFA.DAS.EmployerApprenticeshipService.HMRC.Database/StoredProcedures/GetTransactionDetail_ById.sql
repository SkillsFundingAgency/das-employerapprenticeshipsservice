CREATE PROCEDURE [levy].[GetTransactionDetail_ById]
	@submissionId bigint
AS
select 
	* 
from levy.TransactionLine tl
left join levy.GetLevyDeclarations gdl on gdl.SubmissionId = tl.SubmissionId 
where tl.SubmissionId = @submissionId
