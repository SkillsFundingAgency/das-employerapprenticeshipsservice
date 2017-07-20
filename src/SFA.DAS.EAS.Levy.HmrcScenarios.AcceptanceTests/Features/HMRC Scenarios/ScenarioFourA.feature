Feature: ScenarioFourA - In year adjustment results in balance change

Scenario: Future submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-05-16     | 2017-06-23  |
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |				
		| 123/ABC     | 40000      | 17-18        | 4             | 1                | 2017-08-17     | 2017-08-23  |		
	Then the balance on 08/2017 should be 44000 on the screen
	And the total levy shown for month 05/2017 should be 11000
	And the total levy shown for month 06/2017 should be 11000
	And For month 05/2017 the levy declared should be 10000 and the topup should be 1000
	And For month 06/2017 the levy declared should be 10000 and the topup should be 1000


	Scenario: Late submissions
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | 20000      | 17-18        | 2             | 1                | 2017-06-16     | 2017-06-23  |
		| 123/ABC     | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |			
		| 123/ABC     | 40000      | 17-18        | 4             | 1                | 2017-08-17     | 2017-08-23  |		
		| 123/ABC     | 21250      | 17-18        | 2             | 1                | 2017-09-16     | 2017-09-23  |
		| 123/ABC     | 31250      | 17-18        | 3             | 1                | 2017-09-15     | 2017-09-23  |			
		| 123/ABC     | 41250      | 17-18        | 4             | 1                | 2017-09-17     | 2017-09-23  |		
		| 123/ABC     | 51250      | 17-18        | 5             | 1                | 2017-09-17     | 2017-09-23  |		
	Then the balance on 08/2017 should be 56375 on the screen
	And the total levy shown for month 08/2017 should be 11000
	And the total levy shown for month 09/2017 should be 12375
	And For month 08/2017 the levy declared should be 10000 and the topup should be 1000
	And For month 09/2017 the levy declared should be 11250 and the topup should be 1125