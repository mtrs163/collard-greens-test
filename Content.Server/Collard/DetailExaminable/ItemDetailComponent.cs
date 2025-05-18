using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;

namespace Content.Server.Collard.DetailExaminable

{
    [RegisterComponent]
    public sealed partial class ItemDetailComponent : Component
    {
        [DataField("content", required: true)] [ViewVariables(VVAccess.ReadWrite)]
        public string Content = string.Empty;

    }
}
