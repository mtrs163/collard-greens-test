using Content.Shared.Access.Systems;
using Content.Shared.Lock;
using Content.Shared.Power;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;
using Content.Shared.Atmos;
using Robust.Shared.Collections;
using Content.Shared.Damage.Systems;
using System.Data;

namespace Content.Shared.Collard.Grill;

public abstract partial class SharedGrillSystem : EntitySystem
{
    [Dependency] protected IGameTiming Timing = default!;
    [Dependency] private AccessReaderSystem _accessReader = default!;
    [Dependency] private SharedEntityStorageSystem _entityStorage = default!;
    [Dependency] protected SharedIdCardSystem IdCard = default!;
    [Dependency] private LockSystem _lock = default!;
    [Dependency] protected MetaDataSystem MetaDataSystem = default!;
    [Dependency] private SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] protected SharedAudioSystem Audio = default!;
    [Dependency] private DamageableSystem _damageable = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillPlatenCloseMessage>(OnPlatenCloseRequest);
        SubscribeLocalEvent<ClamshellGrillComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillPlatenOpenMessage>(OnPlatenOpenRequest);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillStopSoundsMessage>(OnStopSounds);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillProgramCreatedMessage>(OnProgramCreated);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillProgramDeletedMessage>(OnProgramDeleted);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillStateChange>(OnStateChangeRequest);
        SubscribeLocalEvent<ClamshellGrillComponent, StorageCloseAttemptEvent>(OnCloseAttempt);
        SubscribeLocalEvent<ClamshellGrillComponent, LockToggleAttemptEvent>(OnLockToggleAttempt);
        SubscribeLocalEvent<ClamshellGrillComponent, LockToggledEvent>(OnLockToggled);
        SubscribeLocalEvent<ClamshellGrillComponent, StorageAfterCloseEvent>(OnClosed);
        SubscribeLocalEvent<ClamshellGrillComponent, StorageBeforeOpenEvent>(OnOpen);
    }

    private void OnPowerChanged(Entity<ClamshellGrillComponent> ent, ref PowerChangedEvent args)
    {
        if (args.Powered == false)
        {
            ent.Comp.CurrentState = GrillState.Unpowered;
            ent.Comp.NextState = GrillState.Unpowered;
            ent.Comp.AudioStream = Audio.Stop(ent.Comp.AudioStream);
        }
        else
        {
            ent.Comp.CurrentState = GrillState.MainMenu;
            ent.Comp.NextState = GrillState.MainMenu;
            _lock.Unlock(ent.Owner, ent.Owner);
            _entityStorage.OpenStorage(ent.Owner);
        }
        Dirty(ent);
    }

    private void OnPlatenCloseRequest(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillPlatenCloseMessage args)
    {
        // validation.
        if (string.IsNullOrWhiteSpace(args.Program.Name) ||
            args.Program.Time < 0 ||
            args.Program.Temperature < 0)
        {
            return;
        }

        _lock.Lock(ent.Owner, args.Actor);
        _entityStorage.CloseStorage(ent.Owner);

        var isStandby = false;
        if (args.Program.Time == 0) isStandby = true;
        ent.Comp.SelectedProgram = args.Program;
        ent.Comp.TimeoutTime = Timing.CurTime + TimeSpan.FromSeconds(args.Program.Time - 5 + ent.Comp.PlatenMoveDuration);

        ClosePlaten(ent, args.Program, isStandby);
        Dirty(ent);
    }

    private void OnPlatenOpenRequest(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillPlatenOpenMessage args)
    {

        _lock.Unlock(ent.Owner, args.Actor);
        _entityStorage.OpenStorage(ent.Owner);

        OpenPlaten(ent, args.Error, args.Silent);
        Dirty(ent);
    }

    private void OnProgramCreated(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillProgramCreatedMessage args)
    {
        if (!_accessReader.IsAllowed(args.Actor, ent))
            return;
        ent.Comp.SavedPrograms.Add(new GrillProgram(args.Name, args.Time, args.Temp));
        // set main menu state for the UI
        ent.Comp.CurrentState = GrillState.MainMenu;
        ent.Comp.NextState = GrillState.MainMenu;
        Dirty(ent);
    }

    private void OnProgramDeleted(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillProgramDeletedMessage args)
    {
        if (!_accessReader.IsAllowed(args.Actor, ent))
            return;
        ent.Comp.SavedPrograms.Remove(args.Program);
        Dirty(ent);
    }

    private void OnStateChangeRequest(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillStateChange args)
    {
        ent.Comp.CurrentState = args.State;
        ent.Comp.NextState = args.State;
        Dirty(ent);
    }

    private void OnStopSounds(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillStopSoundsMessage args)
    {
        ent.Comp.AudioStream = Audio.Stop(ent.Comp.AudioStream);
        Dirty(ent);
    }

    private void OnCloseAttempt(Entity<ClamshellGrillComponent> ent, ref StorageCloseAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        args.Cancelled = true;

        if (args.User is not { } user)
            return;

        // my heart yearns for this to be predicted but for some reason opening an entitystorage via
        // verb does not predict it properly.
        _userInterface.TryOpenUi(ent.Owner, ClamshellGrillUiKey.Key, user);
        Dirty(ent);
    }

    private void OnLockToggleAttempt(Entity<ClamshellGrillComponent> ent, ref LockToggleAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        args.Cancelled = true;

        // my heart yearns for this to be predicted but for some reason opening an entitystorage via
        // verb does not predict it properly.
        _userInterface.TryOpenUi(ent.Owner, ClamshellGrillUiKey.Key, args.User);
        Dirty(ent);
    }

    private void OnLockToggled(Entity<ClamshellGrillComponent> ent, ref LockToggledEvent args)
    {
        if (args.Locked)
            return;

        // If we unlock the door, then we're gonna reset the ID.
    }

    private void OnOpen(Entity<ClamshellGrillComponent> ent, ref StorageBeforeOpenEvent args)
    {
        if (!TryComp<EntityStorageComponent>(ent, out var storage)) return;
        foreach (var item in storage.Contents.ContainedEntities)
        {
            RemCompDeferred<ActivelyGrilledComponent>(item);
        }
        Dirty(ent);
    }

    private void OnClosed(Entity<ClamshellGrillComponent> ent, ref StorageAfterCloseEvent args)
    {
        if (!TryComp<EntityStorageComponent>(ent, out var storage)) return;
        foreach (var item in storage.Contents.ContainedEntities)
        {
            var grilledComp = EnsureComp<ActivelyGrilledComponent>(item);
            grilledComp.Grill = ent.Owner;
        }
        Dirty(ent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ClamshellGrillComponent, EntityStorageComponent>();
        while (query.MoveNext(out var uid, out var grill, out var storage))
        {

            if (grill.NextSecond < Timing.CurTime && (grill.CurrentState == GrillState.Cooking || grill.CurrentState == GrillState.Standby))
            {
                var contents = new ValueList<EntityUid>(storage.Contents.ContainedEntities);
                foreach (var contained in contents)
                {
                    _damageable.TryChangeDamage(contained, grill.CrushingDamage);
                }
                grill.NextSecond += TimeSpan.FromSeconds(1);
                Dirty(uid, grill);
            }

            if (grill.OperationEndTime < Timing.CurTime && grill.NextState == GrillState.Cooking)
            {
                if (grill.SelectedProgram is null) continue;
                grill.NextSecond = Timing.CurTime + TimeSpan.FromSeconds(1);
                grill.OperationEndTime = Timing.CurTime + TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time);
                grill.StartTime = Timing.CurTime;
                grill.CurrentState = GrillState.Cooking;
                grill.NextState = GrillState.Opening;
                grill.AudioStream = Audio.Stop(grill.AudioStream);
                Dirty(uid, grill);
            }

            if (grill.OperationEndTime < Timing.CurTime && grill.NextState == GrillState.Standby)
            {
                grill.NextSecond = Timing.CurTime + TimeSpan.FromSeconds(1);
                grill.CurrentState = GrillState.Standby;
                grill.NextState = GrillState.Standby;
                grill.AudioStream = Audio.Stop(grill.AudioStream);
                Dirty(uid, grill);
            }

            if (grill.OperationEndTime < Timing.CurTime && grill.CurrentState == GrillState.Cooking)
            {
                grill.CurrentState = GrillState.Cooking;
                grill.NextState = GrillState.Opening;
                _lock.Unlock(uid, uid);
                _entityStorage.OpenStorage(uid);
                PlayDoneSound((uid, grill));
                OpenPlaten((uid, grill), false, false);
            }

            if (grill.OperationEndTime < Timing.CurTime && grill.NextState == GrillState.SelectingProgram)
            {
                grill.CurrentState = GrillState.SelectingProgram;
                grill.NextState = GrillState.SelectingProgram;
                grill.AudioStream = Audio.Stop(grill.AudioStream);
                Dirty(uid, grill);
            }

            if (grill.TimeoutTime < Timing.CurTime && grill.TimeoutTime != TimeSpan.Zero && grill.CurrentState == GrillState.Cooking)
            {
                PlayTimeoutSound((uid, grill));
                grill.TimeoutTime = TimeSpan.Zero;
                Dirty(uid, grill);
            }

            if (grill.CurrentState == GrillState.Cooking)
            {
                if (grill.SelectedProgram is null) continue;
                storage.Air.Temperature = grill.SelectedProgram.Value.Temperature + Atmospherics.T0C;
                Dirty(uid, grill);
            }

            if (grill.OperationEndTime < Timing.CurTime && grill.CurrentState != GrillState.Cancelling)
            {
                grill.AudioStream = Audio.Stop(grill.AudioStream);
                Dirty(uid, grill);
            }
        }
    }

    protected virtual void ClosePlaten(Entity<ClamshellGrillComponent> ent, GrillProgram program, bool isStandby)
    {

    }

    protected virtual void OpenPlaten(Entity<ClamshellGrillComponent> ent, bool error, bool silent)
    {

    }

    protected virtual void PlayTimeoutSound(Entity<ClamshellGrillComponent> ent)
    {

    }

    protected virtual void PlayDoneSound(Entity<ClamshellGrillComponent> ent)
    {

    }
}
