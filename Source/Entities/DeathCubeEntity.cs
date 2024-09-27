using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;
using Directions = Celeste.MoveBlock.Directions;

namespace Celeste.Mod.CyrusSandbox.Entities {
    [CustomEntity("CyrusHelper/DeathcubeEntity")]
    public class DeathcubeEntity : Entity {
        private PlayerCollider pc;
        private Solid solid;
        private readonly bool hitboxes;
        public readonly Directions direction;
        Sprite sprite = GFX.SpriteBank.Create("DeathcubeEntity");

        public DeathcubeEntity(EntityData data, Vector2 offset)
            : base(data.Position + offset) {
            base.Depth = 0;
            Add(new ClimbBlocker(edge: true));
            hitboxes = data.Bool("unforgivingHitbox", false);
            direction = data.Enum("direction", Directions.Up);
            Add(pc = new PlayerCollider(OnCollide));
            //help how do i use dictionaries to make this better
            
            switch(direction)
            {
                case Directions.Left:
                    sprite.Rotation = 90f.ToRad();
                    break;
                case Directions.Right:
                    sprite.Rotation = 270f.ToRad();
                    break;
                case Directions.Down:
                    sprite.Rotation = 180f.ToRad();
                    break;
            }

            switch(hitboxes)
            {
                case true:
                    Collider = new Hitbox(16, 16, -8, -8);
                    break;
                case false:
                    Collider = new Hitbox(14, 14, -7, -7);
                    break;
            }
            Add(sprite);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnCollide(Player player)
        {
            if (OnCollide != null)
            {
                player.Die((player.Center - base.Center).SafeNormalize());
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            switch (hitboxes)
            {
                case true:
                    scene.Add(solid = new Solid(Position + new Vector2(-4, -4), base.Width - 6f, base.Height - 6f, safe: false));
                    break;
                case false:
                    scene.Add(solid = new Solid(Position + new Vector2(-4,-4), base.Width - 6f, base.Height - 6f, safe: false));
                    break;
            }
        }
    }
}
