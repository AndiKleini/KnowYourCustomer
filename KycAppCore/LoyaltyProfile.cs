using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using KycAppCore.Events;
using KycAppCore.OutPorts;

[assembly: InternalsVisibleTo("KYC")]

namespace KycAppCore;

public class LoyaltyProfile(ICustomerActivityStore activityStore)
{
    private const int PointsForSignupLongtimeAgo = 5;

    public async Task GenerateProfile(int customerId)
    {
        var profileData =
            (await activityStore.GetEventsFor(customerId))
            .Aggregate<CustomerActivityEventBase, (int Point, ErrorCodes Error)>(
                (0, ErrorCodes.UnknownCustomer),
                (aggProfileData, ca) =>
                    ca switch
                    {
                        SignUpActivityEvent su =>
                            new(
                                Points = aggProfileData.Point + (su.ActivityTimeStamp.Date < OneYearAgo()
                                    ? PointsForSignupLongtimeAgo
                                    : 0),
                                Error = ErrorCodes.NoError
                            ),
                        PurchaseEvent pe => 
                            new (
                                Points = 
                                    aggProfileData.Point + 
                                    GetLoyaltyAmountOfMoneySpent(pe), Error = aggProfileData.Error
                                ),
                        _ => aggProfileData
                    }
                );
        this.Error = profileData.Error;
        this.Points = profileData.Point;
    }

    internal static int GetLoyaltyAmountOfMoneySpent(PurchaseEvent purchaseEvent)
    {
        if (purchaseEvent?.ActivityTimeStamp.Date < ThirtyDaysAgo())
        {
            return 0;
        }
        
        return purchaseEvent switch
        {
            PurchaseByInstallmentEvent pi => pi.Commission * 2 / 100,
            not null => purchaseEvent.Amount * 2 / 100,
            null => throw new ArgumentNullException()
        };
    }
    
    private static DateTime ThirtyDaysAgo()
    {
        return DateTime.Now.AddDays(-30).Date;
    }

    private static DateTime OneYearAgo()
    {
        return DateTime.Now.AddYears(-1);
    }

    public int Points { get; private set; }
    public ErrorCodes Error { get; private set; }
}