using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCore;

public class LoyaltyProfile(ICustomerActivityStore activityStore)
{
    private const int POINTS_FOR_SIGNUP_LONGTIME_AGO = 5;

    public void GenerateProfile(int customerId)
    {
        var signUpDate = activityStore.Consume(customerId).OfType<SignUpActivityEvent>()
            .First().ActivityTimeStamp;
        this.Points = signUpDate <= DateTime.Now.AddYears(-1) ? POINTS_FOR_SIGNUP_LONGTIME_AGO : 0;
    }

    public int Points { get; private set; }
}