using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.AletrisSandbox.Entities;

[CustomEntity("AletrisSandbox/CherryEntity")]
public class CherryEntity : Entity
{
    public bool Inverted;

    public bool UnforgivingHitbox;
    public int AnimationRate;
    public bool AnimatedHitbox;
    public bool BigHitbox;
    public Vector2 velocity;
    public Color color;

    PlayerCollider pc;

    Sprite sprite = GFX.SpriteBank.Create("CherryEntity");
    Sprite bigsprite = GFX.SpriteBank.Create("CherryEntityBig");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public CherryEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = -1;
        Inverted = data.Bool("Inverted");
        AnimationRate = data.Int("animationRate", 30);
        UnforgivingHitbox = data.Bool("unforgivingHitbox");
        AnimatedHitbox = data.Bool("animatedHitbox", true);
        color = data.HexColor("color", Color.LightPink);
        BigHitbox = data.Bool("bigHitbox");

        sprite.SetColor(color);
        bigsprite.SetColor(color);

        if (UnforgivingHitbox)
        { Collider = new Circle(UnforgivingHitbox ? 6f : 5f); }
        else
        { Collider = new Circle(UnforgivingHitbox ? 4f : 3f); }

        // ReSharper disable twice PossibleLossOfFraction
        if (AnimationRate > 0)
        {
            sprite.Rate = 1 / AnimationRate;
            bigsprite.Rate = 1 / AnimationRate;
        }

        if (Inverted)
        {
            sprite.Play("Iidle", true);
            bigsprite.Play("Iidle", true);
        }
        else
        {
            sprite.Play("idle", true);
            bigsprite.Play("idle", true);
        }

        Add(BigHitbox ? bigsprite : sprite);
        Add(new LedgeBlocker());
        Add(pc = new(OnCollide));
    }

    public override void Update()
    {

        base.Update();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void OnCollide(Player player)
    {
        player.Die((player.Center - Center).SafeNormalize());
    }
}