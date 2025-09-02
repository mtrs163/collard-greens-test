using System.Linq;
using System.Numerics;
using Robust.Shared.Console;
using Robust.Server.Player;
using Robust.Shared.Player;
using Content.Shared.Administration;
using Robust.Shared.Random;
using Content.Server.Popups;
using Content.Shared.IdentityManagement;
using Content.Server.Access.Systems;
using System.Diagnostics.CodeAnalysis;

namespace Content.Server.Collard.Commands;

[AnyCommand]
sealed class RollCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    public override string Command => "roll";
    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (args.Length != 1)
        {
            shell.WriteLine(Loc.GetString("shell-need-exactly-one-argument"));
            shell.WriteLine(Help);
            return;
        }
        if (!int.TryParse(args[0], out var val))
        {
            shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
            shell.WriteLine(Help);
            return;
        }
        if (val <= 1)
        {
            shell.WriteError(Loc.GetString("cmd-roll-no-negative"));
            shell.WriteLine(Help);
            return;
        }
        var roll = _random.Next(1, val + 1);
        shell.WriteLine(Loc.GetString("cmd-roll-result", ("value", roll)));

        if (player is null) return;

        if (!TryParseUid(player, shell, _entManager, out var entityUid))
            return;

        if (entityUid is null) return;
        _popup.PopupEntity(Loc.GetString("cmd-roll-popup",
                                        ("value", roll),
                                        ("user", Identity.Entity(entityUid.Value, _entManager)),
                                        ("maxval", val)),
                                        entityUid.Value);
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

