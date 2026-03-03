using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private bool _allowFlavorText;

    private FlavorText.FlavorText? _flavorText;
    private TextEdit? _flavorTextEdit;
    private TextEdit? _poseTextEdit; // collard-DetailExaminableGlowup

    /// <summary>
    /// Refreshes the flavor text editor status.
    /// </summary>
    public void RefreshFlavorText()
    {
        if (_allowFlavorText)
        {
            if (_flavorText != null)
                return;

            _flavorText = new FlavorText.FlavorText();
            TabContainer.AddChild(_flavorText);
            TabContainer.SetTabTitle(TabContainer.ChildCount - 1, Loc.GetString("humanoid-profile-editor-flavortext-tab"));
            _flavorTextEdit = _flavorText.CFlavorTextInput;
            _poseTextEdit = _flavorText.CRoundstartPoseInput; // collard-DetailExaminableGlowup

            _flavorText.OnFlavorTextChanged += OnFlavorTextChange;
            _flavorText.OnRoundstartPoseTextChanged += OnRoundstartPoseTextChange; // collard-DetailExaminableGlowup
        }
        else
        {
            if (_flavorText == null)
                return;

            TabContainer.RemoveChild(_flavorText);
            _flavorText.OnFlavorTextChanged -= OnFlavorTextChange;
            _flavorText.OnRoundstartPoseTextChanged += OnRoundstartPoseTextChange; // collard-DetailExaminableGlowup
            _flavorText.Dispose();
            _flavorTextEdit?.Dispose();
            _poseTextEdit?.Dispose(); // collard-DetailExaminableGlowup
            _flavorTextEdit = null;
            _poseTextEdit = null; // collard-DetailExaminableGlowup
            _flavorText = null;
        }
    }

    private void OnFlavorTextChange(string content)
    {
        if (Profile is null)
            return;

        Profile = Profile.WithFlavorText(content);
        SetDirty();
    }

    // collard-DetailExaminableGlowup-start
        private void OnRoundstartPoseTextChange(string content)
        {
            if (Profile is null)
                return;

            Profile = Profile.WithRoundstartPose(content);
            SetDirty();
        }
        // collard-DetailExaminableGlowup-end

    private void UpdateFlavorTextEdit()
    {
        if (_flavorTextEdit != null)
        {
            _flavorTextEdit.TextRope = new Rope.Leaf(Profile?.FlavorText ?? "");
        }
    }

    private void UpdatePoseTextEdit()
        {
            if (_poseTextEdit != null)
            {
                _poseTextEdit.TextRope = new Rope.Leaf(Profile?.Pose ?? "");
            }
        }
        // collard-DetailExaminableGlowup-end
}
