using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype; // collard-DetailExaminableGlowup
using Robust.Shared.Prototypes; // collard-DetailExaminableGlowup

namespace Content.Server.DetailExaminable;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DetailExaminableComponent : Component
{
    [DataField, AutoNetworkedField]
    public string Content = string.Empty;

    // collard-DetailExaminableGlowup-start
    [DataField]
    public string PoseContent = string.Empty;

    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    [AutoNetworkedField]
    public string ChangePoseAction = "ActionChangePose";

    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    [AutoNetworkedField]
    public string ChangeFlavorAction = "ActionChangeFlavor";

    [DataField]
    [AutoNetworkedField]
    public EntityUid? PoseActionEntity;

    [DataField]
    [AutoNetworkedField]
    public EntityUid? FlavorActionEntity;
    // collard-DetailExaminableGlowup-end
}
