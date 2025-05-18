using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype; // collard-DetailExaminableGlowup
using Robust.Shared.Prototypes; // collard-DetailExaminableGlowup

namespace Content.Server.DetailExaminable;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DetailExaminableComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public string Content = string.Empty;

    // collard-DetailExaminableGlowup-start
    [DataField("poseContent")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string PoseContent = string.Empty;

    [DataField("changePoseAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    [AutoNetworkedField]
    public string ChangePoseAction = "ActionChangePose";

    [DataField("changeFlavorAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    [AutoNetworkedField]
    public string ChangeFlavorAction = "ActionChangeFlavor";

    [DataField("poseActionEntity")]
    [AutoNetworkedField]
    public EntityUid? PoseActionEntity;

    [DataField("flavorActionEntity")]
    [AutoNetworkedField]
    public EntityUid? FlavorActionEntity;
    // collard-DetailExaminableGlowup-end
}
