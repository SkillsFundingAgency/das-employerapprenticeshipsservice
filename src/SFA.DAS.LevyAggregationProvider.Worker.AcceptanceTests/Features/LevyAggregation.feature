Feature: LevyAggregation
	In order to show how my transaction history
	I want to be view the aggregation of my PAYE schemes

@mytag
Scenario Outline: Add two numbers
	Given I have added "<paye_schemes>" to my account
	When I build the aggregation
	Then the result is equal to "<paye_scenario_result>"
Examples: 
	| paye_schemes | paye_scenario_result | period | account_open_from |
	| 123    | scenario_1_result    |        |                   |
