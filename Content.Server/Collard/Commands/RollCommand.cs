using Robust.Shared.Console;
using Robust.Shared.Player;
using Content.Shared.Administration;
using Robust.Shared.Random;
using System.Diagnostics.CodeAnalysis;
using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using System.Text;
using Content.Server.Access.Components;
using System.Linq;

namespace Content.Server.Collard.Commands;

[AnyCommand]
public sealed class RollCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    public override string Command => "roll";

    public override string Help => LocalizationManager.GetString($"cmd-{Command}-help", ("command", Command));

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (args.Length != 1)
        {
            shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
            shell.WriteLine(Help);
            return;
        }

        if (!args[0].ToLower().Contains('d'))
        {
            shell.WriteError(Loc.GetString("cmd-roll-bad-argument"));
            shell.WriteLine(Help);
            return;
        }

        var dice = args[0].ToLower();

        char[] separators = [' ', 'd'];
        var splitDice = dice.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        if (splitDice.Length != 2)
        {
            shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
            shell.WriteLine(Help);
            return;
        }
        if (!int.TryParse(splitDice[0], out var diceAmount))
        {
            shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
            shell.WriteLine(Help);
            return;
        }
        if (!int.TryParse(splitDice[1], out var diceValue))
        {
            shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
            shell.WriteLine(Help);
            return;
        }
        if (diceAmount < 1 || diceValue < 2)
        {
            shell.WriteError(Loc.GetString("cmd-roll-bad-argument"));
            shell.WriteLine(Help);
            return;
        }
        if (diceAmount > 100 || diceValue > 100)
        {
            shell.WriteError(Loc.GetString("cmd-roll-toomuch"));
            shell.WriteLine(Help);
            return;
        }

        var rollResults = new List<int>();
        var rollSum = 0;
        for (var i = 0; i < diceAmount; i++)
        {
            rollResults.Add(_random.Next(1, diceValue + 1));
            rollSum += rollResults[i];
        }
        var rollsString = string.Join(",", rollResults.ToArray());

        if (player is null) return;
        if (!TryParseUid(player, shell, _entManager, out var entityUid))
            return;
        if (entityUid is null) return;

        var message = Loc.GetString("cmd-roll-rolled", ("dice", args[0]), ("rolls", rollsString), ("sum", rollSum));
        _chat.TrySendInGameICMessage(entityUid.Value, message, InGameICChatType.Emote, false);

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
        if (args.Length != 1)
            return CompletionResult.Empty;

        return CompletionResult.FromHint(
            Loc.GetString("cmd-roll-completion"));
    }
}

