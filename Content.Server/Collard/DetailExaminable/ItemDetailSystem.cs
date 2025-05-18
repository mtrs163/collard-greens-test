using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.Utility;
using Content.Server.EUI;
using Content.Server.Collard.DetailExaminable;
using Content.Shared.Collard.DetailExaminable;
using Robust.Shared.Player;
using Content.Server.Actions;
using Content.Server.Administration;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Shared.Database;

namespace Content.Server.Collard.DetailExaminable
{
    public sealed class ItemDetailSystem : EntitySystem
    {
        [Dependency] private readonly ExamineSystemShared _examineSystem = default!;
        [Dependency] private readonly ActionsSystem _actions = default!;
        [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ItemDetailComponent, GetVerbsEvent<ExamineVerb>>(OnGetExamineVerbs);
        }

        private void OnGetExamineVerbs(EntityUid uid, ItemDetailComponent component, GetVerbsEvent<ExamineVerb> args)
        {
            if (Identity.Name(args.Target, EntityManager) != MetaData(args.Target).EntityName)
                return;

            if (!EntityManager.TryGetComponent(args.User, out ActorComponent? actor))
                return;

            var player = actor.PlayerSession;
            var detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);
            var checkDetailVerb = new ExamineVerb()
            {
                Act = () =>
                {
                    var markup = new FormattedMessage();
                    markup.AddMarkupOrThrow(component.Content);
                    _examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
                },
                Text = Loc.GetString("item-detail-verb-check"),
                Category = VerbCategory.Examine,
                Disabled = !detailsRange,
                Message = detailsRange ? null : Loc.GetString("detail-examinable-verb-disabled"),
                Icon = new SpriteSpecifier.Texture(new ("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"))
            };

            var editDetailVerb = new ExamineVerb()
            {
                Act = () =>
                {
                    _quickDialog.OpenDialog(player,
                                Loc.GetString("item-detail-dialog-name"),
                                Loc.GetString("item-detail-dialog-field"),
                                (string newDesc) =>
                                {
                                    if (string.IsNullOrWhiteSpace(newDesc) || string.IsNullOrEmpty(newDesc))
                                    {
                                        component.Content = Loc.GetString("item-detail-content-none");
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

            args.Verbs.Add(checkDetailVerb);
            args.Verbs.Add(editDetailVerb);
        }
    }
}
