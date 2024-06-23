using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCoreSpecs.TestAdapters;

public class ActivityStoreTestAdapter : ICustomerActivityStore
{
    private readonly List<CustomerActivityEventBase> activityEvents = new();

    public IEnumerable<CustomerActivityEventBase> GetEventsFor(int customerId)
    {
        return activityEvents;
    }

    public void Register(CustomerActivityEventBase activityEvent)
    {
        this.activityEvents.Add(activityEvent);
    }
}