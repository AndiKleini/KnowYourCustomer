using KycAppCore.Events;

namespace KycAppCore;

public static class SumUpPointsExtension
{
    public static int SumUpPoints(this IEnumerable<PurchaseEvent> events)
    {
        return events.Sum(s => s.Amount);
    }
}