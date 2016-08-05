Feature: InviteTeamMember
	In order to increase the size of my team
	As an account owner
	I want to be able to add more people

@mytag
Scenario Outline: Add team member
	Given I am an account owner
	When I invite a team member with email address "<email>" and name "<name>"
	Then A user invite is "<result>"
Examples:
	| email             | name		| result	  |
	| test@test.com     | tester	| created	  |
	| test'ing@test.com | tester	| created	  |
	| notanemail		| something | not_created |
	|					| something | not_created |
	| test@test.com		|           | not_created |
