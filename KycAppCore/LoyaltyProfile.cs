using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCore;

public class LoyaltyProfile(ICustomerActivityStore activityStore)
{
    private const int POINTS_FOR_SIGNUP_LONGTIME_AGO = 5;

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
            this.Points = signUpDate <= DateTime.Now.AddYears(-1) ? POINTS_FOR_SIGNUP_LONGTIME_AGO : 0;
        }

        var spentWithinTheLast30Days = (await activityStore.GetEventsFor(customerId)).
            OfType<PurchaseEvent>().
            Where(s => s.ActivityTimeStamp >= DateTime.Now.AddDays(-30)).
            Sum(s => s.Amount);

        this.Points += spentWithinTheLast30Days * 2 / 100;
    }

    public int Points { get; private set; }
    public ErrorCodes Error { get; private set; }
}