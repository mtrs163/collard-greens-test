using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech.EntitySystems;

namespace Content.Server.Speech.EntitySystems;

public sealed class MothAccentSystem : RelayAccentSystem<MothAccentComponent>
{
    private static readonly Regex RegexLowerBuzz = new Regex("z{1,3}");
    private static readonly Regex RegexUpperBuzz = new Regex("Z{1,3}");
    // collard-Localization-start
    private static readonly Regex RegexLowerBuzzCyr = new Regex("з{1,3}");
    private static readonly Regex RegexUpperBuzzCyr = new Regex("З{1,3}");
    private static readonly Regex RegexLowerBuzzZhCyr = new Regex("ж{1,3}");
    private static readonly Regex RegexUpperBuzzZhCyr = new Regex("Ж{1,3}");
    // collard-Localization-end

    public override string Accentuate(string message, Entity<MothAccentComponent>? ent = null)
    {
        // buzzz
        message = RegexLowerBuzz.Replace(message, "zzz");
        // buZZZ
        message = RegexUpperBuzz.Replace(message, "ZZZ");

        // collard-Localization-start
        // ж => жжж
        message = RegexLowerBuzzZhCyr.Replace(message, "жжж");
        // Ж => ЖЖЖ
        message = RegexUpperBuzzZhCyr.Replace(message, "ЖЖЖ");
        // з => ззз
        message = RegexLowerBuzzCyr.Replace(message, "ззз");
        // З => ЗЗЗ
        message = RegexUpperBuzzCyr.Replace(message, "ЗЗЗ");
        // collard-Localization-end

        return message;
    }
}
