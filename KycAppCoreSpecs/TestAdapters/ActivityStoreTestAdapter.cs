using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCoreSpecs.TestAdapters;

public class ActivityStoreTestAdapter(IEnumerable<CustomerActivityEventBase> activityEvents) : ICustomerActivityStore
{
    public IEnumerable<CustomerActivityEventBase> Consume(int customerId)
    {
        return activityEvents;
    }
}