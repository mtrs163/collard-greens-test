using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random; // collard-Localization
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class FrontalLispSystem : EntitySystem
{
    // @formatter:off
    private static readonly Regex RegexUpperTh = new(@"[T]+[Ss]+|[S]+[Cc]+(?=[IiEeYy]+)|[C]+(?=[IiEeYy]+)|[P][Ss]+|([S]+[Tt]+|[T]+)(?=[Ii]+[Oo]+[Uu]*[Nn]*)|[C]+[Hh]+(?=[Ii]*[Ee]*)|[Z]+|[S]+|[X]+(?=[Ee]+)");
    private static readonly Regex RegexLowerTh = new(@"[t]+[s]+|[s]+[c]+(?=[iey]+)|[c]+(?=[iey]+)|[p][s]+|([s]+[t]+|[t]+)(?=[i]+[o]+[u]*[n]*)|[c]+[h]+(?=[i]*[e]*)|[z]+|[s]+|[x]+(?=[e]+)");
    private static readonly Regex RegexUpperEcks = new(@"[E]+[Xx]+[Cc]*|[X]+");
    private static readonly Regex RegexLowerEcks = new(@"[e]+[x]+[c]*|[x]+");
    // collard-Localization-start (velikiy naser)
    private static readonly Regex RegexLowerSCyr = new(@"с+");
    private static readonly Regex RegexUpperSCyr = new(@"С+");
    private static readonly Regex RegexLowerChCyr = new(@"ч+");
    private static readonly Regex RegexUpperChCyr = new(@"Ч+");
    private static readonly Regex RegexLowerCCyr = new(@"ц+");
    private static readonly Regex RegexUpperCCyr = new(@"Ц+");
    private static readonly Regex RegexLowerTCyr = new(@"\B[т](?![АЕЁИОУЫЭЮЯаеёиоуыэюя])");
    private static readonly Regex RegexUpperTCyr = new(@"\B[Т](?![АЕЁИОУЫЭЮЯаеёиоуыэюя])");
    private static readonly Regex RegexLowerZCyr = new(@"з+");
    private static readonly Regex RegexUpperZCyr = new(@"З+");
    // collard-Localization-end
    // @formatter:on

    [Dependency] private readonly IRobustRandom _random = default!; // collard-Localization

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FrontalLispComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, FrontalLispComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // handles ts, sc(i|e|y), c(i|e|y), ps, st(io(u|n)), ch(i|e), z, s
        message = RegexUpperTh.Replace(message, "TH");
        message = RegexLowerTh.Replace(message, "th");
        // handles ex(c), x
        message = RegexUpperEcks.Replace(message, "EKTH");
        message = RegexLowerEcks.Replace(message, "ekth");

        // collard-Localization-start
        // с - ш
        message = RegexLowerSCyr.Replace(message, _random.Prob(0.90f) ? "ш" : "с");
        message = RegexUpperSCyr.Replace(message, _random.Prob(0.90f) ? "Ш" : "С");
        // ч - ш
        message = RegexLowerChCyr.Replace(message, _random.Prob(0.90f) ? "ш" : "ч");
        message = RegexUpperChCyr.Replace(message, _random.Prob(0.90f) ? "Ш" : "Ч");
        // ц - ч
        message = RegexLowerCCyr.Replace(message, _random.Prob(0.90f) ? "ч" : "ц");
        message = RegexUpperCCyr.Replace(message, _random.Prob(0.90f) ? "Ч" : "Ц");
        // т - ч
        message = RegexLowerTCyr.Replace(message, _random.Prob(0.90f) ? "ч" : "т");
        message = RegexUpperTCyr.Replace(message, _random.Prob(0.90f) ? "Ч" : "Т");
        // з - ж
        message = RegexLowerZCyr.Replace(message, _random.Prob(0.90f) ? "ж" : "з");
        message = RegexUpperZCyr.Replace(message, _random.Prob(0.90f) ? "Ж" : "З");
        // collard-Localization-end

        args.Message = message;
    }
}
