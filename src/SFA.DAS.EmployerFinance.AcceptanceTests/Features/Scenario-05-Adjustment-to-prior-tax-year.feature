Feature: HMRC-Scenario-05-Adjustment-to-prior-tax-year

Scenario: Sceanrio-01-End-of-year-adjustment
	Given We have an account with id 25
	And Hmrc return the following submissions for paye scheme 123/ABC
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000501   | 11250      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 999000502   | 22500      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 999000503   | 33750      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 999000504   | 45000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |
		| 999000505   | 56250      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |
		| 999000506   | 67500      | 17-18        | 6             | 1                | 2017-10-15     | 2017-10-23  |
		| 999000507   | 78750      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 999000508   | 90000      | 17-18        | 8             | 1                | 2017-12-15     | 2017-12-23  |
		| 999000509   | 101250     | 17-18        | 9             | 1                | 2018-01-15     | 2018-01-23  |
		| 999000510   | 112500     | 17-18        | 10            | 1                | 2018-02-15     | 2018-02-23  |
		| 999000511   | 123750     | 17-18        | 11            | 1                | 2018-03-15     | 2018-03-23  |
		| 999000512   | 135000     | 17-18        | 12            | 1                | 2018-04-15     | 2018-04-23  |
		| 999000513   | 10000      | 18-19        | 1             | 1                | 2018-05-15     | 2018-05-23  |
	When we refresh levy data for account id 25 paye scheme 123/ABC
	And All the transaction lines in this scenario have had there transaction date updated to the specified created date
	Given Hmrc return the following submissions for paye scheme 123/ABC
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000514   | 120000     | 17-18        | 12            | 1                | 2018-06-10     | 2018-06-23  | 
		| 999000515   | 20000      | 18-19        | 2             | 1                | 2018-06-15     | 2018-06-23  | 
	When we refresh levy data for account id 25 paye scheme 123/ABC
	And All the transaction lines in this scenario have had there transaction date updated to the specified created date
	Then account with id 25 should see a level 1 screen with a balance of 154000 on the 06/2018
	And account with id 25 should see a level 1 screen with a total levy of -5500 on the 06/2018
	And account with id 25 should see a level 2 screen with a levy declared of -5000 on the 06/2018
	And account with id 25 should see a level 2 screen with a top up of -500 on the 06/2018

