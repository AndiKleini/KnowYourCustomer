using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCore;

public class LoyaltyProfile(ICustomerActivityStore activityStore)
{
    private const int PointsForSignupLongtimeAgo = 5;

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
            this.Points = signUpDate <= OneYearAgo() ? PointsForSignupLongtimeAgo : 0;
        }
        
        this.Points += (await activityStore.GetEventsFor(customerId)).
            OfType<PurchaseEvent>().
            Where(p => p.ActivityTimeStamp.Date >= ThirtyDaysAgo()).
            
            Sum(s => 2 * s.Amount / 100);
    }

    private static DateTime ThirtyDaysAgo()
    {
        return DateTime.Now.AddDays(-30).Date;
    }

    private static DateTime OneYearAgo()
    {
        return DateTime.Now.AddYears(-1);
    }

    public int Points { get; private set; }
    public ErrorCodes Error { get; private set; }
}