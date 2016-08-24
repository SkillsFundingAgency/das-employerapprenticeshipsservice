Feature: LevyAggregation
	In order to show how my transaction history
	I want to be view the aggregation of my PAYE schemes

@mytag
Scenario Outline: Add two numbers
	Given I have added "<paye_scenario>" to my account
	When I build the aggregation
	Then the result is equal to "<paye_scenario_result>"
Examples: 
	| paye_scenario | paye_scenario_result |
	| scenario_1    | scenario_1_result    |
