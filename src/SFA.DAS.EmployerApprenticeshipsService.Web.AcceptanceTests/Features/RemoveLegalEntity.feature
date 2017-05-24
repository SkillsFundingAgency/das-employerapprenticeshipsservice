Feature: RemoveLegalEntity
	In order to remove connected organisations
	As a account owner
	I want to be able to remove legal entities from my account

@mytag
Scenario Outline: Remove legal entity
	Given I am an account "<account_role>"
	Then I "<result>" remove a legal entity
	Examples:
	| account_role | result   |
	| Owner        | can     |
	| Transactor   | cannot	|
	| Viewer       | cannot	|