using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCore;

public class LoyaltyProfile(ICustomerActivityStore activityStore)
{
    private const int PointsForSignupLongtimeAgo = 5;

    public async Task GenerateProfile(int customerId)
    {
        var fraudSuspicions = (await activityStore.GetEventsFor(customerId)).OfType<FraudSuspicionEvent>();
        var fraudSuspicionsResolved =
            (await activityStore.GetEventsFor(customerId)).OfType<FraudSuspicionResolvedEvent>();

        var res =
            from a in fraudSuspicions
            join
                b in fraudSuspicionsResolved
                on a.FraudSuspicionId equals b.FraudSuspicionId into ab
            from match in ab.DefaultIfEmpty()
            select new { a.FraudSuspicionId, resolved = match != null };
        
        if (res.Any(p => !p.resolved))
        {
            return;
        };
        
        DateTime? signUpDate = (await activityStore.GetEventsFor(customerId)).OfType<SignUpActivityEvent>()
            .FirstOrDefault(s => s.CustomerId == customerId)?.ActivityTimeStamp;
        if (signUpDate == null)
        {
            this.Error = ErrorCodes.UnknownCustomer;
        }
        else
        {
            this.Points += signUpDate.Value.Date < DateTime.Now.AddDays(-365).Date ? PointsForSignupLongtimeAgo : 0;
        }

        this.Points += 2* (await activityStore.GetEventsFor(customerId)).OfType<PurchaseEvent>().
            Where(p => p.ActivityTimeStamp > DateTime.Now.AddDays(-30)).
            Sum(p => p.Amount / 100);
    }
    
    public int Points { get; private set; }
    public ErrorCodes Error { get; private set; }
}