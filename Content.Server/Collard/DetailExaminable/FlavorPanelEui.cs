using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Shared.Collard.DetailExaminable;
using Content.Shared.Eui;
using JetBrains.Annotations;
using Content.Server.DetailExaminable;
using Content.Shared.Thief;
using Content.Shared.Collard.Humanoid;
using Robust.Shared.GameObjects;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;

namespace Content.Server.Collard.DetailExaminable
{
    [UsedImplicitly]
    public sealed class FlavorPanelEui : BaseEui
    {
        [Dependency] private readonly EntityManager _entityManager = default!;
        private readonly NetEntity _target;
        private readonly EntityUid _targetUid;
        private string _flavor;
        private string _name;
        private string _pose;
        private ERPStatus _status;

        public FlavorPanelEui(NetEntity entity)
        {
            _target = entity;
            IoCManager.InjectDependencies(this);
            _targetUid = _entityManager.GetEntity(entity);
            _flavor = GetFlavor();
            _pose = GetPose();
            _status = GetERPStatus();
            _name = Identity.Name(_targetUid, _entityManager);
        }

        private string GetFlavor()
        {
            if (!_entityManager.TryGetComponent<DetailExaminableComponent>(_targetUid, out var detailExaminable))
                return Loc.GetString("flavor-panel-flavor-none");
            if (detailExaminable is null)
                return Loc.GetString("flavor-panel-flavor-none");
            return detailExaminable.Content;
        }

        private string GetPose()
        {
            if (!_entityManager.TryGetComponent<DetailExaminableComponent>(_targetUid, out var detailExaminable))
                return Loc.GetString("posing-content-none");
            if (detailExaminable is null)
                return Loc.GetString("posing-content-none");
            return detailExaminable.PoseContent;
        }

        private ERPStatus GetERPStatus()
        {
            if (!_entityManager.TryGetComponent<HumanoidAppearanceComponent>(_targetUid, out var appearance))
                return ERPStatus.Ask;
            if (appearance is null)
                return ERPStatus.Ask;
            return appearance.ERPStatus;
        }

        public override void Opened()
        {
            base.Opened();
            StateDirty();
        }

        public override EuiStateBase GetNewState()
        {
            return new FlavorPanelEuiState
            {
                TargetNetEntity = _target,
                Pose = _pose,
                Flavor = _flavor,
                Status = _status,
                Name = _name,
            };
        }

    }
}
