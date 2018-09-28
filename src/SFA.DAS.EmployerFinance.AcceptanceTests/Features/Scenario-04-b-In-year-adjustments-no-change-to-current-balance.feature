Feature: HMRC-Scenario-04b-In-year-adjustment-no-change-to-current-balance

Scenario: Late-submissions
	Given We have an account
	And Hmrc return the following submissions for paye scheme 123/ABC
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000451   | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 999000452   | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 999000453   | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 999000454   | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |
		| 999000455   | 50000      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |
		| 999000456   | 21250      | 17-18        | 2             | 1                | 2017-09-15     | 2017-09-23  |
		| 999000457   | 60000      | 17-18        | 6             | 1                | 2017-10-15     | 2017-10-23  |
		| 999000458   | 70000      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 999000459   | 80000      | 17-18        | 8             | 1                | 2017-12-15     | 2017-12-23  |
		| 999000460   | 90000      | 17-18        | 9             | 1                | 2018-01-15     | 2018-01-23  |
		| 999000461   | 100000     | 17-18        | 10            | 1                | 2018-02-15     | 2018-02-23  |
		| 999000462   | 110000     | 17-18        | 11            | 1                | 2018-03-15     | 2018-03-23  |
		| 999000463   | 120000     | 17-18        | 12            | 1                | 2018-04-15     | 2018-04-23  |
		| 999000464   | 98000      | 17-18        | 10            | 1                | 2018-04-15     | 2018-04-23  |
	When we refresh levy data for paye scheme 123/ABC
	And all the transaction lines in this scenario have had there transaction date updated to the specified created date
	Then we should see a level 1 screen with a balance of 132000 on the 04/2017
	And we should see a level 1 screen with a total levy of 11000 on the 06/2017
	And we should see a level 1 screen with a total levy of 11000 on the 02/2018
	And we should see a level 2 screen with a levy declared of 10000 on the 02/2018
	And we should see a level 2 screen with a top up of 1000 on the 02/2018
	And we should see a level 2 screen with a levy declared of 10000 on the 06/2017
	And we should see a level 2 screen with a top up of 1000 on the 06/2017
