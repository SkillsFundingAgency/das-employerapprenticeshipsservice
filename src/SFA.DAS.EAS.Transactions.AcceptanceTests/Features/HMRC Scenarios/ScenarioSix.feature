Feature: Scenario Six - Period of inactivity

Scenario: Inactivity period
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 1234/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 1234/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 1234/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 1234/ABC     | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |		
		| 1234/ABC     | 47500      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 1234/ABC     | 57500      | 18-19        | 8             | 1                | 2017-12-15     | 2017-12-23  |		
	Then the balance on 12/2017 should be 63250 on the screen		
	And the total levy shown for month 11/2017 should be 8250
	And For month 12/2017 the levy declared should be 7500 and the topup should be 750