using Robust.Shared.Console;
using Robust.Shared.Player;
using Content.Shared.Administration;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;
using Content.Server.Chat.Systems;
using Content.Shared.Chat;

namespace Content.Server.Collard.Commands;

[AnyCommand]
public sealed class CheckCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    public override string Command => "check";
    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        var requirement = 10;
        var maxVal = 20;
        var check = args[0];
        if (args.Length != 1 && args.Length != 3)
        {
            shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
            shell.WriteLine(Help);
            return;
        }
        else if (args.Length == 3)
        {
            if (!int.TryParse(args[1], out var val))
            {
                shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
                shell.WriteLine(Help);
                return;
            }
            if (!int.TryParse(args[2], out var req))
            {
                shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
                shell.WriteLine(Help);
                return;
            }
            requirement = req;
            maxVal = val;
        }

        if (maxVal <= 1)
        {
            shell.WriteError(Loc.GetString("cmd-check-no-negative"));
            shell.WriteLine(Help);
            return;
        }
        if (requirement <= 1 || requirement > maxVal)
        {
            shell.WriteError(Loc.GetString("cmd-check-bad-requirement"));
            shell.WriteLine(Help);
            return;
        }

        var roll = _random.Next(1, maxVal + 1);
        shell.WriteLine(Loc.GetString("cmd-check-result", ("value", roll)));

        if (player is null) return;
        if (!TryParseUid(player, shell, _entManager, out var entityUid))
            return;
        if (entityUid is null) return;

        if (roll == maxVal)
        {
            var message = Loc.GetString("cmd-check-popup-success-critical",
                                        ("value", roll),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
        else if (roll >= requirement)
        {
            var message = Loc.GetString("cmd-check-popup-success",
                                        ("value", roll),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
        else if (roll > 1)
        {
            var message = Loc.GetString("cmd-check-popup-failure",
                                        ("value", roll),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
        else
        {
            var message = Loc.GetString("cmd-check-popup-failure-critical",
                                        ("value", roll),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
    }
    private bool TryParseUid(ICommonSession session, IConsoleShell shell,
        IEntityManager entMan, [NotNullWhen(true)] out EntityUid? entityUid)
    {
        if (session.AttachedEntity.HasValue)
        {
            entityUid = session.AttachedEntity.Value;
            return true;
        }
        if (session == null)
            shell.WriteError("Can't find username: " + session);
        else
            shell.WriteError(session + " does not have an entity.");

        entityUid = EntityUid.Invalid;
        return false;
    }
}

