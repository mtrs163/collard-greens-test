using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.Utility;
// collard-DetailExaminableGlowup-start
using Content.Server.Actions;
using Content.Server.EUI;
using Content.Server.Administration;
using Content.Shared.Collard.DetailExaminable;
using Content.Server.Collard.DetailExaminable;
using Robust.Shared.Player;
using Content.Server.Popups;
// collard-DetailExaminableGlowup-end

namespace Content.Server.DetailExaminable;

public sealed partial class DetailExaminableSystem : EntitySystem
{
    [Dependency] private ExamineSystemShared _examine = default!;
    // collard-DetailExaminableGlowup-start
    [Dependency] private EuiManager _euis = default!;
    [Dependency] private ActionsSystem _actions = default!;
    [Dependency] private QuickDialogSystem _quickDialog = default!;
    [Dependency] private PopupSystem _popup = default!;
    // collard-DetailExaminableGlowup-end

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>(OnGetExamineVerbs);
        // collard-DetailExaminableGlowup-start
        SubscribeLocalEvent<DetailExaminableComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<DetailExaminableComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<DetailExaminableComponent, ChangePoseActionEvent>(OnChangePoseAction);
        SubscribeLocalEvent<DetailExaminableComponent, ChangeFlavorActionEvent>(OnChangeFlavorAction);
        // collard-DetailExaminableGlowup-end
    }

    // collard-DetailExaminableGlowup-start
    private void OnMapInit(EntityUid uid, DetailExaminableComponent component, MapInitEvent args)
    {
        // try to add posing action when posing comp added
        component.PoseActionEntity = null;
        _actions.AddAction(uid, ref component.PoseActionEntity, component.ChangePoseAction);
        component.FlavorActionEntity = null;
        _actions.AddAction(uid, ref component.FlavorActionEntity, component.ChangeFlavorAction);
    }

    private void OnShutdown(EntityUid uid, DetailExaminableComponent component, ComponentShutdown args)
    {
        // remove actions when component removed
        if (component.PoseActionEntity != null)
            _actions.RemoveAction(uid, component.PoseActionEntity);
        if (component.FlavorActionEntity != null)
            _actions.RemoveAction(uid, component.FlavorActionEntity);
    }
    // collard-DetailExaminableGlowup-end

    private void OnGetExamineVerbs(Entity<DetailExaminableComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
    {
        if (Identity.Name(args.Target, EntityManager) != MetaData(args.Target).EntityName)
            return;

        var detailsRange = _examine.IsInDetailsRange(args.User, ent);

        var user = args.User;

        // collard-DetailExaminableGlowup-start
        if (!TryComp(args.User, out ActorComponent? actor))
            return;
        var player = actor.PlayerSession;
        var target = GetNetEntity(args.Target);
        // collard-DetailExaminableGlowup-end

        var verb = new ExamineVerb()
        {
            Act = () =>
            {
                _euis.OpenEui(new FlavorPanelEui(target), player); // collard-DetailExaminableGlowup
            },
            Text = Loc.GetString("detail-examinable-verb-text"),
            Category = VerbCategory.Examine,
            Disabled = !detailsRange,
            Message = detailsRange ? null : Loc.GetString("detail-examinable-verb-disabled"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"))
        };

        args.Verbs.Add(verb);
    }

    // collard-DetailExaminableGlowup-start
    private void OnChangePoseAction(EntityUid uid, DetailExaminableComponent component, ChangePoseActionEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp(args.Performer, out ActorComponent? actor))
            return;

        var player = actor.PlayerSession;

        _quickDialog.OpenDialog(player,
                                Loc.GetString("posing-dialog-name"),
                                Loc.GetString("posing-dialog-field-pose"),
                                (LongString newPose) =>
            {
                if (newPose.String == string.Empty)
                {
                    component.PoseContent = Loc.GetString("posing-content-none");
                    _popup.PopupEntity(Loc.GetString("posing-pose-erased",
                                        ("user", Identity.Name(args.Performer, EntityManager))),
                                        args.Performer,
                                        Shared.Popups.PopupType.Medium);
                }
                else
                {
                    component.PoseContent = newPose.String;
                    _popup.PopupEntity(Loc.GetString("posing-pose-changed",
                                        ("pose", newPose.String),
                                        ("user", Identity.Name(args.Performer, EntityManager))),
                                        args.Performer,
                                        Shared.Popups.PopupType.Medium);
                }
            });
        args.Handled = true;
    }

    private void OnChangeFlavorAction(EntityUid uid, DetailExaminableComponent component, ChangeFlavorActionEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp(args.Performer, out ActorComponent? actor))
            return;

        var player = actor.PlayerSession;

        _quickDialog.OpenDialog(player,
                                Loc.GetString("flavor-dialog-name"),
                                Loc.GetString("flavor-dialog-field-flavor"),
                                (LongString newFlavor) =>
            {
                if (newFlavor.String == string.Empty)
                {
                    component.Content = Loc.GetString("flavor-content-none");
                }
                else
                {
                    component.Content = newFlavor.String;
                }
            });
        args.Handled = true;
    }
    // collard-DetailExaminableGlowup-end
}
