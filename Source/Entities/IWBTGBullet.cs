using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;


// thank you code_modding and especially kalobi and snip for dealing with me being dumb at code modding
// without them this would not have worked


namespace Celeste.Mod.CyrusSandbox.Entities
{
    [CustomEntity("CyrusSandbox/IWBTGBullet")]
    [Tracked]
    public class IWBTGBullet : Actor
    {
        public Vector2 velocity;
        public readonly Player owner;
        private bool dead = false;

        private readonly Collision onCollideH;
        private readonly Collision onCollideV;

        private int lifetime;

        public IWBTGBullet(Vector2 position, Vector2 velocity, Player owner) : base(position)
        {

            Position = position;
            this.velocity = velocity;
            this.owner = owner;

            Depth = 100;
            Collider = new Hitbox(4f, 4f, 0f, 0f);

            onCollideH += OnCollideH;
            onCollideV += OnCollideV;

            lifetime = 6000;

            (owner.Scene as Level).Add(this);
            Add(new Image(GFX.Game["CyrusSandbox/gun/bullet"]));
        }

        private void OnCollideH(CollisionData data)
        {
            if (!(CyrusSandboxModule.Settings.IWBTGGunOptions.IWBTGGunHitsStuffOverride || CyrusSandboxModule.Session.IWBTGGunHitsStuff))
            {
                Kill();
                return;
            }
            if (data.Hit.ToString() == "Celeste.SolidTiles") { Kill(); return; }
        }

        private void OnCollideV(CollisionData data)
        {
            if (!(CyrusSandboxModule.Settings.IWBTGGunOptions.IWBTGGunHitsStuffOverride || CyrusSandboxModule.Session.IWBTGGunHitsStuff))
            {
                Kill();
                return;
            }
            if (data.Hit.ToString() == "Celeste.SolidTiles") { Kill(); return; }
        }


        public override void Update()
        {

            base.Update();

            MoveH(velocity.X, onCollideH);
            MoveV(velocity.Y, onCollideV);

            if (--lifetime <= 0) { Kill(); }

            /*Camera camera = (Scene as Level).Camera;
            if (Position.X < camera.X || Position.X > camera.X + 320f ||
                Position.Y < camera.Y || Position.Y > camera.Y + 180f)
            {
                Kill();
            */

            Level level = SceneAs<Level>();
            if (Position.X <= level.Bounds.Left || Position.X >= level.Bounds.Right ||
               Position.Y >= level.Bounds.Bottom || Position.Y <= level.Bounds.Top)
            {
                Kill();
            }



            foreach (CyrusSandboxModule.BulletCollider collider in Scene.Tracker.GetComponents<CyrusSandboxModule.BulletCollider>())
            {
                if (collider.Check(this))
                {
                    collider.OnCollide(this);
                    if (dead) return;
                }
            }
        }

        public override void Render()
        {
            if (owner.Scene != null)
            {
                //(owner.Scene as Level).Particles.Emit(ParticleTypes.SparkyDust, Position, Color.Yellow);
            }
            base.Render();
        }

        public void Kill()
        {
            dead = true;
            RemoveSelf();
        }

    }

}



