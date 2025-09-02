using Content.Server.GameTicking;
using Content.Server.Popups;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Content.Shared.Mind;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Server.Administration;
using Content.Shared.FixedPoint;

namespace Content.Server.Collard.Commands
{
    [AnyCommand]
    internal sealed class KysCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public string Command => "kys";

        public string Description => Loc.GetString("kys-command-description");

        public string Help => Loc.GetString("kys-command-help-text");

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length == 1)
            {
                var types = _prototypeManager.EnumeratePrototypes<DamageTypePrototype>()
                    .Select(p => new CompletionOption(p.ID));

                var groups = _prototypeManager.EnumeratePrototypes<DamageGroupPrototype>()
                    .Select(p => new CompletionOption(p.ID));

                return CompletionResult.FromHintOptions(types.Concat(groups).OrderBy(p => p.Value),
                    Loc.GetString("damage-command-arg-type"));
            }

            if (args.Length == 2)
            {
                return CompletionResult.FromHint(Loc.GetString("damage-command-arg-quantity"));
            }

            if (args.Length == 3)
            {
                // if type.Name is good enough for cvars, <bool> doesn't need localizing.
                return CompletionResult.FromHint(Loc.GetString("kys-command-arg-ingorearmor"));
            }

            return CompletionResult.Empty;
        }

        private delegate void Damage(EntityUid entity, bool ignoreResistances);

        private bool TryParseDamageArgs(
            IConsoleShell shell,
            string[] args,
            [NotNullWhen(true)] out Damage? func)
        {
            if (!float.TryParse(args[1], out var amount))
            {
                shell.WriteLine(Loc.GetString("damage-command-error-quantity", ("arg", args[1])));
                func = null;
                return false;
            }

            if (amount < 1)
            {
                shell.WriteLine(Loc.GetString("kys-command-error-quantity-low"));
                func = null;
                return false;
            }

            if (amount > 300)
            {
                shell.WriteLine(Loc.GetString("kys-command-error-quantity-high"));
                func = null;
                return false;
            }

            if (_prototypeManager.TryIndex<DamageGroupPrototype>(args[0], out var damageGroup))
            {
                func = (entity, ignoreResistances) =>
                {
                    var damage = new DamageSpecifier(damageGroup, amount);
                    _e.System<DamageableSystem>().TryChangeDamage(entity, damage, ignoreResistances);
                };

                return true;
            }
            // Fall back to DamageType

            if (_prototypeManager.TryIndex<DamageTypePrototype>(args[0], out var damageType))
            {
                func = (entity, ignoreResistances) =>
                {
                    var damage = new DamageSpecifier(damageType, amount);
                    _e.System<DamageableSystem>().TryChangeDamage(entity, damage, ignoreResistances);
                };
                return true;

            }

            shell.WriteLine(Loc.GetString("damage-command-error-type", ("arg", args[0])));
            func = null;
            return false;
        }

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                shell.WriteLine(Loc.GetString("damage-command-error-args"));
                return;
            }

            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (player.Status != SessionStatus.InGame || player.AttachedEntity == null)
                return;

            var minds = _e.System<SharedMindSystem>();

            // This check also proves mind not-null for at the end when the mob is ghosted.
            if (!minds.TryGetMind(player, out var mindId, out var mindComp) ||
                mindComp.OwnedEntity is not { Valid: true } victim)
            {
                shell.WriteLine(Loc.GetString("kys-command-no-mind"));
                return;
            }

            if (_e.HasComponent<AdminFrozenComponent>(victim))
            {
                var deniedMessage = Loc.GetString("kys-command-denied");
                shell.WriteLine(deniedMessage);
                _e.System<PopupSystem>()
                    .PopupEntity(deniedMessage, victim, victim);
                return;
            }

            if (!TryParseDamageArgs(shell, args, out var damageFunc))
                return;

            bool ignoreResistances;
            if (args.Length == 3)
            {
                if (!bool.TryParse(args[2], out ignoreResistances))
                {
                    shell.WriteLine(Loc.GetString("damage-command-error-bool", ("arg", args[2])));
                    return;
                }
            }
            else
            {
                ignoreResistances = false;
            }

            damageFunc(victim, ignoreResistances);

            if (ignoreResistances)
                shell.WriteLine(Loc.GetString("kys-command-finished-ignore", ("damage", args[1]), ("dmgtype", args[0])));
            else
                shell.WriteLine(Loc.GetString("kys-command-finished-noignore", ("damage", args[1]), ("dmgtype", args[0])));
        }
    }
}
