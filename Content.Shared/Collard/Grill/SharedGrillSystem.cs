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
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
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

    // CCvar.
    // private int _maxIdJobLength;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ClamshellGrillComponent, ClamshellGrillStartedMessage>(OnIdConfigured);
        SubscribeLocalEvent<ClamshellGrillComponent, StorageCloseAttemptEvent>(OnCloseAttempt);
        SubscribeLocalEvent<ClamshellGrillComponent, LockToggleAttemptEvent>(OnLockToggleAttempt);
        SubscribeLocalEvent<ClamshellGrillComponent, LockToggledEvent>(OnLockToggled);
        SubscribeLocalEvent<ClamshellGrillComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
        // SubscribeLocalEvent<GenpopIdCardComponent, ExaminedEvent>(OnExamine);

        //Subs.CVar(_cfgManager, CCVars.MaxIdJobLength, value => _maxIdJobLength = value, true);
    }

    private void OnIdConfigured(Entity<ClamshellGrillComponent> ent, ref ClamshellGrillStartedMessage args)
    {
        // validation.
        if (string.IsNullOrWhiteSpace(args.Name) || // args.Name.Length > _maxIdJobLength ||
            args.Sentence < 0 ||
            args.Crime < 0)
        {
            return;
        }

        if (!_accessReader.IsAllowed(args.Actor, ent))
            return;

        // We don't spawn the actual ID now because then the locker would eat it.
        // Instead, we just fill in the spot temporarily til the checks pass.
        ent.Comp.LinkedId = EntityUid.Invalid;

        _lock.Lock(ent.Owner, args.Actor);
        _entityStorage.CloseStorage(ent);

        ClosePlaten(ent, args.Name, args.Sentence, args.Crime);
    }

    private void OnCloseAttempt(Entity<ClamshellGrillComponent> ent, ref StorageCloseAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        args.Cancelled = true;

        // if (args.User is not { } user)
        //     return;

        // // my heart yearns for this to be predicted but for some reason opening an entitystorage via
        // // verb does not predict it properly.
        // _userInterface.TryOpenUi(ent.Owner, ClamshellGrillUiKey.Key, user);
    }

    private void OnLockToggleAttempt(Entity<ClamshellGrillComponent> ent, ref LockToggleAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        args.Cancelled = true;
    }

    private void OnLockToggled(Entity<ClamshellGrillComponent> ent, ref LockToggledEvent args)
    {
        if (args.Locked)
            return;

        // If we unlock the door, then we're gonna reset the ID.
        CancelIdCard(ent);
    }

    private void OnGetVerbs(Entity<ClamshellGrillComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (ent.Comp.LinkedId == null)
            return;

        if (!args.CanAccess || !args.CanComplexInteract || !args.CanInteract)
            return;

        if (!TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId, out var expire) ||
            !TryComp<GenpopIdCardComponent>(ent.Comp.LinkedId, out var genpopId))
            return;

        var user = args.User;
        var hasAccess = _accessReader.IsAllowed(args.User, ent);
        args.Verbs.Add(new Verb // End sentence early.
        {
            Act = () =>
            {
                IdCard.ExpireId((ent.Comp.LinkedId.Value, expire));
            },
            Priority = 13,
            Text = Loc.GetString("genpop-locker-action-end-early"),
            Impact = LogImpact.Medium,
            DoContactInteraction = true,
            Disabled = !hasAccess,
        });

        args.Verbs.Add(new Verb // Cancel Sentence.
        {
            Act = () =>
            {
                CancelIdCard(ent, user);
            },
            Priority = 12,
            Text = Loc.GetString("genpop-locker-action-clear-id"),
            Impact = LogImpact.Medium,
            DoContactInteraction = true,
            Disabled = !hasAccess,
        });

        var servedTime = 1 - (expire.ExpireTime - Timing.CurTime).TotalSeconds / genpopId.SentenceDuration.TotalSeconds;

        // Can't reset it after its expired.
        if (expire.Expired)
            return;

        args.Verbs.Add(new Verb // Reset Sentence.
        {
            Act = () =>
            {
                IdCard.SetExpireTime((ent.Comp.LinkedId.Value, expire), Timing.CurTime + genpopId.SentenceDuration);
            },
            Priority = 11,
            Text = Loc.GetString("genpop-locker-action-reset-sentence", ("percent", Math.Clamp(servedTime, 0, 1) * 100)),
            Impact = LogImpact.Medium,
            DoContactInteraction = true,
            Disabled = !hasAccess,
        });
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
            if (grill.CurrentState == grill.NextState)
            {
                grill.AudioStream = Audio.Stop(grill.AudioStream);
                continue;
            }

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

            if (grill.OperationEndTime < Timing.CurTime)
            {
                grill.CurrentState = grill.NextState;
                grill.AudioStream = Audio.Stop(grill.AudioStream);
            }
        }
    }

    protected virtual void ClosePlaten(Entity<ClamshellGrillComponent> ent, string name, float sentence, float crime)
    {

    }
}
