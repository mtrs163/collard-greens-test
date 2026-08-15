using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Shared.Security.Components;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Audio;
using Content.Shared.Damage;

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
    public GrillState CurrentState = GrillState.Unpowered;

    [DataField, AutoNetworkedField]
    public GrillState NextState = GrillState.Unpowered;

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
    /// Start time.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan StartTime = TimeSpan.Zero;

    /// <summary>
    /// Time at which the timeout sound will play.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan TimeoutTime;

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

    [DataField]
    public HashSet<GrillProgram> SavedPrograms = new();

    [DataField, AutoNetworkedField]
    public GrillProgram? SelectedProgram = null;

    /// <summary>
    /// Damage dealt each second to entities inside while platen is closed.
    /// </summary>
    [DataField]
    public DamageSpecifier CrushingDamage = new();
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillProgramCreatedMessage : BoundUserInterfaceMessage
{
    public string Name;
    public float Time;
    public float Temp;

    public ClamshellGrillProgramCreatedMessage(string name, float time, float temp)
    {
        Name = name;
        Time = time;
        Temp = temp;
    }
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillProgramDeletedMessage : BoundUserInterfaceMessage
{
    public GrillProgram Program;

    public ClamshellGrillProgramDeletedMessage(GrillProgram program)
    {
        Program = program;
    }
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillPlatenCloseMessage : BoundUserInterfaceMessage
{
    public GrillProgram Program;

    public ClamshellGrillPlatenCloseMessage(GrillProgram program)
    {
        Program = program;
    }
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillPlatenOpenMessage : BoundUserInterfaceMessage
{
    public bool Error;
    public bool Silent;

    public ClamshellGrillPlatenOpenMessage(bool error, bool silent)
    {
        Error = error;
        Silent = silent;
    }
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillStateChange : BoundUserInterfaceMessage
{
    public GrillState State;

    public ClamshellGrillStateChange(GrillState state)
    {
        State = state;
    }
}

[Serializable, NetSerializable]
public sealed class ClamshellGrillStopSoundsMessage : BoundUserInterfaceMessage { }

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
    Standby,
    Error,
    Cancelling,
    OpeningError,
    ClosingError,
    SelectingProgram,
    EditingProgram,
    Unpowered,
    MainMenu
}

[DataDefinition, NetSerializable, Serializable]
public readonly partial record struct GrillProgram
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Name { get; init; } = string.Empty;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Time { get; init; } = 60f;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Temperature { get; init; } = 120f;

    public GrillProgram(string name, float time, float temp)
    {
        Name = name;
        Time = time;
        Temperature = temp;
    }
}
