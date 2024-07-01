Feature: LoyaltyProfiler
Creates loayality profiles for customers

Link to a feature: [Calculator]($projectname$/Features/LoyaltyProfiler.feature)

@mytag
Scenario: Layoality Profile evaluates to zero when registration happended within the last 365 days
	Given the customer signed up 1 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 0

@mytag
Scenario: Layoality Profile evaluates to zero when registration happended exactly 365 days ago
	Given the customer signed up 365 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 0

@mytag
Scenario: Layoality Profile evaluates to five when registration happended exactly 366 days ago
	Given the customer signed up 366 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 5

@mytag
Scenario: Layoality Profile evaluates to error when no signup event exists
	Given the customer did never sign up
	When the loyalty profile is evaluated
	Then the loyalty profile emits error 1

@mytag
Scenario: Layoality Profile points are evaluating to 5 + X * 2 when registration happended longer than 365 days 
          and purchases in the hight of X were made within the last 30 days
	Given the customer signed up <DaysPassedSinceSignUp> days ago
    And the customer spent more than <MoneySpent> between <FromDaysAgo> and <ToDaysAgo> days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is <ExpectedLoyaltyPoints>
Examples:
| DaysPassedSinceSignUp | MoneySpent | FromDaysAgo | ToDaysAgo | ExpectedLoyaltyPoints |
| 500                   | 5000       | 30          | 0         | 105                   |
| 400                   | 2000       | 40          | 35        | 5                     |  
| 400                   | 1000       | 20          | 0         | 25                    |     
| 400                   | 2000       | 30          | 29        | 45                    |  