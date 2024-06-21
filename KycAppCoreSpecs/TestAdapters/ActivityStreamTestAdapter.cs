using System.Reactive.Linq;
using KycAppCore.Events;
using KycAppCore.OutPorts;
using System.Reactive.Subjects;

namespace KycAppCoreSpecs.TestAdapters;

public class ActivityStreamTestAdapter(IEnumerable<CustomerActivityEventBase> activityEvents) : ICustomerActivityStream
{
    public IObservable<CustomerActivityEventBase> Consume(int customerId)
    { 
        return activityEvents.ToObservable();
    }
}