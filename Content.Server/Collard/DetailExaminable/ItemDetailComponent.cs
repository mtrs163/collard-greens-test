using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;

namespace Content.Server.Collard.DetailExaminable

{
    [RegisterComponent]
    public sealed partial class ItemDetailComponent : Component
    {
        [DataField]
        public string Content = "Никто не установил описание для данного предмета.";

    }
}
