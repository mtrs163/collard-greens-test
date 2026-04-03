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

    public override string Help => LocalizationManager.GetString($"cmd-{Command}-help", ("command", Command));

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        var requirement = 10;
        if (args.Length < 1 || args.Length > 3)
        {
            shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
            shell.WriteLine(Help);
            return;
        }
        else if (args.Length >= 2)
        {
            if (!int.TryParse(args[1], out var req))
            {
                shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
                shell.WriteLine(Help);
                return;
            }
            requirement = req;
        }
        if (requirement < 0 || requirement > 20)
        {
            shell.WriteError(Loc.GetString("cmd-check-bad-requirement"));
            shell.WriteLine(Help);
            return;
        }
        var check = args[0];

        var roll1 = _random.Next(1, 21);
        var roll2 = _random.Next(1, 21);
        var roll = roll1;
        var advantageStatus = "none";
        if (args.Length == 3)
        {
            switch (args[2])
            {
                case "advantage":
                    if (roll2 >= roll1) roll = roll2;
                    else roll = roll1;
                    advantageStatus = "adv";
                    break;
                case "disadvantage":
                    if (roll2 <= roll1) roll = roll2;
                    else roll = roll1;
                    advantageStatus = "dis";
                    break;
                case "adv":
                    if (roll2 >= roll1) roll = roll2;
                    else roll = roll1;
                    advantageStatus = "adv";
                    break;
                case "dis":
                    if (roll2 <= roll1) roll = roll2;
                    else roll = roll1;
                    advantageStatus = "dis";
                    break;
                default:
                    roll = roll1;
                    advantageStatus = "none";
                    break;
            }
        }

        shell.WriteLine(Loc.GetString("cmd-check-result", ("roll1", roll1), ("roll2", roll2), ("value", roll)));

        if (player is null) return;
        if (!TryParseUid(player, shell, _entManager, out var entityUid))
            return;
        if (entityUid is null) return;

        if (roll == 20)
        {
            var message = Loc.GetString("cmd-check-popup-success-critical",
                                        ("value", roll),
                                        ("checkname", check),
                                        ("requirement", requirement),
                                        ("advantageStatus", Loc.GetString($"cmd-check-advantage-{advantageStatus}")));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
        else if (roll == 1)
        {
            var message = Loc.GetString("cmd-check-popup-failure-critical",
                                        ("value", roll),
                                        ("checkname", check),
                                        ("requirement", requirement),
                                        ("advantageStatus", Loc.GetString($"cmd-check-advantage-{advantageStatus}")));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
        else if (roll >= requirement)
        {
            var message = Loc.GetString("cmd-check-popup-success",
                                        ("value", roll),
                                        ("checkname", check),
                                        ("requirement", requirement),
                                        ("advantageStatus", Loc.GetString($"cmd-check-advantage-{advantageStatus}")));
            _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);
        }
        else
        {
            var message = Loc.GetString("cmd-check-popup-failure",
                                        ("value", roll),
                                        ("checkname", check),
                                        ("requirement", requirement),
                                        ("advantageStatus", Loc.GetString($"cmd-check-advantage-{advantageStatus}")));
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

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHint(
                Loc.GetString("cmd-check-checkname-completion"));
        }

        if (args.Length == 2)
        {
            return CompletionResult.FromHint(
                Loc.GetString("cmd-check-dc-completion"));
        }

        if (args.Length != 3)
            return CompletionResult.Empty;

        return CompletionResult.FromHint(
            Loc.GetString("cmd-check-advantage-completion"));
    }
}

