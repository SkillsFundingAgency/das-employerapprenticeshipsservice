Feature: Tranfer Processing

Scenario: After processing payents the transfer collection is started
Given the transfer receiver has an account
And I get payments for that account
When I finish processing those payments
Then the payments processing completed message is created

Scenario: Getting transfers
Given the transfer receiver has an account
And the transfer sender has an account
And I get payments for that account
And transfers have been associated with those payments
And I have completed getting my payments for that account
When I finish processing the transfers for that account
Then the account transfers should be save


Scenario: Processing transfer sender transactions
Given the transfer receiver has an account
And the transfer sender has an account
And I get payments for that account
And transfers have been associated with those payments
And I have completed getting my payments for that account
And I have finished getting transfers for that account
When I process tranfers transactions for that account
Then the transfer senders transactions should be saved

Scenario: Processing transfer receivers transactions
Given the transfer receiver has an account
And the transfer sender has an account
And I get payments for that account
And transfers have been associated with those payments
And I have completed getting my payments for that account
And I have finished getting transfers for that account
When I process tranfers transactions for that account
Then the transfer receiver transactions should be saved