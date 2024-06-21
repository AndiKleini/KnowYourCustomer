using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCoreSpecs.TestAdapters;

public class ActivityStreamTestAdapter : ICustomerActivityStream
{
    private readonly IEnumerable<CustomerActivityEventBase> activityEvents;

    public ActivityStreamTestAdapter(IEnumerable<CustomerActivityEventBase> events)
    {
        this.activityEvents = events;
    }
    
    public IObservable<CustomerActivityEventBase> Consume(int customerId)
    {
        throw new NotImplementedException();
    }
}