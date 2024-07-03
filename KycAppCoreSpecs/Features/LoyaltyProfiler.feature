Feature: LoyaltyProfiler
Creates loayality profiles for customers

Link to a feature: [Calculator]($projectname$/Features/LoyaltyProfiler.feature)

Scenario: Layoality Profile evaluates to zero when registration happended within the last 365 days
	Given the customer signed up 1 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 0

Scenario: Layoality Profile evaluates to zero when registration happended exactly 365 days ago
	Given the customer signed up 365 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 0

Scenario: Layoality Profile evaluates to five when registration happened exactly 366 days ago
	Given the customer signed up 366 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 5

Scenario: Layoality Profile evaluates to error when no signup event exists
	Given the customer did never sign up
	When the loyalty profile is evaluated
	Then the loyalty profile emits error 1

# this is a new scenario that was refined in a three amigos session
Scenario: Layoality Profile points are evaluating to 5 + X * 2 when registration happended longer than 365 days 
          and purchases with the amount of X were made within the last 30 days
	Given the customer signed up <DaysPassedSinceSignUp> days ago
    And the customer spent more than <MoneySpent> cash <DaysAgo> days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is <ExpectedLoyaltyPoints>
Examples:
| DaysPassedSinceSignUp | MoneySpent | DaysAgo | ExpectedLoyaltyPoints |
| 500                   | 5000       | 0         | 105                   |
| 400                   | 2000       | 35        | 5                     |  
| 400                   | 1000       | 0         | 25                    |     
| 400                   | 2000       | 29        | 45                    |  