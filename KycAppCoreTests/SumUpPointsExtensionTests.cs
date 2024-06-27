using FluentAssertions;
using KycAppCore;
using KycAppCore.Events;

namespace KycAppCoreTests;

public class SumUpPointsExtensionTests
{
    [Test]
    public void SumUpPoints_PurchasesByInstallmentsInPlace_SumUpOnlyMoneySpent()
    {
        int customerId = 1;
        int purchaseAmount = 200;
        int commission = 200;
        IEnumerable<PurchaseEvent> purchaseEvents =
            new[]
            {
                new PurchaseEvent(customerId, DateTime.Now.AddDays(-1), purchaseAmount),
                new PurchaseByInstallmentEvent(customerId, DateTime.Now.AddDays(-2), 300, commission)
            };

        int yieldPoints = purchaseEvents.SumUpPoints();

        yieldPoints.Should().Be(purchaseAmount + commission);
    }   
}