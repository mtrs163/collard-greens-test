using Content.Shared.Security.Components;
using JetBrains.Annotations;
using Content.Client.Security.Ui;
using Content.Client.Collard.Grill.UI;
using Content.Shared.Collard.Grill;

namespace Content.Client.Collard.Grill.UI;

[UsedImplicitly]
public sealed class ClamshellGrillBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private ClamshellGrillMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = new(Owner, EntMan);

        _menu.OnProgramConfigurationComplete += (name, time, temp) =>
        {
            SendPredictedMessage(new ClamshellGrillProgramCreatedMessage(name, time, temp));
        };

        _menu.OnPopulatePrograms += PopulatePrograms;
        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    public void PopulatePrograms()
    {
        var grill = EntMan.GetComponent<ClamshellGrillComponent>(Owner);
        _menu?.Populate(grill.SavedPrograms);
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

