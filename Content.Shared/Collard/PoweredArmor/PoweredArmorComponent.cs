using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.Utility;

namespace Content.Shared.Collard.PoweredArmor;

[RegisterComponent, NetworkedComponent]
//[Access(typeof(SharedStunbatonSystem))]
public sealed partial class PoweredArmorComponent : Component
{
    [DataField]
    public SoundSpecifier SparksSound = new SoundCollectionSpecifier("sparks");

    /// <summary>
    /// The damage reduction
    /// </summary>
    [DataField(required: true)]
    public DamageModifierSet Modifiers = default!;

    /// <summary>
    /// If true, you can examine the armor to see the protection. If false, the verb won't appear.
    /// </summary>
    [DataField]
    public bool ShowArmorOnExamine = true;

    [DataField]
    public SoundSpecifier? HitSound = new SoundCollectionSpecifier("sparks");
}

