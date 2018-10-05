Feature: HMRC-Scenario-02-Seasonal-variations-single-PAYE

Scenario: Month-02-submission
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
		| 999000201   | 10000      | 17-18        | 1             | 1                | 2017-05-15     |
		| 999000202   | 20000      | 17-18        | 2             | 1                | 2017-06-15     |
		| 999000203   | 18750      | 17-18        | 3             | 1                | 2017-07-15     |		
	When we refresh levy data for paye scheme 223/ABC
	And all the transaction lines in this scenario have had there transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 20625 on the 07/2017
	And we should see a level 1 screen with a total levy of -1375 on the 07/2017
	And we should see a level 2 screen with a levy declared of -1250 on the 07/2017
	And we should see a level 2 screen with a top up of -125 on the 07/2017

Scenario: Month-02-submission-Checking-2nd-negative-declaration
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
		| 999000204   | 10000      | 17-18        | 1             | 1                | 2017-05-15     |
		| 999000205   | 20000      | 17-18        | 2             | 1                | 2017-06-15     |
		| 999000206   | 18750      | 17-18        | 3             | 1                | 2017-07-15     |
		| 999000207   | 28750      | 17-18        | 4             | 1                | 2017-08-15     |
		| 999000208   | 38750      | 17-18        | 5             | 1                | 2017-09-15     |
		| 999000209   | 48750      | 17-18        | 6             | 1                | 2017-10-15     |
		| 999000210   | 58750      | 17-18        | 7             | 1                | 2017-11-15     |
		| 999000211   | 68750      | 17-18        | 8             | 1                | 2017-12-15     |
		| 999000212   | 67500      | 17-18        | 9             | 1                | 2018-01-15     |
		| 999000213   | 77500      | 17-18        | 10            | 1                | 2018-02-15     |
		| 999000214   | 87500      | 17-18        | 11            | 1                | 2018-03-15     |
		| 999000215   | 97500      | 17-18        | 12            | 1                | 2018-04-15     |
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had there transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 107250 on the 04/2018
	And we should see a level 1 screen with a total levy of -1375 on the 01/2018
	And we should see a level 2 screen with a levy declared of -1250 on the 01/2018
	And we should see a level 2 screen with a top up of -125 on the 01/2018
