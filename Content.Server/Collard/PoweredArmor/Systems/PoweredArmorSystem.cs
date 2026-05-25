using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Collard.PoweredArmor;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Content.Shared.Verbs;
using Robust.Shared.Utility;
using Content.Shared.Armor;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Network;

namespace Content.Server.Collard.PoweredArmor.Systems
{
    public sealed class PoweredArmorSystem : EntitySystem
    {
        [Dependency] private readonly RiggableSystem _riggableSystem = default!;
        [Dependency] private readonly SharedBatterySystem _battery = default!;
        [Dependency] private readonly ItemToggleSystem _itemToggle = default!;
        [Dependency] private readonly ExamineSystemShared _examine = default!;
        [Dependency] private readonly AudioSystem _audio = default!;
        [Dependency] private readonly INetManager _net = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<PoweredArmorComponent, SolutionContainerChangedEvent>(OnSolutionChange);
            SubscribeLocalEvent<PoweredArmorComponent, ChargeChangedEvent>(OnChargeChanged);
            SubscribeLocalEvent<PoweredArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>(OnDamageModify);
            SubscribeLocalEvent<PoweredArmorComponent, GetVerbsEvent<ExamineVerb>>(OnArmorVerbExamine);
        }

        public static readonly EntProtoId ShieldHitEffectPrototype = "EffectEmpDisabled";

        private void OnDamageModify(EntityUid uid, PoweredArmorComponent component, InventoryRelayedEvent<DamageModifyEvent> args)
        {
            if (TryComp<MaskComponent>(uid, out var mask) && mask.IsToggled)
                return;

            if (!TryComp<BatteryComponent>(uid, out var battery) || battery.State == BatteryState.Empty)
                return;

            if (_battery.GetCharge((uid, battery)) < args.Args.OriginalDamage.GetTotal().Float() * 10)
            {
                _battery.SetCharge((uid, battery), 0);
                return;
            }

            var hasNeededDamageType = false;
            foreach (var damageType in component.Modifiers.Coefficients)
            {
                if (args.Args.OriginalDamage.DamageDict.ContainsKey(damageType.Key))
                {
                    hasNeededDamageType = true;
                    break;
                }
            }
            if (!hasNeededDamageType) return;

            if (!_battery.TryUseCharge((uid, battery), args.Args.OriginalDamage.GetTotal().Float() * 10))
            {
                return;
            }
            _audio.PlayPvs(component.HitSound, uid, AudioParams.Default);
            args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, component.Modifiers);
            if (_net.IsServer)
                Spawn(ShieldHitEffectPrototype, Transform(uid).Coordinates);
        }

        private void OnSolutionChange(Entity<PoweredArmorComponent> entity, ref SolutionContainerChangedEvent args)
        {
            if (!TryComp<RiggableComponent>(entity, out var riggable) ||
                !TryComp<BatteryComponent>(entity, out var battery))
                return;

            if (_itemToggle.IsActivated(entity.Owner) && riggable.IsRigged)
                _riggableSystem.Explode(entity.Owner, _battery.GetCharge((entity, battery)));
        }

        private void OnChargeChanged(Entity<PoweredArmorComponent> entity, ref ChargeChangedEvent args)
        {
            if (TryComp<BatteryComponent>(entity.Owner, out var battery) &&
                _battery.GetCharge((entity.Owner, battery)) < 1)
            {
                _itemToggle.TryDeactivate(entity.Owner, predicted: false);
            }
            else _itemToggle.TryActivate(entity.Owner, predicted: false);
        }

        private void OnArmorVerbExamine(EntityUid uid, PoweredArmorComponent component, GetVerbsEvent<ExamineVerb> args)
        {
            if (!args.CanInteract || !args.CanAccess || !component.ShowArmorOnExamine)
                return;

            var examineMarkup = GetArmorExamine(component.Modifiers);

            var ev = new ArmorExamineEvent(examineMarkup);
            RaiseLocalEvent(uid, ref ev);

            _examine.AddDetailedExamineVerb(args, component, examineMarkup,
                Loc.GetString("armor-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png",
                Loc.GetString("armor-examinable-verb-message"));
        }

        private FormattedMessage GetArmorExamine(DamageModifierSet armorModifiers)
        {
            var msg = new FormattedMessage();
            msg.AddMarkupOrThrow(Loc.GetString("powered-armor-examine"));

            foreach (var coefficientArmor in armorModifiers.Coefficients)
            {
                msg.PushNewline();

                var armorType = Loc.GetString("armor-damage-type-" + coefficientArmor.Key.ToLower());
                msg.AddMarkupOrThrow(Loc.GetString("armor-coefficient-value",
                    ("type", armorType),
                    ("value", MathF.Round((1f - coefficientArmor.Value) * 100, 1))
                ));
            }

            foreach (var flatArmor in armorModifiers.FlatReduction)
            {
                msg.PushNewline();

                var armorType = Loc.GetString("armor-damage-type-" + flatArmor.Key.ToLower());
                msg.AddMarkupOrThrow(Loc.GetString("armor-reduction-value",
                    ("type", armorType),
                    ("value", flatArmor.Value)
                ));
            }

            return msg;
        }
    }
}
