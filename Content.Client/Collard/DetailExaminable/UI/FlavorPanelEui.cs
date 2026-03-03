using Content.Client.Eui;
using Content.Shared.Collard.DetailExaminable;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client.Collard.DetailExaminable.UI
{
    [UsedImplicitly]
    public sealed class FlavorPanelEui : BaseEui
    {
        private readonly FlavorPanel _window;

        public FlavorPanelEui()
        {
            IoCManager.InjectDependencies(this);
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
