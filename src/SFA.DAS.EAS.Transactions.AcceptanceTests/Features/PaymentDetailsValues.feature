Feature: PaymentDetails
	In order to review my levy payments
	As an Employer
	I want to be able to see payment details

Scenario: Payment details refreshed
	#Given a payment is made for a commitment	
	Given I have an account
	And I have an apprenticeship	
	And I have a standard
	And I have a provider
	When I make a payment for the apprenticeship standard    
	And payment details are updated
	Then the updated payment details should be stored

Scenario: Payment course details stored
	Given I have an account
	And I have an apprenticeship	
	And I have a standard
	And I have a provider
	When I make a payment for the apprenticeship standard        
	And payment details are updated
	Then the apprenticeship course details are stored

	Scenario: Payment pathway details stored
	Given I have an account
	And I have an apprenticeship	
	And I have a framework
	And I have a provider
	When I make a payment for the apprenticeship framework      
	And payment details are updated
	Then the apprenticeship pathway details are stored


Scenario: Co-investment Payment course details stored
	Given I have an account
	And I have an apprenticeship	
	And I have a standard
	And I have a provider
	When I make a co-investment payment for the apprenticeship     
	And payment details are updated
	Then the apprenticeship course details are stored with coinvestment figures

Scenario: Payment learner details stored
	Given I have an account
	And I have an apprenticeship	
	And I have a standard
	And I have a provider
	When I make a payment for the apprenticeship standard       
	And payment details are updated
	Then the apprenticeship learner details are stored	
