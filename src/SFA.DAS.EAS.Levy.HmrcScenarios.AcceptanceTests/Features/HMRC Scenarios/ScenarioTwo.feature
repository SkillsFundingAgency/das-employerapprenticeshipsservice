Feature: Scenario Two - Seasonal variations, single PAYE

Scenario: Month three submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 18750      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
	Then the balance on 07/2017 should be 20625 on the screen
	And the total levy shown for month 07/2017 should be -1375
	And For month 07/2017 the levy declared should be -1250 and the topup should be -125

Scenario: Month twelve submission (Checking 2nd negative declaration)
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 18750      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 123/ABC     | 28750      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |
		| 123/ABC     | 38750      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |
		| 123/ABC     | 48750      | 17-18        | 6             | 1                | 2017-10-15     | 2017-10-23  |
		| 123/ABC     | 58750      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 123/ABC     | 68750      | 17-18        | 8             | 1                | 2017-12-15     | 2017-12-23  |
		| 123/ABC     | 67500      | 17-18        | 9             | 1                | 2018-01-15     | 2018-01-23  |
		| 123/ABC     | 77500      | 17-18        | 10            | 1                | 2018-02-15     | 2018-02-23  |
		| 123/ABC     | 87500      | 17-18        | 11            | 1                | 2018-03-15     | 2018-03-23  |
		| 123/ABC     | 97500      | 17-18        | 12            | 1                | 2018-04-15     | 2018-04-23  |	
	Then the balance on 04/2018 should be 107250 on the screen		
	And the total levy shown for month 01/2018 should be -1375
	And For month 01/2018 the levy declared should be -1250 and the topup should be -125