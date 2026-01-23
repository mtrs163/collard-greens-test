using Content.Shared.MassMedia.Systems;
using Content.Shared.Slippery;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Shared.Collard.Slippery;

[RegisterComponent, NetworkedComponent, Access(typeof(SlipperySystem))]
public sealed partial class TemporaryNoSlipComponent : Component
{
    [DataField]
    public TimeSpan EndTime = TimeSpan.Zero;
}
