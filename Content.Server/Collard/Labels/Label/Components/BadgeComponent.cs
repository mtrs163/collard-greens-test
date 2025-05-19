using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Collard.Labels.Components
{
    /// <summary>
    ///     This component allows you to attach and remove a piece of paper to an entity.
    /// </summary>
    [RegisterComponent]
    public sealed partial class BadgeComponent : Component
    {
        [DataField("badgeSlot")]
        public ItemSlot BadgeSlot = new();
    }
}
