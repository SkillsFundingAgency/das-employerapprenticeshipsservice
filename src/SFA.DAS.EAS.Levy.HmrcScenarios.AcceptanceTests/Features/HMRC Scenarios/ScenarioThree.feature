Feature: ScenarioThree - Multiple EPS submissions for month within submission window

Scenario: Month three submission (Multiple submission month)
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 35000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
		| 123/ABC     | 25000      | 17-18        | 3             | 1                | 2017-07-16     | 2017-07-23  |		
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-17     | 2017-07-23  |		
	Then the balance on 07/2017 should be 33000 on the screen
	And the total levy shown for month 07/2017 should be 11000
	And For month 07/2017 the levy declared should be 10000 and the topup should be 1000

Scenario: Month four submission (Month after multiple submissions)
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 35000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
		| 123/ABC     | 25000      | 17-18        | 3             | 1                | 2017-07-16     | 2017-07-23  |		
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-17     | 2017-07-23  |		
		| 123/ABC     | 40000      | 17-18        | 4             | 1                | 2017-08-17     | 2017-08-23  |		
	Then the balance on 08/2017 should be 44000 on the screen
	And the total levy shown for month 08/2017 should be 11000
	And For month 08/2017 the levy declared should be 10000 and the topup should be 1000