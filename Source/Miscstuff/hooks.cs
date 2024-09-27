using Celeste.Mod.CyrusSandbox.Entities;
using Microsoft.Xna.Framework;
using Monocle;

// THANK YOU EVERYBODY IN #CODE_MODDING

namespace Celeste.Mod.CyrusSandbox.GunSupport
{
    internal static class VanillaHooks
    {
        public static void Load()
        {
            On.Celeste.CrystalStaticSpinner.ctor_Vector2_bool_CrystalColor += CrystalSpinnerHook;
            On.Celeste.CrushBlock.ctor_Vector2_float_float_Axes_bool += KevinHook;
            On.Celeste.FlyFeather.ctor_Vector2_bool_bool += FeatherHook;
            On.Celeste.Bumper.ctor_Vector2_Nullable1 += BumperHook;
        }
        public static void Unload()
        {
            On.Celeste.CrystalStaticSpinner.ctor_Vector2_bool_CrystalColor -= CrystalSpinnerHook;
            On.Celeste.CrushBlock.ctor_Vector2_float_float_Axes_bool -= KevinHook;
            On.Celeste.FlyFeather.ctor_Vector2_bool_bool -= FeatherHook;
            On.Celeste.Bumper.ctor_Vector2_Nullable1 -= BumperHook;
        }

        private static void CrystalSpinnerHook(On.Celeste.CrystalStaticSpinner.orig_ctor_Vector2_bool_CrystalColor orig, CrystalStaticSpinner self, Vector2 position, bool attachToSolid, CrystalColor color)
        {
            void CollisionHandler(IWBTGBullet bullet)
            {
                if (!(CyrusSandboxModule.Settings.IWBTGGunOptions.IWBTGGunHitsStuffOverride || CyrusSandboxModule.Session.IWBTGGunHitsStuff)) { return; }
                bullet.Kill();
                if (!(CyrusSandboxModule.Settings.IWBTGGunOptions.IWBTGGunDestroyStuffOverride || CyrusSandboxModule.Session.IWBTGGunDestroysStuff)) { return; }
                self.Destroy();
            }

            orig(self, position, attachToSolid, color);
            self.Add(new CyrusSandboxModule.BulletCollider(CollisionHandler, self.Collider));
        }

        private static void KevinHook(On.Celeste.CrushBlock.orig_ctor_Vector2_float_float_Axes_bool orig, CrushBlock self, Vector2 position, float width, float height, CrushBlock.Axes axes, bool chillOut)
        {
            void CollisionHandler(IWBTGBullet bullet)
            {
                if (!(CyrusSandboxModule.Settings.IWBTGGunOptions.IWBTGGunHitsStuffOverride || CyrusSandboxModule.Session.IWBTGGunHitsStuff)) { return; }
                self.Attack(-bullet.velocity.SafeNormalize());
                bullet.Kill();
            }

            orig(self, position, width, height, axes, chillOut);

            Collider collidere = new Hitbox(self.Width + 4f, self.Height + 4f, self.Collider.Left - 2f, self.Collider.Top - 2f);
            self.Add(new CyrusSandboxModule.BulletCollider(CollisionHandler, collidere));
        }

        private static void FeatherHook(On.Celeste.FlyFeather.orig_ctor_Vector2_bool_bool orig, FlyFeather self, Vector2 position, bool shielded, bool singleUse)
        {
            void CollisionHandler(IWBTGBullet bullet)
            {
                if (!(CyrusSandboxModule.Settings.IWBTGGunOptions.IWBTGGunHitsStuffOverride || CyrusSandboxModule.Session.IWBTGGunHitsStuff)) { return; }
                if (!(self.shielded))
                {
                    self.OnPlayer(bullet.owner);
                    bullet.Kill();
                }
                else
                {
                    self.moveWiggle.Start();
                    self.shieldRadiusWiggle.Start();
                    self.moveWiggleDir = (self.Center - bullet.Position).SafeNormalize(Vector2.UnitY);
                    Audio.Play("event:/game/06_reflection/feather_bubble_bounce", bullet.Position);
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    bullet.velocity = -bullet.velocity;
                }
            }
            orig(self, position, shielded, singleUse);
            self.Add(new CyrusSandboxModule.BulletCollider(CollisionHandler, self.Collider));
        }

        private static void BumperHook(On.Celeste.Bumper.orig_ctor_Vector2_Nullable1 orig, Bumper self, Vector2 position, Vector2? node)
        {
            void CollisionHandler(IWBTGBullet bullet)
            {
                if (self.fireMode)
                {
                    Vector2 vector = (bullet.Position - self.Center).SafeNormalize();
                    self.hitDir = -vector;
                    self.hitWiggler.Start();
                    Audio.Play("event:/game/09_core/hotpinball_activate", self.Position);
                    bullet.Kill();
                    self.SceneAs<Level>().Particles.Emit(Bumper.P_FireHit, 12, self.Center + vector * 12f, Vector2.One * 3f, vector.Angle());
                }
                else if (self.respawnTimer <= 0f)
                {
                    Audio.Play("event:/game/06_reflection/pinballbumper_hit", bullet.Position);
                    self.respawnTimer = 0.6f;
                    Vector2 vector = (self.Center - bullet.Position).SafeNormalize(-Vector2.UnitY);
                    self.sprite.Play("hit", restart: true);
                    self.spriteEvil.Play("hit", restart: true);
                    self.light.Visible = false;
                    self.bloom.Visible = false;
                    self.SceneAs<Level>().DirectionalShake(vector, 0.15f);
                    self.SceneAs<Level>().Displacement.AddBurst(self.Center, 0.3f, 8f, 32f, 0.8f);
                    self.SceneAs<Level>().Particles.Emit(Bumper.P_Launch, 12, self.Center + vector * 12f, Vector2.One * 3f, vector.Angle());
                    bullet.velocity = -vector * bullet.velocity.Length();
                }

            }

            orig(self, position, node);
            self.Add(new CyrusSandboxModule.BulletCollider(CollisionHandler, self.Collider));

        }


    }
}
