using FluentAssertions;
using KycAppCore;
using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCoreTests;


public class LoyaltyProfileTests
{
    [Test]
    public void GenerateProfile_MultipleSignUpEventsFromDifferentCustomersInStream_PicksSignUpEventFromSpecifiedCustomer()
    {
        int customerSignedUpMoreThan365DaysAgo = 1;
        int customerSignedUpYesterday = 2;
        var activityStoreMock = new Moq.Mock<ICustomerActivityStore>();
        activityStoreMock.Setup(s => s.Consume(customerSignedUpMoreThan365DaysAgo)).Returns(
            new List<CustomerActivityEventBase>() 
            {
                new SignUpActivityEvent(customerSignedUpYesterday, DateTime.Now.AddDays(-1)),
                new SignUpActivityEvent(customerSignedUpMoreThan365DaysAgo, DateTime.Now.AddDays(-500))
            });
        var instanceUnderTest = new LoyaltyProfile(activityStoreMock.Object);

        instanceUnderTest.GenerateProfile(customerSignedUpMoreThan365DaysAgo);

        instanceUnderTest.Points.Should().Be(5);
    }
}