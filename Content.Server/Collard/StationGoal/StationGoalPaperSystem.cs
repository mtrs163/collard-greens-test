using Content.Server.Fax;
using Content.Server.MassMedia.Systems;
using Content.Server.Station.Systems;
using Content.Shared.Fax.Components;
using Content.Shared.Collard.GameTicking;
using Content.Server.Collard.StationGoal;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Collard.StationGoal
{
    /// <summary>
    ///     System to spawn paper with station goal.
    /// </summary>
    public sealed partial class StationGoalPaperSystem : EntitySystem
    {
        [Dependency] private IPrototypeManager _proto = default!;
        [Dependency] private IRobustRandom _random = default!;
        [Dependency] private FaxSystem _fax = default!;
        [Dependency] private NewsSystem _news = default!;
        [Dependency] private StationSystem _station = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<RoundStartedEvent>(OnRoundStarted);
        }

        private void OnRoundStarted(RoundStartedEvent ev)
        {

            var query = EntityQueryEnumerator<StationGoalComponent>();
            while (query.MoveNext(out var uid, out var station))
            {
                var tempGoals = new List<ProtoId<StationGoalPrototype>>(station.Goals);
                var goalId = _random.Pick(tempGoals);
                var goalProto = _proto.Index(goalId);

                if (goalProto is null)
                    return;

                if (SendStationGoal(uid, goalProto))
                {
                    Log.Info($"Goal {goalProto.ID} has been sent to station {MetaData(uid).EntityName}");
                }
            }
        }

        public bool SendStationGoal(EntityUid ent, ProtoId<StationGoalPrototype> goal)
        {
            return SendStationGoal(ent, _proto.Index(goal));
        }

        /// <summary>
        ///     Send a station goal on selected station to all faxes which are authorized to receive it.
        /// </summary>
        /// <returns>True if at least one fax received paper</returns>
        public bool SendStationGoal(EntityUid ent, StationGoalPrototype goal)
        {
            var printout = new FaxPrintout(
                Loc.GetString(goal.Text, ("station", MetaData(ent).EntityName),
                                        ("date", DateTime.Now.AddYears(1000).ToString("dd.MM.yyyy"))),
                Loc.GetString("station-goal-fax-paper-name"),
                null,
                null,
                "paper_stamp-centcom",
                [new() { StampedName = Loc.GetString("stamp-component-stamped-name-centcom"), StampedColor = Color.FromHex("#006600") }]
            );

            var wasSent = false;
            var query = EntityQueryEnumerator<FaxMachineComponent>();
            while (query.MoveNext(out var faxUid, out var fax))
            {
                if (!fax.ReceiveAllStationGoals && !(fax.ReceiveStationGoal && _station.GetOwningStation(faxUid) == ent))
                    continue;

                _fax.Receive(faxUid, printout, null, fax);

                foreach (var spawnEnt in goal.Spawns)
                    SpawnAtPosition(spawnEnt, Transform(faxUid).Coordinates);

                wasSent |= fax.ReceiveStationGoal;
            }

            var ccFaxes = EntityQueryEnumerator<FaxMachineComponent>();
            while (ccFaxes.MoveNext(out var ccFaxUid, out var ccFax))
            {
                if (!ccFax.CentcomFax) continue;
                _fax.Receive(ccFaxUid, printout, null, ccFax);
            }

            // Publish news if at least one fax received the goal.
            if (wasSent)
            {
                PublishStationGoalNews(ent, goal);
            }

            return wasSent;
        }

        /// <summary>
        ///     Publishes a news article about the station goal in the mass media.
        /// </summary>
        private void PublishStationGoalNews(EntityUid ent, StationGoalPrototype goal)
        {
            var stationName = MetaData(ent).EntityName;

            var title = Loc.GetString("station-goal-news-title", ("station", stationName));


            var content = Loc.GetString(goal.Text, ("station", stationName),
                                            ("date", DateTime.Now.AddYears(1000).ToString("dd.MM.yyyy")));

            var endPattern = Loc.GetString("station-goal-end");

            if (content.EndsWith(endPattern))
            {
                content = content[..^endPattern.Length];
                content = content.TrimEnd();
            }

            _news.TryAddNews(ent, title, content, out _, Loc.GetString("station-goal-news-author"));
        }

        public bool SendProtoStationGoal(StationGoalPrototype goal)
        {
            var faxes = EntityQueryEnumerator<FaxMachineComponent>();
            var wasSent = false;
            //foreach (var fax in faxes)
            while (faxes.MoveNext(out var uid, out var fax))
            {
                if (!fax.ReceiveStationGoal) continue;

                if (!TryComp<MetaDataComponent>(_station.GetOwningStation(uid), out var meta))
                    continue;

                var stationName = meta.EntityName;

                var printout = new FaxPrintout(
                Loc.GetString(goal.Text, ("station", stationName),
                                        ("date", DateTime.Now.AddYears(1000).ToString("dd.MM.yyyy"))),
                Loc.GetString("station-goal-fax-paper-name"),
                null,
                null,
                "paper_stamp-centcom",
                [new() { StampedName = Loc.GetString("stamp-component-stamped-name-centcom"), StampedColor = Color.FromHex("#006600") }]
            );
                _fax.Receive(uid, printout, Loc.GetString("fax-component-sender-name-centcom"), fax);
                var ccFaxes = EntityQueryEnumerator<FaxMachineComponent>();
                while (ccFaxes.MoveNext(out var ccFaxUid, out var ccFax))
                {
                    if (!ccFax.CentcomFax) continue;
                    _fax.Receive(ccFaxUid, printout, Loc.GetString("fax-component-sender-name-centcom"), ccFax);
                }

                wasSent = true;
            }

            return wasSent;
        }
    }
}
