# Know your customer

The solution is a training and experimental area for TDD. 

## *Scenario 1:* Extending the calculation of loyalty points

Following feature is requested:

**As a marketing manager I want to make loyalty of purchasing customers visible, so that proper classifications for bonus promotions can be made.**

Actually customers who are recently purchasing a lot should get better bonus promotions.

For the first iteration the simple formula $P = P + 2M$ calculating the value of the promotion points should do the trick. $P$ is the variable of the loyalty points and $M$ is the overall amount the customer spent within the last 30 days.

Development, business and quality assuarance (the three amigos) are discussing the feature and as am outcome following scenario is created:

**Scenario:**\
Layoality Profile points are evaluating to 5 + X * 2 when registration happended longer than 365 days and purchases in the hight of X were made within the last 30 days.\

&nbsp;&nbsp;&nbsp;&nbsp;**Given** the customer signed up \<DaysPassedSinceSignUp\> days ago\
&nbsp;&nbsp;&nbsp;&nbsp;**And** the customer spent more than \<MoneySpent\> \<DaysAgo\> days ago\
&nbsp;&nbsp;&nbsp;&nbsp;**When** the loyalty profile is evaluated\
&nbsp;&nbsp;&nbsp;&nbsp;**Then** the value for the loyalty points is \<ExpectedLoyaltyPoints\>\

Examples:\
| DaysPassedSinceSignUp | MoneySpent | FromDaysAgo | DaysAgo | ExpectedLoyaltyPoints |\
| 500&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;| 5000&emsp;&emsp;&emsp;&nbsp;&nbsp;| 30&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp;| 0&emsp;&emsp;&emsp;&nbsp;&nbsp;         | 105 &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp;&nbsp;|

### *Step 1:* Add failing specflow test

1. After introducing the feature we add the spec to the feature file [LoyaltyProfiler.feature](KycAppCoreSpecs/Features/LoayalityProfiler.feature).
2. We can reuse already implemented bindings for most of the step definitions as those are already in place testing the previous feature of rewarding sign up.
3. We have to implement the step biding for the step **the customer spent more than \<MoneySpent\> \<DaysAgo\> days ago**. Therefore we create the proper method in [LoyaltyProfilerStepDefintion](KycAppCoreSpecs/Steps/LoayalityProfilerStepDefintion.cs).
```csharp
[Given("the customer spent more than (.*) between (.*) and (.*) days ago")]
public void TheCustomerSpentMoreThanMoneySpentBetweenFromDaysAgoAndToDaysAgo(
    int moneySpent, int fromDaysAgo, int toDaysAgo)
{
    var activityStoreTestAdapter = scenarioContext.Get<ActivityStoreTestAdapter>(ActivityStoreTestAdapterKey);
    activityStoreTestAdapter.Register(
        new PurchaseEvent(1, DateTime.Now.AddDays(toDaysAgo - 1), moneySpent));
}
```
> **_Note:_** The record (I prefer record here but feel free going with a class) PurchaseEvent didn't exist in the solution and was created during coding the test. Use means of your IDE to create the new type quickly and without leaving the specflow project KycAppCoreSpecs.

Let the tests run and show that the recently added one is red while the others are green.

4.) Implement the logic of the feature. For finding the code for the business logic (imagine you are a newbee in this repo and you see it for the first time) look at the defintion for the **then**-step. This shows that the method **GenerateProfile** in the class [LoyaltyProfile](KycCoreApp/LoyaltyProfile.cs) is invoked. Below you can see one solution for adding the feature.
```csharp
public async Task GenerateProfile(int customerId)
    {
        DateTime? signUpDate = (await activityStore.GetEventsFor(customerId)).OfType<SignUpActivityEvent>()
            .FirstOrDefault(s => s.CustomerId == customerId)?.ActivityTimeStamp;
        if (signUpDate == null)
        {
            this.Error = ErrorCodes.UnknownCustomer;
        }
        else
        {
            this.Points += signUpDate <= DateTime.Now.AddYears(-1) ? PointsForSignupLongtimeAgo : 0;
        }

        // this block contains the new added code
        this.Points +=
            (await activityStore.GetEventsFor(customerId)).OfType<PurchaseEvent>()
            .Where(p => p.ActivityTimeStamp.Date > DateTime.Now.AddDays(-30)).
            Sum(p => p.Amount * 2 / 100);
    }
```
> **_Note:_** Do the minimal things that are necessary for getting the tests green.

Let the tests run again and show that all are green.

5.) Refactor the code in the method **GenerateProfile** by calling **GetEventsFor** on **activityStore** only once (-> **_performance_**). Additionally substitute the expressions
```csharp
DateTime.Now.AddDays(-30) 
```
and
```csharp
DateTime.Now.AddDays(-1)
```
by properly named methods (-> **_clean code_**).
```csharp
 public async Task GenerateProfile(int customerId)
    {
        var customerActivityEventBases = 
            (await activityStore.GetEventsFor(customerId)).ToList();
        
        DateTime? signUpDate = customerActivityEventBases.OfType<SignUpActivityEvent>()
            .FirstOrDefault(s => s.CustomerId == customerId)?.ActivityTimeStamp;
        if (signUpDate == null)
        {
            this.Error = ErrorCodes.UnknownCustomer;
        }
        else
        {
            this.Points += signUpDate <= OneYearAgo() ? PointsForSignupLongtimeAgo : 0;
        }

        this.Points +=
            customerActivityEventBases.OfType<PurchaseEvent>()
            .Where(p => p.ActivityTimeStamp.Date > ThirtyDaysAgo()).
            Sum(p => p.Amount * 2 / 100);
    }
```
> **_Note:_** Let the tests run after each refactoring task in order to check whether something is broken or not.

## Takeaways ##
Let's summarize a few takeaways here.
* **Stay in the test code as much/long you can !**\
The test code is the first client code to your businesslogic ever and therefore it is a suitable place for design decisions regarding your APIs. During writing the tests you figure out what you really need and the risk of falling into the traps of speculative generality or YAGNI (You aint gonna need it) is lower. This approach may also have a huge impact on readability as well.\ Use
means of your IDE for easily creating methods, types and properties on demand (e.g.: generate method), create type ...), so that you are not distracted from the test code.
* **Switch between styles**\
There are different approaches of unit testing out in the field like London (mocking, white box, inside-out) or Detroit (Classic, black box, outside-in). Take a closer look at the philosophies behind those but see them as frameworks or tools. In the real world, by knowing the advantages and disadvantages, you can make use of all of them. It depends on the situation which style is currently more suitable and don't hesitate using both as well.


