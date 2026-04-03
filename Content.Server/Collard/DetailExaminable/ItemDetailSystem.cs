using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.Utility;
using Robust.Shared.Player;
using Content.Server.Administration;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Ghost;
using Content.Shared.Hands.Components;
using YamlDotNet.Core;

namespace Content.Server.Collard.DetailExaminable
{
    public sealed class ItemDetailSystem : EntitySystem
    {
        [Dependency] private readonly ExamineSystemShared _examineSystem = default!;
        [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ItemDetailComponent, GetVerbsEvent<ExamineVerb>>(OnGetExamineVerbs);
            SubscribeLocalEvent<ItemDetailComponent, ExaminedEvent>(HandleExamined);
        }
        private void OnGetExamineVerbs(EntityUid uid, ItemDetailComponent component, GetVerbsEvent<ExamineVerb> args)
        {
            if (Identity.Name(args.Target, EntityManager) != MetaData(args.Target).EntityName)
                return;

            if (!TryComp(args.User, out ActorComponent? actor))
                return;

            var player = actor.PlayerSession;
            var detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);
            var editDetailVerb = new ExamineVerb()
            {
                Act = () =>
                {
                    if (!HasComp<GhostComponent>(args.User) || HasComp<HandsComponent>(args.User))
                        _quickDialog.OpenDialog(player,
                                Loc.GetString("item-detail-dialog-name"),
                                Loc.GetString("item-detail-dialog-field"),
                                (string newDesc) =>
                                {
                                    if (string.IsNullOrWhiteSpace(newDesc) || string.IsNullOrEmpty(newDesc) || newDesc == string.Empty)
                                    {
                                        component.Content = Loc.GetString("item-detail-content-none");
                                        return;
                                    }
                                    if (newDesc.Length > 128)
                                    {
                                        _popupSystem.PopupEntity(Loc.GetString("item-detail-popup-long"), uid);
                                        return;
                                    }
                                    _adminLogger.Add(LogType.Action,
                                        LogImpact.Low,
                                        $"{ToPrettyString(args.User):user} redescribed {ToPrettyString(uid):tool} from \"{component.Content}\" to \"{newDesc}\"");
                                    component.Content = newDesc;
                                });
                },
                Text = Loc.GetString("item-detail-verb-edit"),
                Category = VerbCategory.Examine,
                Disabled = !detailsRange,
                Message = detailsRange ? null : Loc.GetString("detail-examinable-verb-disabled"),
                Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/insert.svg.192dpi.png"))
            };

            args.Verbs.Add(editDetailVerb);
        }

        private void HandleExamined(EntityUid examinedUid, ItemDetailComponent component, ExaminedEvent args)
        {
            using (args.PushGroup(nameof(ItemDetailComponent)))
            {
                args.PushText(component.Content);
            }
        }
    }
}
