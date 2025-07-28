using Content.Shared.Light.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.VariationPass.Components;

/// <summary>
/// This handle randomly destroying lights, causing them to flicker endlessly, or replacing their tube/bulb with different variants.
/// </summary>
[RegisterComponent]
public sealed partial class PoweredLightVariationPassComponent : Component
{
    /// <summary>
    ///     Chance that a light will be replaced with a broken variant.
    /// </summary>
    [DataField]
    public float LightBreakChance = 0.05f; // collard-VariationDeshittification

    /// <summary>
    ///     Chance that a light will be replaced with an aged variant.
    /// </summary>
    [DataField]
    public float LightAgingChance = 0.03f; // collard-VariationDeshittification

    [DataField]
    public float AgedLightTubeFlickerChance = 0.01f;  // collard-VariationDeshittification

    [DataField]
    public EntProtoId BrokenLightBulbPrototype = "LightBulbBroken";

    [DataField]
    public EntProtoId BrokenLightTubePrototype = "LightTubeBroken";

    [DataField]
    public EntProtoId AgedLightBulbPrototype = "LightBulbOld";

    [DataField]
    public EntProtoId AgedLightTubePrototype = "LightTubeOld";
}
