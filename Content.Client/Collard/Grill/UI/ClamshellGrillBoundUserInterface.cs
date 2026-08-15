using Content.Shared.Security.Components;
using JetBrains.Annotations;
using Content.Client.Security.Ui;
using Content.Client.Collard.Grill.UI;
using Content.Shared.Collard.Grill;
using Robust.Client.Timing;

namespace Content.Client.Collard.Grill.UI;

[UsedImplicitly]
public sealed partial class ClamshellGrillBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{

    [Dependency] private IClientGameTiming _timing = default!;
    private ClamshellGrillMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = new(Owner, EntMan);

        _menu.OnProgramConfigurationComplete += (name, time, temp) =>
        {
            SendPredictedMessage(new ClamshellGrillProgramCreatedMessage(name, time, temp));
        };

        _menu.OnDeleteProgram += (program) =>
        {
            SendPredictedMessage(new ClamshellGrillProgramDeletedMessage(program));
        };

        _menu.OnCookingStarted += (program) =>
        {
            SendPredictedMessage(new ClamshellGrillPlatenCloseMessage(program));
        };

        _menu.OnCookingCancelled += CancelCooking;

        _menu.OnOpenPlaten += (error, silent) =>
        {
            SendPredictedMessage(new ClamshellGrillPlatenOpenMessage(error, silent));
        };

        _menu.StopSounds += () => SendPredictedMessage(new ClamshellGrillStopSoundsMessage());

        _menu.OnMainMenuOpened += OpenMainMenu;
        _menu.OnEditorOpened += OpenEditor;
        _menu.OnSelectorOpened += OpenSelector;
        _menu.OnPopulatePrograms += PopulatePrograms;
        _menu.OnOpen += SelectTab;
        _menu.OnUpdateTime += UpdateTime;
        _menu.OnUpdateOverlay += UpdateOverlay;
        _menu.OnUpdatePower += UpdatePower;
        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    public void PopulatePrograms()
    {
        var grill = EntMan.GetComponent<ClamshellGrillComponent>(Owner);
        _menu?.Populate(grill.SavedPrograms);
    }

    public void UpdateTime()
    {
        var grill = EntMan.GetComponent<ClamshellGrillComponent>(Owner);
        if (grill.SelectedProgram is null) return;
        if (grill.CurrentState is not GrillState.Cooking && grill.CurrentState is not GrillState.Standby)
        {
            _menu?.UpdateTimer(TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time), (float)TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time).TotalSeconds, grill.StartTime);
        }
        else _menu?.UpdateTimer(grill.OperationEndTime.Subtract(_timing.CurTime), (float)TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time).TotalSeconds, grill.StartTime);
        if (grill.OperationEndTime < _timing.CurTime && grill.CurrentState == GrillState.Cooking)
            _menu?.EnterProgramSelector();
    }

    public void UpdateOverlay()
    {
        var grill = EntMan.GetComponent<ClamshellGrillComponent>(Owner);
        if (grill.CurrentState == GrillState.Closing || grill.CurrentState == GrillState.Opening || grill.CurrentState == GrillState.OpeningError)
        {
            _menu?.UpdateOverlay(true);
        }
        else _menu?.UpdateOverlay(false);
    }

    public void UpdatePower()
    {
        var grill = EntMan.GetComponent<ClamshellGrillComponent>(Owner);
        if (grill.CurrentState == GrillState.Unpowered)
        {
            _menu?.UpdatePower(false);
        }
        else _menu?.UpdatePower(true);
    }

    public void SelectTab()
    {
        var grill = EntMan.GetComponent<ClamshellGrillComponent>(Owner);
        if (grill.CurrentState == GrillState.SelectingProgram || grill.CurrentState == GrillState.Opening)
        {
            _menu?.EnterProgramSelector();
            return;
        }
        if (grill.NextState == GrillState.Cooking || grill.CurrentState == GrillState.Cooking)
        {
            _menu?.EnterCooking();
            return;
        }
        if (grill.NextState == GrillState.Standby || grill.CurrentState == GrillState.Standby)
        {
            _menu?.EnterStandbyWindow();
            return;
        }
        if (grill.CurrentState == GrillState.MainMenu)
        {
            _menu?.EnterMainMenu();
            return;
        }
        if (grill.CurrentState == GrillState.EditingProgram)
        {
            _menu?.EnterProgramEditor();
            return;
        }
        if (grill.CurrentState == GrillState.Cancelling)
        {
            _menu?.EnterCancellation();
            return;
        }
    }

    public void OpenMainMenu()
    {
        SendPredictedMessage(new ClamshellGrillStateChange(GrillState.MainMenu));
    }

    public void OpenEditor()
    {
        SendPredictedMessage(new ClamshellGrillStateChange(GrillState.EditingProgram));
    }

    public void OpenSelector()
    {
        SendPredictedMessage(new ClamshellGrillStateChange(GrillState.SelectingProgram));
    }

    public void CancelCooking()
    {
        SendPredictedMessage(new ClamshellGrillStateChange(GrillState.Cancelling));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _menu?.Orphan();
        _menu = null;
    }
}

