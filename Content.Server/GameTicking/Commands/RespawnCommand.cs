using System.Linq;
using Content.Server.Administration;
using Content.Server.Mind;
using Content.Shared.Players;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Network;
using Content.Server.Chat.Managers; // collard-Admin1984

namespace Content.Server.GameTicking.Commands
{
    sealed class RespawnCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly IPlayerManager _player = default!;
        [Dependency] private readonly IPlayerLocator _locator = default!;
        [Dependency] private readonly GameTicker _gameTicker = default!;
        [Dependency] private readonly MindSystem _mind = default!;
        [Dependency] private readonly IChatManager _chat = default!; // collard-Admin1984

        public override string Command => "respawn";

        public override async void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player;
            if (args.Length > 1)
            {
                shell.WriteError(Loc.GetString("cmd-respawn-invalid-args"));
                return;
            }

            NetUserId userId;
            if (args.Length == 0)
            {
                if (player == null)
                {
                    shell.WriteError(Loc.GetString("cmd-respawn-no-player"));
                    return;
                }

                userId = player.UserId;
            }
            else
            {
                var located = await _locator.LookupIdByNameOrIdAsync(args[0]);

                if (located == null)
                {
                    shell.WriteError(Loc.GetString("cmd-respawn-unknown-player"));
                    return;
                }

                userId = located.UserId;
            }

            if (!_player.TryGetSessionById(userId, out var targetPlayer))
            {
                if (!_player.TryGetPlayerData(userId, out var data))
                {
                    shell.WriteError(Loc.GetString("cmd-respawn-unknown-player"));
                    return;
                }

                _mind.WipeMind(data.ContentData()?.Mind);
                shell.WriteError(Loc.GetString("cmd-respawn-player-not-online"));
                return;
            }

            _gameTicker.Respawn(targetPlayer);
            // collard-Admin1984-start
            if (shell.Player is null) return;
            if (targetPlayer is null) return;
            _chat.SendAdminAnnouncement(Loc.GetString("respawn-command-admin-notification", ("plrname", targetPlayer), ("adminname", shell.Player)));
            // collard-Admin1984-end
        }

      public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length != 1)
                return CompletionResult.Empty;

            var options = _player.Sessions.OrderBy(c => c.Name).Select(c => c.Name).ToArray();

            return CompletionResult.FromHintOptions(options, Loc.GetString("cmd-respawn-player-completion"));
        }
    }
}
