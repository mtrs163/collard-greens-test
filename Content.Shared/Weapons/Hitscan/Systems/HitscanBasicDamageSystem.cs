using Content.Shared.Collard.Dice; //collard-SavingThrows
using Content.Shared.Damage.Systems;
using Content.Shared.Mobs.Components; //collard-SavingThrows
using Content.Shared.Weapons.Hitscan.Components;
using Content.Shared.Weapons.Hitscan.Events;

namespace Content.Shared.Weapons.Hitscan.Systems;

public sealed class HitscanBasicDamageSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SavingThrowSystem _savingThrow = default!; //collard-SavingThrows

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HitscanBasicDamageComponent, HitscanRaycastFiredEvent>(OnHitscanHit);
    }

    private void OnHitscanHit(Entity<HitscanBasicDamageComponent> ent, ref HitscanRaycastFiredEvent args)
    {
        if (args.Data.HitEntity == null)
            return;

        if (HasComp<MobStateComponent>(args.Data.HitEntity.Value)) //collard-SavingThrows
            if (_savingThrow.InitiateSilentSavingThrowPredicted(args.Data.HitEntity.Value, ent.Comp.SavingDifficulty)) return; //collard-SavingThrows

        var dmg = ent.Comp.Damage * _damage.UniversalHitscanDamageModifier;

        if(!_damage.TryChangeDamage(args.Data.HitEntity.Value, dmg, out var damageDealt, origin: args.Data.Gun))
            return;

        var damageEvent = new HitscanDamageDealtEvent
        {
            Target = args.Data.HitEntity.Value,
            DamageDealt = damageDealt,
        };

        RaiseLocalEvent(ent, ref damageEvent);
    }
}
