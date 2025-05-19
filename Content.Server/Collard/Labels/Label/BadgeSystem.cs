using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Labels;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Paper;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Content.Server.Collard.Labels.Components;
using Robust.Shared.Utility;

namespace Content.Server.Collard.Labels
{
    /// <summary>
    /// A system that lets players see the contents of a label on an object.
    /// </summary>
    [UsedImplicitly]
    public sealed class BadgeSystem : EntitySystem
    {
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

        public const string ContainerName = "badge_label";

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<BadgeComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<BadgeComponent, ComponentRemove>(OnComponentRemove);
            SubscribeLocalEvent<BadgeComponent, ExaminedEvent>(OnExamined);
        }

        /// <summary>
        /// Apply or remove a label on an entity.
        /// </summary>
        /// <param name="uid">EntityUid to change label on</param>
        /// <param name="text">intended label text (null to remove)</param>
        /// <param name="label">label component for resolve</param>
        /// <param name="metadata">metadata component for resolve</param>

        private void OnComponentInit(EntityUid uid, BadgeComponent component, ComponentInit args)
        {
            _itemSlotsSystem.AddItemSlot(uid, ContainerName, component.BadgeSlot);
        }

        private void OnComponentRemove(EntityUid uid, BadgeComponent component, ComponentRemove args)
        {
            _itemSlotsSystem.RemoveItemSlot(uid, component.BadgeSlot);
        }

        private void OnExamined(EntityUid uid, BadgeComponent comp, ExaminedEvent args)
        {
            if (comp.BadgeSlot.Item is not {Valid: true} item)
                return;

            using (args.PushGroup(nameof(BadgeComponent)))
            {
                if (!args.IsInDetailsRange)
                {
                    args.PushMarkup(Loc.GetString("comp-badge-has-badge-cant-read"));
                    return;
                }

                if (!EntityManager.TryGetComponent(item, out PaperComponent? paper))
                    // Assuming yaml has the correct entity whitelist, this should not happen.
                    return;

                if (string.IsNullOrWhiteSpace(paper.Content))
                {
                    args.PushMarkup(Loc.GetString("comp-badge-has-badge-blank"));
                    return;
                }

                args.PushMarkup(Loc.GetString("comp-badge-has-badge"));
                var text = paper.Content;
                args.PushMarkup(text.TrimEnd());
            }
        }
    }
}
