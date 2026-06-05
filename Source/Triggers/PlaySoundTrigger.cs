using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.AletrisSandbox.Triggers;

[CustomEntity("AletrisSandbox/PlaySoundTrigger")]
public class PlaySoundTrigger : Trigger
{

    private int pitch;
    private string sound;
    private bool atPlr;
    private float volume;

    public static float convertToFreqA4(int offset)
    {
        return 440f * (float)Math.Pow(2f, offset / 12f);
    }

    public PlaySoundTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        pitch = data.Int("pitch", 60);
        sound = data.Attr("soundId", "");
        volume = data.Float("volume", 1f);
        atPlr = data.Bool("atPlayer");
    }

    public override void OnEnter(Player player)
    {
        Vector2 position = (player != null && atPlr) ? player.Position : Position;

        float pih = convertToFreqA4(pitch - 69) / 440f; // yippee 12tet
        FMOD.Studio.EventInstance instance = Audio.Play(SFX.EventnameByHandle(sound), position);

        instance?.setVolume(volume);
        instance?.setPitch(pih);
        base.OnEnter(player);
    }
}