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

        _menu.OnCookingStarted += (program) =>
        {
            SendPredictedMessage(new ClamshellGrillPlatenCloseMessage(program));
        };

        _menu.OnOpenPlaten += (error, silent) =>
        {
            SendPredictedMessage(new ClamshellGrillPlatenOpenMessage(error, silent));
        };

        _menu.StopSounds += () => SendPredictedMessage(new ClamshellGrillStopSoundsMessage());

        _menu.OnPopulatePrograms += PopulatePrograms;
        _menu.OnUpdateTime += UpdateTime;
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
        if (grill.CurrentState is not GrillState.Cooking)
        {
            _menu?.UpdateTimer(TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time), (float)TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time).TotalSeconds);
        }
        else _menu?.UpdateTimer(grill.OperationEndTime.Subtract(_timing.CurTime), (float)TimeSpan.FromSeconds(grill.SelectedProgram.Value.Time).TotalSeconds);
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

