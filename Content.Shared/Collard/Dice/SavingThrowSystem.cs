using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Shared.Collard.Dice;

public sealed partial class SavingThrowSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAudioSystem _audio = default!;

    public bool InitiateSavingThrow(EntityUid uid, int difficulty)
    {
        var throwResult = _random.Next(1, 21);
        if (throwResult >= difficulty)
        {
            _popup.PopupEntity(Loc.GetString("dice-saving-throw-successful"), uid);
            _audio.PlayPvs(new SoundPathSpecifier("/Audio/Collard/Misc/saving_success.ogg"), uid, AudioParams.Default);
            return true;
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("dice-saving-throw-failed"), uid);
            _audio.PlayPvs(new SoundPathSpecifier("/Audio/Collard/Misc/saving_failed.ogg"), uid, AudioParams.Default);
            return false;
        }
    }

    public bool InitiateSilentSavingThrow(EntityUid uid, int difficulty)
    {
        var throwResult = _random.Next(1, 21);
        if (throwResult >= difficulty)
        {
            _popup.PopupEntity(Loc.GetString("dice-saving-throw-successful"), uid);
            return true;
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("dice-saving-throw-failed"), uid);
            return false;
        }
    }

    public bool InitiateSavingThrowPredicted(EntityUid uid, int difficulty)
    {
        var throwResult = _random.Next(1, 21);
        if (throwResult >= difficulty)
        {
            _popup.PopupPredicted(Loc.GetString("dice-saving-throw-successful"), uid, null);
            _audio.PlayPredicted(new SoundPathSpecifier("/Audio/Collard/Misc/saving_success.ogg"), uid, null, AudioParams.Default);
            return true;
        }
        else
        {
            _popup.PopupPredicted(Loc.GetString("dice-saving-throw-failed"), uid, null);
            _audio.PlayPredicted(new SoundPathSpecifier("/Audio/Collard/Misc/saving_failed.ogg"), uid, null, AudioParams.Default);
            return false;
        }
    }

    public bool InitiateSilentSavingThrowPredicted(EntityUid uid, int difficulty)
    {
        var throwResult = _random.Next(1, 21);
        if (throwResult >= difficulty)
        {
            _popup.PopupPredicted(Loc.GetString("dice-saving-throw-successful"), uid, null);
            return true;
        }
        else
        {
            _popup.PopupPredicted(Loc.GetString("dice-saving-throw-failed"), uid, null);
            return false;
        }
    }
}
