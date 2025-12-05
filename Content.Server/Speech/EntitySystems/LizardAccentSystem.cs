using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class LizardAccentSystem : EntitySystem
{
    private static readonly Regex RegexLowerS = new("s+");
    private static readonly Regex RegexUpperS = new("S+");
    private static readonly Regex RegexInternalX = new(@"(\w)x");
    private static readonly Regex RegexLowerEndX = new(@"\bx([\-|r|R]|\b)");
    private static readonly Regex RegexUpperEndX = new(@"\bX([\-|r|R]|\b)");
    // collard-Localization-start
    private static readonly Regex RegexLowerSCyr = new("с+");
    private static readonly Regex RegexUpperSCyr = new("С+");
    private static readonly Regex RegexLowerZCyr = new("з+");
    private static readonly Regex RegexUpperZCyr = new("З+");
    private static readonly Regex RegexLowerShCyr = new("ш+");
    private static readonly Regex RegexUpperShCyr = new("Ш+");
    private static readonly Regex RegexLowerChCyr = new("ч+");
    private static readonly Regex RegexUpperChCyr = new("Ч+");
    private static readonly Regex RegexLowerSchCyr = new("щ+");
    private static readonly Regex RegexUpperSchCyr = new("Щ+");
    private static readonly Regex RegexLowerTsCyr = new("ц+");
    private static readonly Regex RegexUpperTsCyr = new("Ц+");
    private static readonly Regex RegexLowerZhCyr = new("ж+");
    private static readonly Regex RegexUpperZhCyr = new("Ж+");
    // collard-Localization-end

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LizardAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, LizardAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // hissss
        message = RegexLowerS.Replace(message, "sss");
        // hiSSS
        message = RegexUpperS.Replace(message, "SSS");
        // ekssit
        message = RegexInternalX.Replace(message, "$1kss");
        // ecks
        message = RegexLowerEndX.Replace(message, "ecks$1");
        // eckS
        message = RegexUpperEndX.Replace(message, "ECKS$1");
        // collard-Localization-start
        // c => ссс
        message = RegexLowerSCyr.Replace(message, "ссс");
        // С => CCC
        message = RegexUpperSCyr.Replace(message, "ССС");
        // з => ссс
        message = RegexLowerZCyr.Replace(message, "ссс");
        // З => CCC
        message = RegexUpperZCyr.Replace(message, "ССС");
        // ш => шшш
        message = RegexLowerShCyr.Replace(message, "шшш");
        // Ш => ШШШ
        message = RegexUpperShCyr.Replace(message, "ШШШ");
        // ч => щщщ
        message = RegexLowerChCyr.Replace(message, "щщщ");
        // Ч => ЩЩЩ
        message = RegexUpperChCyr.Replace(message, "ЩЩЩ");
        // щ => щщщ
        message = RegexLowerSchCyr.Replace(message, "щщщ");
        // Щ => ЩЩЩ
        message = RegexUpperSchCyr.Replace(message, "ЩЩЩ");
        // ц => тссс
        message = RegexLowerTsCyr.Replace(message, "тссс");
        // Ц => ТССС
        message = RegexUpperTsCyr.Replace(message, "ТССС");
        // ж => шшш
        message = RegexLowerZhCyr.Replace(message, "шшш");
        // Ж => ШШШ
        message = RegexUpperZhCyr.Replace(message, "ШШШ");
        // collard-Localization-end
        args.Message = message;
    }
}
