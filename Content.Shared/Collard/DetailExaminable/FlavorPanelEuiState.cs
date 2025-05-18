using Content.Shared.Eui;
using Content.Shared.Collard.Humanoid;
using Robust.Shared.Serialization;

namespace Content.Shared.Collard.DetailExaminable
{
    [Serializable, NetSerializable]
    public sealed class FlavorPanelEuiState : EuiStateBase
    {
        public NetEntity TargetNetEntity;
        public string? Pose;
        public string? Flavor;
        public ERPStatus Status;
        public string? Name;
    }
}
