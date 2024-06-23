using FluentAssertions;
using KycAppCore;
using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCoreTests;


public class LoyaltyProfileTests
{
    [Test]
    public void GenerateProfile_MultipleSignUpEventsFromDifferentCustomersRetrieved_PickSignUpEventFromSpecifiedCustomer()
    {
        int customerSignedUpMoreThan365DaysAgo = 1;
        int customerSignedUpYesterday = 2;
        var activityStoreMock = new Moq.Mock<ICustomerActivityStore>();
        activityStoreMock.Setup(s => s.GetEventsFor(customerSignedUpMoreThan365DaysAgo)).Returns(
            new List<CustomerActivityEventBase>() 
            {
                new SignUpActivityEvent(customerSignedUpYesterday, DateTime.Now.AddDays(-1)),
                new SignUpActivityEvent(customerSignedUpMoreThan365DaysAgo, DateTime.Now.AddDays(-500))
            });
        var instanceUnderTest = new LoyaltyProfile(activityStoreMock.Object);

        instanceUnderTest.GenerateProfile(customerSignedUpMoreThan365DaysAgo);

        instanceUnderTest.Points.Should().Be(5);
    }
    
    [Test]
    public void GenerateProfile_NoSignUpEventRetrieved_SwitchToErrorState()
    {
        int customerId = 1;
        var activityStoreMock = new Moq.Mock<ICustomerActivityStore>();
        activityStoreMock.Setup(s => s.GetEventsFor(customerId)).Returns([]);
        var instanceUnderTest = new LoyaltyProfile(activityStoreMock.Object);

        instanceUnderTest.GenerateProfile(customerId);

        instanceUnderTest.Points.Should().Be(0);
        instanceUnderTest.Error.Should().Be(ErrorCodes.UnknownCustomer);
    }
}