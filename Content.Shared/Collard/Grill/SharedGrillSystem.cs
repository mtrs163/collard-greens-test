using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Security.Components;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Timing;

namespace Content.Shared.Collard.Grill;

public abstract partial class SharedGrillSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfgManager = default!;
    [Dependency] protected IGameTiming Timing = default!;
    [Dependency] private AccessReaderSystem _accessReader = default!;
    [Dependency] private SharedEntityStorageSystem _entityStorage = default!;
    [Dependency] protected SharedIdCardSystem IdCard = default!;
    [Dependency] private LockSystem _lock = default!;
    [Dependency] protected MetaDataSystem MetaDataSystem = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] protected SharedAudioSystem Audio = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillPlatenCloseMessage>(OnPlatenCloseRequest);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillPlatenOpenMessage>(OnPlatenOpenRequest);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillStopSoundsMessage>(OnStopSounds);
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillProgramCreatedMessage>(OnProgramCreated);
        SubscribeLocalEvent<ClamshellGrillComponent, StorageCloseAttemptEvent>(OnCloseAttempt);
        SubscribeLocalEvent<ClamshellGrillComponent, LockToggleAttemptEvent>(OnLockToggleAttempt);
        SubscribeLocalEvent<ClamshellGrillComponent, LockToggledEvent>(OnLockToggled);
    }

    private void OnPlatenCloseRequest(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillPlatenCloseMessage args)
    {
        // validation.
        if (string.IsNullOrWhiteSpace(args.Program.Name) ||
            args.Program.Time <= 0 ||
            args.Program.Temperature < 0)
        {
            return;
        }

        if (!_accessReader.IsAllowed(args.Actor, ent))
            return;

        _lock.Lock(ent.Owner, args.Actor);
        _entityStorage.CloseStorage(ent);

        var isStandby = false;
        if (args.Program.Time == 0) isStandby = true;
        ent.Comp.SelectedProgram = args.Program;

        ClosePlaten(ent, args.Program, isStandby);
    }

    private void OnPlatenOpenRequest(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillPlatenOpenMessage args)
    {
        if (!_accessReader.IsAllowed(args.Actor, ent))
            return;

        _lock.Unlock(ent.Owner, args.Actor);
        _entityStorage.OpenStorage(ent);

        OpenPlaten(ent, args.Error, args.Silent);
    }

    private void OnProgramCreated(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillProgramCreatedMessage args)
    {
        ent.Comp.SavedPrograms.Add(new GrillProgram(args.Name, args.Time, args.Time));
    }

    private void OnStopSounds(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillStopSoundsMessage args)
    {
        ent.Comp.AudioStream = Audio.Stop(ent.Comp.AudioStream);
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
    }

    private void OnLockToggleAttempt(Entity<ClamshellGrillComponent> ent, ref LockToggleAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        args.Cancelled = true;

        // my heart yearns for this to be predicted but for some reason opening an entitystorage via
        // verb does not predict it properly.
        _userInterface.TryOpenUi(ent.Owner, ClamshellGrillUiKey.Key, args.User);
    }

    private void OnLockToggled(Entity<ClamshellGrillComponent> ent, ref LockToggledEvent args)
    {
        if (args.Locked)
            return;

        // If we unlock the door, then we're gonna reset the ID.
        CancelIdCard(ent);
    }

    private void CancelIdCard(Entity<ClamshellGrillComponent> ent, EntityUid? user = null)
    {
        if (ent.Comp.LinkedId == null)
            return;

        var metaData = MetaData(ent);
        MetaDataSystem.SetEntityName(ent, Loc.GetString("genpop-locker-name-default"), metaData);
        MetaDataSystem.SetEntityDescription(ent, Loc.GetString("genpop-locker-desc-default"), metaData);

        ent.Comp.LinkedId = null;
        _lock.Unlock(ent.Owner, user);
        _entityStorage.OpenStorage(ent.Owner);

        if (TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId, out var expire))
            IdCard.ExpireId((ent.Comp.LinkedId.Value, expire));

        Dirty(ent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ClamshellGrillComponent, EntityStorageComponent>();
        while (query.MoveNext(out var uid, out var grill, out var storage))
        {
            // if (grill.CurrentState == grill.NextState)
            // {
            //     continue;
            // }

            // if (grill.CurrentState == GrillState.Standby) continue;

            // if (grill.NextSecond < Timing.CurTime)
            // {
            //     var contents = new ValueList<EntityUid>(storage.Contents.ContainedEntities);
            //     foreach (var contained in contents)
            //     {
            //         _damageable.TryChangeDamage(contained, grill.CrushingDamage);
            //     }
            //     grill.NextSecond += TimeSpan.FromSeconds(1);
            //     Dirty(uid, grill);
            // }
            if (grill.OperationEndTime < Timing.CurTime && grill.NextState == GrillState.Cooking)
            {
                if (grill.SelectedProgram is null) continue;
                grill.OperationEndTime = Timing.CurTime + TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time);
                grill.CurrentState = GrillState.Cooking;
                grill.NextState = GrillState.Opening;
                grill.AudioStream = Audio.Stop(grill.AudioStream);
            }

            if (grill.OperationEndTime.Subtract(TimeSpan.FromSeconds(5)) < Timing.CurTime && grill.CurrentState == GrillState.Cooking)
            {
                PlayTimeoutSound((uid, grill));
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

            if (grill.OperationEndTime < Timing.CurTime && grill.NextState == GrillState.Ready)
            {
                grill.CurrentState = GrillState.Ready;
                grill.NextState = GrillState.Ready;
                grill.AudioStream = Audio.Stop(grill.AudioStream);
            }

            if (grill.CurrentState == GrillState.Cooking || grill.CurrentState == GrillState.Standby) grill.AudioStream = Audio.Stop(grill.AudioStream);
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
