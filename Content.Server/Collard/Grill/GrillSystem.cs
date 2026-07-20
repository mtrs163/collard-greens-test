using Content.Shared.Collard.Grill;
using Robust.Shared.Audio;

namespace Content.Server.Collard.Grill;

public sealed partial class GrillSystem : SharedGrillSystem
{
    protected override void ClosePlaten(Entity<ClamshellGrillComponent> ent, string name, float sentence, float crime)
    {
        // Default to prisoner locker coordinates for ID spawn
        ent.Comp.AudioStream = Audio.PlayPvs(ent.Comp.PlatenMovingSound, ent, AudioParams.Default.WithMaxDistance(10f).WithLoop(true))?.Entity;
        ent.Comp.NextState = GrillState.Cooking;
        ent.Comp.OperationEndTime = Timing.CurTime + TimeSpan.FromSeconds(ent.Comp.PlatenMoveDuration);
        Dirty(ent);
    }
}
