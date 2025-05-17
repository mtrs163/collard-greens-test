using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Content.Shared.Chat;
using Content.Server.Chat.Managers;
using Content.Server.Administration;
using Content.Server.Administration.Managers;
using Robust.Shared;

namespace Content.Server.Collard.Commands;

[AnyCommand()]
public sealed class GhostRespawnCommand : IConsoleCommand
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;

    public string Command => "ghostrespawn";
    public string Description => "Allows the player to return to the lobby if they've been dead long enough, allowing re-entering the round AS ANOTHER CHARACTER.";
    public string Help => $"{Command}";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (!_configurationManager.GetCVar(CCVars.RespawnEnabled))
        {
            shell.WriteLine("Respawning is disabled, ask an admin to respawn you.");
            return;
        }

        if (shell.Player is null)
        {
            shell.WriteLine("You cannot run this from the console!");
            return;
        }

        if (shell.Player.AttachedEntity is null)
        {
            shell.WriteLine("You cannot run this in the lobby, or without an entity.");
            return;
        }

        if (!_entityManager.TryGetComponent<GhostComponent>(shell.Player.AttachedEntity, out var ghost))
        {
            shell.WriteLine("You are not a ghost.");
            return;
        }

        var mindSystem = _entityManager.EntitySysManager.GetEntitySystem<MindSystem>();
        if (!mindSystem.TryGetMind(shell.Player, out _, out _))
        {
            shell.WriteLine("You have no mind.");
            return;
        }
        var time = (_gameTiming.CurTime - ghost.TimeOfDeath);
        var respawnTime = _configurationManager.GetCVar(CCVars.RespawnTime);

        if (respawnTime > time.TotalSeconds)
        {
            shell.WriteLine($"You haven't been dead long enough. You have been dead {time.TotalSeconds} seconds of the required {respawnTime}.");
            return;
        }

        var gameTicker = _entityManager.EntitySysManager.GetEntitySystem<GameTicker>();
        gameTicker.Respawn(shell.Player);
        var plrname = shell.Player;
        _chat.SendAdminAnnouncement(Loc.GetString("ghost-respawn-admin-notification", ("plrname", plrname)));
        //_audioSystem.PlayGlobal("/Audio/Machines/high_tech_confirm.ogg", Filter.Empty().AddPlayers(_adminManager.ActiveAdmins), false, AudioParams.Default.WithVolume(-8f));
    }
}
