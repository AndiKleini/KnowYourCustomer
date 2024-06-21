using System.Reactive.Linq;
using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCore;

public class LoyaltyProfile
{
    public void GenerateProfile(int customerId)
    {
        ActivityStreamProvider.GetStream().Consume(customerId).OfType<SignUpActivityEvent>().
            Cast<SignUpActivityEvent>().
            Subscribe(s =>
            {
                this.Points = s.ActivityTimeStamp < DateTime.Now.AddYears(-1) ? 5 : 0;
            });
    }

    public int Points { get; private set; }
}