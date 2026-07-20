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

        _menu.OnConfigurationComplete += (name, time, temp) =>
        {
            SendPredictedMessage(new ClamshellGrillStartedMessage(name, time, temp));
            Close();
        };

        _menu.OnClose += Close;
        _menu.OpenCentered();
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

