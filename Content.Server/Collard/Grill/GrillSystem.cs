using Content.Server.Construction;
using Content.Shared.Collard.Grill;
using Robust.Shared.Audio;

namespace Content.Server.Collard.Grill;

public sealed partial class GrillSystem : SharedGrillSystem
{

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ActivelyGrilledComponent, OnConstructionTemperatureEvent>(OnConstructionTemp);
    }

    protected override void ClosePlaten(Entity<ClamshellGrillComponent> ent, GrillProgram program, bool isStandby)
    {
        ent.Comp.OperationEndTime = Timing.CurTime + TimeSpan.FromSeconds(ent.Comp.PlatenMoveDuration);
        ent.Comp.CurrentState = GrillState.Closing;
        ent.Comp.AudioStream = Audio.PlayPvs(ent.Comp.PlatenMovingSound, ent, AudioParams.Default.WithMaxDistance(10f).WithLoop(true))?.Entity;
        if (isStandby)
        {
            ent.Comp.NextState = GrillState.Standby;
            ent.Comp.StartTime = Timing.CurTime;
        }
        else ent.Comp.NextState = GrillState.Cooking;
        Dirty(ent);
    }

    protected override void OpenPlaten(Entity<ClamshellGrillComponent> ent, bool error, bool silent)
    {
        ent.Comp.OperationEndTime = Timing.CurTime + TimeSpan.FromSeconds(ent.Comp.PlatenMoveDuration);
        ent.Comp.CurrentState = GrillState.Opening;
        ent.Comp.NextState = GrillState.SelectingProgram;
        if (silent)
        {
            Dirty(ent);
            return;
        }
        else if (error)
        {
            ent.Comp.AudioStream = Audio.PlayPvs(ent.Comp.ErrorSound, ent, AudioParams.Default.WithMaxDistance(10f).WithLoop(true))?.Entity;
            ent.Comp.CurrentState = GrillState.Cancelling;
            ent.Comp.NextState = GrillState.Cancelling;
        }
        else ent.Comp.AudioStream = Audio.PlayPvs(ent.Comp.PlatenMovingSound, ent, AudioParams.Default.WithMaxDistance(10f).WithLoop(true))?.Entity;
        Dirty(ent);
    }

    protected override void PlayTimeoutSound(Entity<ClamshellGrillComponent> ent)
    {
        ent.Comp.AudioStream = Audio.PlayPvs(ent.Comp.TimeSound, ent, AudioParams.Default.WithMaxDistance(10f))?.Entity;
        Dirty(ent);
    }

    protected override void PlayDoneSound(Entity<ClamshellGrillComponent> ent)
    {
        ent.Comp.AudioStream = Audio.PlayPvs(ent.Comp.DoneSound, ent, AudioParams.Default.WithMaxDistance(10f))?.Entity;
        Dirty(ent);
    }

    // Stop items from transforming through constructiongraphs while being grilled.
    // They spawn outside of the grill
    private void OnConstructionTemp(Entity<ActivelyGrilledComponent> ent, ref OnConstructionTemperatureEvent args)
    {
        args.Result = HandleResult.False;
    }
}
