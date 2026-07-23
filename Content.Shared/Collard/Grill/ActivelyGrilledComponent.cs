namespace Content.Shared.Collard.Grill;

/// <summary>
/// Attached to an object that's actively being grilled
/// </summary>
[RegisterComponent]
public sealed partial class ActivelyGrilledComponent : Component
{
    /// <summary>
    /// The grill this entity is actively being grilled by.
    /// </summary>
    [DataField]
    public EntityUid? Grill;
}
