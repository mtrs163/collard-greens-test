using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Collard.Humanoid;

public enum ERPStatus : byte
{
    //No,
    Ask,
    Yes,
}

/// <summary>
///     Raised when entity has changed their erpstatus.
/// </summary>
public record struct ERPStatusChangedEvent(ERPStatus OldERPStatus, ERPStatus NewERPStatus);

