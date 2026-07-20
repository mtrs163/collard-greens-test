using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Shared.Security.Components;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Audio;

namespace Content.Shared.Collard.Grill;

/// <summary>
/// This is used for a locker that automatically sets up and handles a <see cref="GenpopIdCardComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class ClamshellGrillComponent : Component
{
    public const int MaxCrimeLength = 48;

    /// <summary>
    /// The <see cref="GenpopIdCardComponent"/> that this locker is currently associated with.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? LinkedId;

    /// <summary>
    /// The Prototype spawned.
    /// </summary>
    [DataField]
    public EntProtoId<GenpopIdCardComponent> IdCardProto = "PrisonerIDCard";

    [DataField, AutoNetworkedField]
    public GrillState CurrentState = GrillState.Ready;

    [DataField, AutoNetworkedField]
    public GrillState NextState = GrillState.Ready;

    /// <summary>
    /// When the current operation will end.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan OperationEndTime;

    /// <summary>
    /// The next second.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan NextSecond;

    /// <summary>
    /// The total duration of the platen moving.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float PlatenMoveDuration = 5f;

    [DataField, AutoNetworkedField]
    public EntityUid? AudioStream;

    [DataField]
    public SoundSpecifier PlatenMovingSound = new SoundPathSpecifier("/Audio/Collard/Effects/Grill/grill_platen_moving_loop.ogg");

    [DataField]
    public SoundSpecifier ErrorSound = new SoundPathSpecifier("/Audio/Collard/Effects/Grill/grill_error_loop.ogg");

    [DataField]
    public SoundSpecifier TimeSound = new SoundPathSpecifier("/Audio/Collard/Effects/Grill/grill_5sec_alarm.ogg");

    [DataField]
    public SoundSpecifier DoneSound = new SoundPathSpecifier("/Audio/Collard/Effects/Grill/grill_done.ogg");
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillStartedMessage : BoundUserInterfaceMessage
{
    public string Name;
    public float Sentence;
    public float Crime;

    public ClamshellGrillStartedMessage(string name, float sentence, float crime)
    {
        Name = name;
        Sentence = sentence;
        Crime = crime;
    }
}

[Serializable, NetSerializable]
public enum ClamshellGrillUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public enum GrillState : byte
{
    Opening,
    Closing,
    Cooking,
    Resting,
    Ready,
    Error,
    OpeningError,
    ClosingError
}
