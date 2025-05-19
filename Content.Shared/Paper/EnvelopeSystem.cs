using Content.Shared.DoAfter;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Content.Shared.Examine;
using Content.Shared.Storage; // collard-EnvelopeGlowup
using Content.Shared.Storage.Components; // collard-EnvelopeGlowup

namespace Content.Shared.Paper;

public sealed class EnvelopeSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!; // collard-EnvelopeGlowup

    public override void Initialize()
    {
        base.Initialize();

        /*SubscribeLocalEvent<EnvelopeComponent, ItemSlotInsertAttemptEvent>(OnInsertAttempt); collard-EnvelopeGlowup
        SubscribeLocalEvent<EnvelopeComponent, ItemSlotEjectAttemptEvent>(OnEjectAttempt);*/
        SubscribeLocalEvent<EnvelopeComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<EnvelopeComponent, EnvelopeDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<EnvelopeComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<EnvelopeComponent, StorageOpenAttemptEvent>(OnStorageOpenAttempt); // collard-EnvelopeGlowup
        SubscribeLocalEvent<EnvelopeComponent, StorageInteractAttemptEvent>(OnStorageInteractAttempt); // collard-EnvelopeGlowup
    }

    private void OnExamine(Entity<EnvelopeComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed)
        {
            args.PushMarkup(Loc.GetString("envelope-sealed-examine", ("envelope", ent.Owner)));
        }
        else if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Torn)
        {
            args.PushMarkup(Loc.GetString("envelope-torn-examine", ("envelope", ent.Owner)));
        }
    }

    private void OnGetAltVerbs(Entity<EnvelopeComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || args.Hands == null)
            return;

        if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Torn)
            return;

        var user = args.User;
        args.Verbs.Add(new AlternativeVerb()
        {
            Text = Loc.GetString(ent.Comp.State == EnvelopeComponent.EnvelopeState.Open ? "envelope-verb-seal" : "envelope-verb-tear"),
            IconEntity = GetNetEntity(ent.Owner),
            Act = () =>
            {
                TryStartDoAfter(ent, user, ent.Comp.State == EnvelopeComponent.EnvelopeState.Open ? ent.Comp.SealDelay : ent.Comp.TearDelay);
            },
        });
    }

    // collard-EnvelopeGlowup-start
    /*private void OnInsertAttempt(Entity<EnvelopeComponent> ent, ref ItemSlotInsertAttemptEvent args)
    {
        args.Cancelled |= ent.Comp.State != EnvelopeComponent.EnvelopeState.Open;
    }

    private void OnEjectAttempt(Entity<EnvelopeComponent> ent, ref ItemSlotEjectAttemptEvent args)
    {
        args.Cancelled |= ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed;
    }*/

    private void OnStorageOpenAttempt(Entity<EnvelopeComponent> ent, ref StorageOpenAttemptEvent args)
    {
        args.Cancelled |= ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed;
    }
    private void OnStorageInteractAttempt(Entity<EnvelopeComponent> ent, ref StorageInteractAttemptEvent args)
    {
        args.Cancelled |= ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed;
    }
    // collard-EnvelopeGlowup-end

    private void TryStartDoAfter(Entity<EnvelopeComponent> ent, EntityUid user, TimeSpan delay)
    {
        if (ent.Comp.EnvelopeDoAfter.HasValue)
            return;

        var doAfterEventArgs = new DoAfterArgs(EntityManager, user, delay, new EnvelopeDoAfterEvent(), ent.Owner, ent.Owner)
        {
            BreakOnDamage = true,
            NeedHand = true,
            BreakOnHandChange = true,
            MovementThreshold = 0.01f,
            DistanceThreshold = 1.0f,
        };

        if (_doAfterSystem.TryStartDoAfter(doAfterEventArgs, out var doAfterId))
            ent.Comp.EnvelopeDoAfter = doAfterId;
    }
    private void OnDoAfter(Entity<EnvelopeComponent> ent, ref EnvelopeDoAfterEvent args)
    {
        ent.Comp.EnvelopeDoAfter = null;

        if (args.Cancelled)
            return;

        if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Open)
        {
            _audioSystem.PlayPredicted(ent.Comp.SealSound, ent.Owner, args.User);
            ent.Comp.State = EnvelopeComponent.EnvelopeState.Sealed;
            Dirty(ent.Owner, ent.Comp);
            _uiSystem.CloseUi(ent.Owner, StorageComponent.StorageUiKey.Key); // collard-EnvelopeGlowup
        }
        else if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed)
        {
            _audioSystem.PlayPredicted(ent.Comp.TearSound, ent.Owner, args.User);
            ent.Comp.State = EnvelopeComponent.EnvelopeState.Torn;
            Dirty(ent.Owner, ent.Comp);

            /*if (_itemSlotsSystem.TryGetSlot(ent.Owner, ent.Comp.SlotId, out var slotComp)) // collard-EnvelopeGlowup
                _itemSlotsSystem.TryEjectToHands(ent.Owner, slotComp, args.User);*/
        }
    }
}
