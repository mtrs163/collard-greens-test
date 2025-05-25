using Robust.Shared.Configuration;

namespace Content.Shared.Collard.CCVars;

[CVarDefs]
public sealed class CCVars
{
    /// <summary>
    /// Send station goal on round start or not.
    /// </summary>
    public static readonly CVarDef<bool> StationGoal =
        CVarDef.Create("game.station_goal", true, CVar.SERVERONLY);
}
