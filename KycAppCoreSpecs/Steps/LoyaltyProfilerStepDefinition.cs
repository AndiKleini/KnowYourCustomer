using FluentAssertions;
using KycAppCore;
using KycAppCore.Events;
using KycAppCoreSpecs.TestAdapters;

namespace KycAppCoreSpecs.Steps;

[Binding]
public sealed class LoyaltyProfilerStepDefinition(ScenarioContext scenarioContext)
{
    private const string EvaluationResultKey = "EvaluationResultKey";
    private const string ActivityStoreTestAdapterKey = "ActivityStoreTestAdapterKey";

    [Given("the customer signed up (.*) days ago")]
    public void TheCustomerSignedUpDaysAgo(int daysSinceSignUp)
    {
        var activityStoreTestAdapter = new ActivityStoreTestAdapter();
        activityStoreTestAdapter.Register(new SignUpActivityEvent(1, DateTime.Now.AddDays(-daysSinceSignUp)));
        scenarioContext.Add(ActivityStoreTestAdapterKey, activityStoreTestAdapter);
    }

    [When(@"the loyalty profile is evaluated")]
    public void WhenTheLoyaltyProfileIsEvaluated()
    {
        var loyaltyProfile = new LoyaltyProfile(
            scenarioContext.Get<ActivityStoreTestAdapter>(ActivityStoreTestAdapterKey));
        loyaltyProfile.GenerateProfile(1);
        scenarioContext.Add(EvaluationResultKey, loyaltyProfile.Points);
    }
    
    [Then("the value for the loyalty points is (.*)")]
    public void WhenTheLoyaltyProfileIsEvaluated(int expectedLoyaltyPoint)
    {
        scenarioContext.TryGetValue<int>(EvaluationResultKey, out var result);
        result.Should().Be(expectedLoyaltyPoint);
    }

    [AfterScenario("Unregister activity events")]
    public void UnregisterActivityEvents()
    {
        scenarioContext.Remove(ActivityStoreTestAdapterKey);
    }
}