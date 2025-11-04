using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Effects;

/// <summary>
/// Will play a sound in PVS range when triggered.
/// If TargetUser is true it will be played at their position.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EmitSoundOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// The <see cref="SoundSpecifier"/> to play.
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public SoundSpecifier? Sound;

    // collard-DistressFlare-start
    /// <summary>
    /// Should the sound be one-time only, or can be activated numerous times.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool OneTime = false;

    /// <summary>
    /// Was this sound triggered before.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Triggered = false;
    // collard-DistressFlare-end

    /// <summary>
    /// Play the sound at the position instead of parented to the source entity.
    /// Useful if the entity is deleted after.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Positional;

    /// <summary>
    /// Should this sound be predicted for the User?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Predicted;
}
