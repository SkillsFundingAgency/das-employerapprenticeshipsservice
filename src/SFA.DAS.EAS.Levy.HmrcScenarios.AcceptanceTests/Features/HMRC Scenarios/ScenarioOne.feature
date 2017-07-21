Feature: HMRC Scenario one - Single PAYE no adjustments

Scenario: Month one submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |		
	Then the balance on 05/2017 should be 11000 on the screen
	And the total levy shown for month 05/2017 should be 11000
	And For month 05/2017 the levy declared should be 10000 and the topup should be 1000

Scenario: Month two submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |		
	Then the balance on 06/2017 should be 22000 on the screen		
	And the total levy shown for month 06/2017 should be 11000
	And For month 06/2017 the levy declared should be 10000 and the topup should be 1000
	
Scenario: Month three submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
	Then the balance on 07/2017 should be 33000 on the screen
	And the total levy shown for month 07/2017 should be 11000
	And For month 07/2017 the levy declared should be 10000 and the topup should be 1000

Scenario: Month six submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 123/ABC     | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |
		| 123/ABC     | 50000      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |
		| 123/ABC     | 60000      | 17-18        | 6             | 1                | 2017-10-15     | 2017-10-23  |		
	Then the balance on 10/2017 should be 66000 on the screen	
	And the total levy shown for month 10/2017 should be 11000
	And For month 10/2017 the levy declared should be 10000 and the topup should be 1000

Scenario: Month twelve submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 123/ABC     | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |
		| 123/ABC     | 50000      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |
		| 123/ABC     | 60000      | 17-18        | 6             | 1                | 2017-10-15     | 2017-10-23  |
		| 123/ABC     | 70000      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 123/ABC     | 80000      | 17-18        | 8             | 1                | 2017-12-15     | 2017-12-23  |
		| 123/ABC     | 90000      | 17-18        | 9             | 1                | 2018-01-15     | 2018-01-23  |
		| 123/ABC     | 100000     | 17-18        | 10            | 1                | 2018-02-15     | 2018-02-23  |
		| 123/ABC     | 110000     | 17-18        | 11            | 1                | 2018-03-15     | 2018-03-23  |
		| 123/ABC     | 120000     | 17-18        | 12            | 1                | 2018-04-15     | 2018-04-23  |		
	Then the balance on 04/2018 should be 132000 on the screen		
	And the total levy shown for month 04/2018 should be 11000
	And For month 04/2018 the levy declared should be 10000 and the topup should be 1000