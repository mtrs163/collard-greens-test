using Content.Client.Eui;
using Content.Shared.Collard.DetailExaminable;
using Content.Shared.Eui;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.Player;
using Content.Client.CharacterInfo;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Systems.Character.Controls;
using Content.Client.UserInterface.Systems.Objectives.Controls;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;
using System.Linq;

namespace Content.Client.Collard.DetailExaminable.UI
{
    [UsedImplicitly]
    public sealed class FlavorPanelEui : BaseEui
    {
        [Dependency] private readonly IPlayerManager _player = default!;

        private readonly FlavorPanel _window;
        private IEntityManager _entManager;

        public FlavorPanelEui()
        {
            IoCManager.InjectDependencies(this);
            _entManager = IoCManager.Resolve<IEntityManager>();
            _window = new FlavorPanel();
            _window.OnClose += OnClosed;
        }

        private void OnClosed()
        {
            SendMessage(new CloseEuiMessage());
        }

        public override void Opened()
        {
            _window.OpenCentered();
        }

        public override void Closed()
        {
            base.Closed();
            _window.Close();
        }

        public override void HandleState(EuiStateBase state)
        {
            var flavorState = (FlavorPanelEuiState)state;
            _window.TargetEntityId = flavorState.TargetNetEntity;
            _window.SetCharacterName(flavorState.Name!);
            _window.SetFlavorText(flavorState.Flavor!);
            _window.SetPoseText(flavorState.Pose!);
            _window.SetERPStatus(flavorState.Status);
            _window.SetSprite();
        }
    }
}
