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
    private const string LoyaltyProfileUnderTestKey = "LoyaltyProfileUnderTestKey";

    [Given("the customer signed up (.*) days ago")]
    public void TheCustomerSignedUpDaysAgo(int daysSinceSignUp)
    {
        var activityStoreTestAdapter = new ActivityStoreTestAdapter();
        activityStoreTestAdapter.Register(new SignUpActivityEvent(1, DateTime.Now.AddDays(-daysSinceSignUp)));
        scenarioContext.Add(ActivityStoreTestAdapterKey, activityStoreTestAdapter);
    }

    [Given("the customer did never sign up")]
    public void TheCustomerDidNeverSignUp()
    {
        var noActivityEventsAreEmitted = new ActivityStoreTestAdapter();
        scenarioContext.Add(ActivityStoreTestAdapterKey, noActivityEventsAreEmitted);
    }

    [When(@"the loyalty profile is evaluated")]
    public void WhenTheLoyaltyProfileIsEvaluated()
    {
        var loyaltyProfile = new LoyaltyProfile(
            scenarioContext.Get<ActivityStoreTestAdapter>(ActivityStoreTestAdapterKey));
        loyaltyProfile.GenerateProfile(1);
        scenarioContext.Add(EvaluationResultKey, loyaltyProfile.Points);
        scenarioContext.Add(LoyaltyProfileUnderTestKey, loyaltyProfile);
    }
    
    [Then("the value for the loyalty points is (.*)")]
    public void ThenTheValueForTheLoyaltyPointsIs(int expectedLoyaltyPoint)
    {
        scenarioContext.TryGetValue<int>(EvaluationResultKey, out var result);
        result.Should().Be(expectedLoyaltyPoint);
    }

    [Then("the loyalty profile emits error (.*)")]
    public void ThenTheLoyaltyProfileEmitsError(ErrorCodes errorCode)
    {
        var loyaltyProfile = scenarioContext.Get<LoyaltyProfile>(LoyaltyProfileUnderTestKey);
        loyaltyProfile.Error.Should().Be(errorCode);
    }

    [AfterScenario("Unregister activity events")]
    public void UnregisterActivityEvents()
    {
        scenarioContext.Remove(ActivityStoreTestAdapterKey);
        scenarioContext.Remove(LoyaltyProfileUnderTestKey);
    }
}