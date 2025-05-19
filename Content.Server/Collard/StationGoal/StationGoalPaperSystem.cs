using Content.Server.Fax;
using Content.Server.Station.Systems;
using Content.Shared.Collard.CCVars;
using Content.Shared.Fax.Components;
using Content.Shared.GameTicking;
using Content.Shared.Collard.GameTicking;
using Content.Shared.Paper;
using Content.Shared.Random.Helpers;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Linq;
using Content.Shared.Dataset;

namespace Content.Server.Collard.StationGoal
{
    /// <summary>
    ///     System to spawn paper with station goal.
    /// </summary>
    public sealed class StationGoalPaperSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly FaxSystem _faxSystem = default!;
        [Dependency] private readonly StationSystem _station = default!;
        [Dependency] private readonly SharedGameTicker _ticker = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<RoundStartedEvent>(OnRoundStarted);
        }

        private void OnRoundStarted(RoundStartedEvent ev)
        {
            SendStationGoal();
        }

        public string ChooseRandomPrefixGoal(string stationName)
        {
            if (stationName.Length < 4)
                return "weh";
            var goalPool = "Generic";
            switch (stationName[..4])
            {
                case "NTAD":
                    goalPool = "Generic"; //"Administrative"; // placeholder
                    break;
                case "NTBS":
                    goalPool = "Business";
                    break;
                case "NTRN":
                    goalPool = "Research";
                    break;
                case "NTEX":
                    goalPool = "Experimental";
                    break;
                case "NTCO":
                    goalPool = "Commercial";
                    break;
                case "NTME":
                    goalPool = "Generic"; //"Medical"; // placeholder
                    break;
                case "NTDE":
                    goalPool = "Generic"; //"Defense"; // placeholder
                    break;
                case "NTTS":
                    goalPool = "Transport";
                    break;
                case "NTRT":
                    goalPool = "Transport";
                    break;
                default:
                    goalPool = "Generic";
                    break;
            }
            var availableGoals = _prototypeManager.EnumeratePrototypes<StationGoalPrototype>().ToList();
            ProtoId<LocalizedDatasetPrototype> stationGoal = "StationGoal" + goalPool;
            var goal = _random.Pick(_prototypeManager.Index(stationGoal).Values);
            return goal;
        }

        /// <summary>
        ///     Send a station goal to all faxes which are authorized to receive it.
        /// </summary>
        /// <returns>True if at least one fax received paper</returns>
        public bool SendStationGoal()
        {
            if (!_cfg.GetCVar(CCVars.StationGoal)) return false;
            var enumerator = EntityManager.EntityQueryEnumerator<FaxMachineComponent>();
            var wasSent = false;
            while (enumerator.MoveNext(out var uid, out var fax))
            {
                if (!fax.ReceiveStationGoal) continue;

                if (!TryComp<MetaDataComponent>(_station.GetOwningStation(uid), out var meta))
                    continue;

                var goal = ChooseRandomPrefixGoal(meta.EntityName);
                var stationShortName = "ERR12";
                if (meta.EntityName.Length < 4)
                {
                    stationShortName = "ERR12. Ошибка автоматической системы рассылки целей станции.";
                }
                else if (meta.EntityName[..4] == "NTDE" || meta.EntityName[..4] == "NTAD" || meta.EntityName[..4] == "NTRN" || meta.EntityName[..4] == "NTCO" || meta.EntityName[..4] == "NTME")
                {
                    stationShortName = meta.EntityName.Substring(6);
                }
                else
                {
                    stationShortName = meta.EntityName.Substring(5);
                }
                var printout = new FaxPrintout(

                    Loc.GetString(
                            goal,
                            ("date", _ticker.ICDateTime.ToString("dd.MM.yyyy")),
                            ("station", stationShortName)),
                    Loc.GetString("station-goal-fax-paper-name"),
                    null,
                    null,
                    "paper_stamp-centcom",
                    new List<StampDisplayInfo>
                    {
                        new() { StampedName = Loc.GetString("stamp-component-stamped-name-centcom"), StampedColor = Color.FromHex("#006600") },
                    });
                _faxSystem.Receive(uid, printout, Loc.GetString("fax-component-sender-name-centcom"), fax);
                var ccFaxes = EntityManager.EntityQueryEnumerator<FaxMachineComponent>();
                while (ccFaxes.MoveNext(out var ccFaxUid, out var ccFax))
                {
                    if (!ccFax.CentcomFax) continue;
                    _faxSystem.Receive(ccFaxUid, printout, Loc.GetString("fax-component-sender-name-centcom"), ccFax);
                }

                wasSent = true;
            }

            return wasSent;
        }

        public bool SendProtoStationGoal(StationGoalPrototype goal)
        {
            var faxes = EntityManager.EntityQueryEnumerator<FaxMachineComponent>();
            var wasSent = false;
            //foreach (var fax in faxes)
            while (faxes.MoveNext(out var uid, out var fax))
            {
                if (!fax.ReceiveStationGoal) continue;

                if (!TryComp<MetaDataComponent>(_station.GetOwningStation(uid), out var meta))
                    continue;

                var stationShortName = "ERR12. Ошибка автоматической системы рассылки целей станции. Пожалуйста, обратитесь к Центральному Командованию.";
                if (meta.EntityName[..4] == "NTBS" || meta.EntityName[..4] == "NTEX" || meta.EntityName[..4] == "NTTS" || meta.EntityName[..4] == "NTRT")
                {
                    stationShortName = meta.EntityName.Substring(5);
                }
                else
                {
                    stationShortName = meta.EntityName.Substring(6);
                }

                var printout = new FaxPrintout(
                    Loc.GetString(
                            goal.Text,
                            ("date", DateTime.Now.AddYears(1000).ToString("dd.MM.yyyy")),
                            ("station", stationShortName)),
                    Loc.GetString("station-goal-fax-paper-name"),
                    null,
                    null,
                    "paper_stamp-centcom",
                    new List<StampDisplayInfo>
                    {
                        new() { StampedName = Loc.GetString("stamp-component-stamped-name-centcom"), StampedColor = Color.FromHex("#006600") },
                    });
                _faxSystem.Receive(uid, printout, Loc.GetString("fax-component-sender-name-centcom"), fax);
                var ccFaxes = EntityManager.EntityQueryEnumerator<FaxMachineComponent>();
                while (ccFaxes.MoveNext(out var ccFaxUid, out var ccFax))
                {
                    if (!ccFax.CentcomFax) continue;
                    _faxSystem.Receive(ccFaxUid, printout, Loc.GetString("fax-component-sender-name-centcom"), ccFax);
                }

                wasSent = true;
            }

            return wasSent;
        }
    }
}
