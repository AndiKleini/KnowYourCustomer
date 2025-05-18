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
            this.Points += signUpDate.Value.Date < DateTime.Now.AddDays(-365).Date ? PointsForSignupLongtimeAgo : 0;
        }
    }
    
    public int Points { get; private set; }
    public ErrorCodes Error { get; private set; }
}