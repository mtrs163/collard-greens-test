using Content.Shared.Medical;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Content.Server.Body.Systems;

namespace Content.Server.Collard.Destructible.Thresholds.Behaviors;

[DataDefinition]
public sealed partial class BloodVomitBehavior : IThresholdBehavior
{
    public void Execute(EntityUid uid, DestructibleSystem system, EntityUid? cause = null)
    {
        system.EntityManager.System<VomitSystem>().Vomit(uid);
        system.EntityManager.System<BloodstreamSystem>().TryModifyBloodLevel(uid, -50);
    }
}
