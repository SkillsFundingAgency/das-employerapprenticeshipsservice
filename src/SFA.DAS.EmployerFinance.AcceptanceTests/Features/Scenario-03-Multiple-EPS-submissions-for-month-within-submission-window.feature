Feature: HMRC-Scenario-03-Multiple-EPS-submissions-for-month-within-submission-window

Scenario: Month-03-submission-Multiple-submission-month
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
		| 999000301   | 10000      | 17-18        | 1             | 1                | 2017-05-15     |
		| 999000302   | 20000      | 17-18        | 2             | 1                | 2017-06-15     |
		| 999000303   | 35000      | 17-18        | 3             | 1                | 2017-07-15     |		
		| 999000304   | 25000      | 17-18        | 3             | 1                | 2017-07-16     |		
		| 999000305   | 30000      | 17-18        | 3             | 1                | 2017-07-17     |		
	When we refresh levy data for paye scheme
	And All the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 33000 on the 07/2017
	And we should see a level 1 screen with a total levy of 11000 on the 07/2017
	And we should see a level 2 screen with a levy declared of 10000 on the 07/2017
	And we should see a level 2 screen with a top up of 1000 on the 07/2017

Scenario: Month-04-submission-Month-after-multiple-submissions
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
		| 999000306   | 10000      | 17-18        | 1             | 1                | 2017-05-15     |
		| 999000307   | 20000      | 17-18        | 2             | 1                | 2017-06-15     |
		| 999000308   | 35000      | 17-18        | 3             | 1                | 2017-07-15     |		
		| 999000309   | 25000      | 17-18        | 3             | 1                | 2017-07-16     |		
		| 999000310   | 30000      | 17-18        | 3             | 1                | 2017-07-17     |		
		| 999000311   | 40000      | 17-18        | 4             | 1                | 2017-08-17     |		
	When we refresh levy data for paye scheme
	And All the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 44000 on the 08/2017
	And we should see a level 1 screen with a total levy of 11000 on the 08/2017
	And we should see a level 2 screen with a levy declared of 10000 on the 08/2017
	And we should see a level 2 screen with a top up of 1000 on the 08/2017
