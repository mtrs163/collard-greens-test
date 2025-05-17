using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server.Collard.Chat.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class SetPlayerJoinCommand : IConsoleCommand
{
    public string Command => "setplayerjoin";
    public string Description => Loc.GetString("set-playerjoin-command-description");
    public string Help => Loc.GetString("set-playerjoin-command-help");
    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var cfg = IoCManager.Resolve<IConfigurationManager>();

        if (args.Length > 1)
        {
            shell.WriteError(Loc.GetString("set-playerjoin-command-too-many-arguments-error"));
            return;
        }

        var playerjoin = cfg.GetCVar(CCVars.ChatEnablePlayerJoinNotification);

        if (args.Length == 0)
        {
            playerjoin = !playerjoin;
        }

        if (args.Length == 1 && !bool.TryParse(args[0], out playerjoin))
        {
            shell.WriteError(Loc.GetString("set-playerjoin-command-invalid-argument-error"));
            return;
        }

        cfg.SetCVar(CCVars.ChatEnablePlayerJoinNotification, playerjoin);

        shell.WriteLine(Loc.GetString(playerjoin ? "set-playerjoin-command-playerjoin-enabled" : "set-playerjoin-command-playerjoin-disabled"));
    }
}
