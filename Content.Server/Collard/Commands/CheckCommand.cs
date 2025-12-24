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
using Content.Shared.Objectives.Components;
using Robust.Shared.Utility;

namespace Content.Server.Collard.Commands;

[AnyCommand]
public sealed class RollCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
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
            _popup.PopupEntity(Loc.GetString("cmd-check-popup-success-critical",
                                        ("value", roll),
                                        ("user", Identity.Entity(entityUid.Value, _entManager)),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement)),
                                        entityUid.Value,
                                        Shared.Popups.PopupType.MediumCaution);
        }
        else if (roll >= requirement)
        {
            _popup.PopupEntity(Loc.GetString("cmd-check-popup-success",
                                        ("value", roll),
                                        ("user", Identity.Entity(entityUid.Value, _entManager)),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement)),
                                        entityUid.Value,
                                        Shared.Popups.PopupType.Medium);
        }
        else if (roll > 1)
        {
            _popup.PopupEntity(Loc.GetString("cmd-check-popup-failure",
                                        ("value", roll),
                                        ("user", Identity.Entity(entityUid.Value, _entManager)),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement)),
                                        entityUid.Value,
                                        Shared.Popups.PopupType.Medium);
        }
        else
            _popup.PopupEntity(Loc.GetString("cmd-check-popup-failure-critical",
                                        ("value", roll),
                                        ("user", Identity.Entity(entityUid.Value, _entManager)),
                                        ("maxval", maxVal),
                                        ("checkname", check),
                                        ("requirement", requirement)),
                                        entityUid.Value,
                                        Shared.Popups.PopupType.MediumCaution);
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

