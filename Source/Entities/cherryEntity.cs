using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.CyrusSandbox.Entities
{

    [CustomEntity("CyrusHelper/cherryEntity")]

    public class cherryEntity : Entity
    {
        public readonly bool UnforgivingHitbox;

        public readonly int AnimationRate;

        public readonly bool AnimatedHitbox;

        public readonly bool BigHitbox;

        public Color color;

        private PlayerCollider pc;

        Sprite sprite = GFX.SpriteBank.Create("cherryEntity");

        Sprite bigsprite = GFX.SpriteBank.Create("cherryEntityBig");

        [MethodImpl(MethodImplOptions.NoInlining)]

        public cherryEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

            base.Depth = -1;
            AnimationRate = data.Int("animationRate", 30);
            UnforgivingHitbox = data.Bool("unforgivingHitbox", false);
            AnimatedHitbox = data.Bool("animatedHitbox", true);
            color = data.HexColor("color", Calc.HexToColor("#FF0000"));
            BigHitbox = data.Bool("bigHitbox", false);

            sprite.Color = color;
            bigsprite.Color = color;

            switch (BigHitbox)
            {
                case true:
                    switch (UnforgivingHitbox)
                    {
                        case false:
                            base.Collider = new Circle(5f);
                            break;
                        case true:
                            base.Collider = new Circle(6f);
                            break;
                    }
                    Add(bigsprite);
                    break;

                case false:
                    switch (UnforgivingHitbox)
                    {
                        case false:
                            base.Collider = new Circle(3f);
                            break;
                        case true:
                            base.Collider = new Circle(4f);
                            break;
                    }
                    Add(sprite);
                    break;
            }

            Add(new LedgeBlocker());
            Add(pc = new PlayerCollider(OnCollide));

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnCollide(Player player)
        {
            if (OnCollide != null)
            {
                player.Die((player.Center - base.Center).SafeNormalize());
            }
        }

        public override void Update()
        {

        }

    }
}