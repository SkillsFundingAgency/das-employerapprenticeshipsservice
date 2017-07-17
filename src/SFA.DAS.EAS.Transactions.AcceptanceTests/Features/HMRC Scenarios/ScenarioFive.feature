Feature: Scenario Five - Adjustment to prior tax year

Scenario: End of year adjustment
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 11250      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 22500      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 33750      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 123/ABC     | 45000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |
		| 123/ABC     | 56250      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |
		| 123/ABC     | 67500      | 17-18        | 6             | 1                | 2017-10-15     | 2017-10-23  |
		| 123/ABC     | 78750      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 123/ABC     | 90000      | 17-18        | 8             | 1                | 2017-12-15     | 2017-12-23  |
		| 123/ABC     | 101250     | 17-18        | 9             | 1                | 2018-01-15     | 2018-01-23  |
		| 123/ABC     | 112500     | 17-18        | 10            | 1                | 2018-02-15     | 2018-02-23  |
		| 123/ABC     | 123750     | 17-18        | 11            | 1                | 2018-03-15     | 2018-03-23  |
		| 123/ABC     | 135000     | 17-18        | 12            | 1                | 2018-04-15     | 2018-04-23  |
		| 123/ABC     | 10000      | 18-19        | 1             | 1                | 2018-05-15     | 2018-05-23  |
		| 123/ABC     | 120000     | 17-18        | 12            | 1                | 2018-06-10     | 2018-06-23  |
		| 123/ABC     | 20000      | 18-19        | 2             | 1                | 2018-06-15     | 2018-06-23  |		
	Then the balance on 06/2018 should be 154500 on the screen		
	And the total levy shown for month 06/2018 should be -5500
	And For month 06/2018 the levy declared should be -5000 and the topup should be -500