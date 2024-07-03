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
            this.Points = signUpDate <= DateTime.Now.AddYears(-1) ? PointsForSignupLongtimeAgo : 0;
        }
    }
    
    public int Points { get; private set; }
    public ErrorCodes Error { get; private set; }
}