using Celeste.Mod.CyrusSandbox.Entities;
using Celeste.Mod.CyrusSandbox.GunSupport;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;

namespace Celeste.Mod.CyrusSandbox
{
    public class CyrusSandboxModule : EverestModule
    {
        public static CyrusSandboxModule Instance { get; private set; }

        public override Type SettingsType => typeof(CyrusSandboxModuleSettings);
        public static CyrusSandboxModuleSettings Settings => (CyrusSandboxModuleSettings)Instance._Settings;

        public override Type SessionType => typeof(CyrusSandboxModuleSession);
        public static CyrusSandboxModuleSession Session => (CyrusSandboxModuleSession)Instance._Session;

        public CyrusSandboxModule()
        {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(CyrusSandboxModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(CyrusSandboxModule), LogLevel.Info);
#endif
        }

        public MTexture gunTexture;
        private Texture2D crossTexture;

        private VirtualJoystick joystickAim;
        private Vector2 oldJoystickAim;
        private Vector2 oldMouseCursorPos = Vector2.Zero;
        private Vector2 CursorPos = Vector2.Zero;
        private bool usingJoystickAim = false;

        private static MouseState State => Mouse.GetState();
        private static Vector2 MouseCursorPos => Vector2.Transform(new Vector2(State.X, State.Y), Matrix.Invert(Engine.ScreenMatrix));
        public override void LoadContent(bool firstLoad)
        {
            gunTexture = GFX.Game["CyrusSandbox/gun/gun"];
            crossTexture = GFX.Game["CyrusSandbox/gun/crosshair"].Texture.Texture;
        }
        private void EverestExitMethod(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
        {
            Settings.IWBTGGunOptions.IWBTGGunEnableOverride = Session.oldEnabledConfig;
            Settings.HealthOptions.HPSystemEnableOverride = Session.oldEnabledConfig;
        }
        private void EverestEnterMethod(Session session, bool fromSaveData)
        {
            Session.oldEnabledConfig = Settings.IWBTGGunOptions.IWBTGGunEnableOverride;
            Session.oldEnabledConfig = Settings.HealthOptions.HPSystemEnableOverride;
        }

        private static Vector2 PlayerPosScreenSpace(Actor self)
        {
            return self.Center - (self.Scene as Level).Camera.Position;
        }

        private static Vector2 ToCursor(Actor player, Vector2 cursorPos)
        {
            return Vector2.Normalize(cursorPos / 6f - PlayerPosScreenSpace(player));
        }

        private static float ToRotation(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        private void GunPlayerRender(On.Celeste.Player.orig_Render orig, Player self)
        {
            orig(self);

            if (!(Settings.IWBTGGunOptions.IWBTGGunEnableOverride || Session.IWBTGGunEnabled)) return;

            SpriteEffects flip = SpriteEffects.None;
            Vector2 offset;

            Vector2 gunVector = ToCursor(self, CursorPos);

            float gunRotation = Math.Min(Math.Max(ToRotation(gunVector), -1.2f), 1.2f);

            if (Settings.IWBTGGunOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled)
            {

                self.Facing = (Facings)Math.Sign(ToCursor(self, CursorPos).X);
                if (self.Facing == 0) self.Facing = Facings.Right;

                if (gunVector.Y > 0)
                {
                    gunRotation = ToRotation(gunVector);
                }
                else
                {
                    gunRotation = ToRotation(gunVector);
                }

                if (self.Facing == Facings.Left)
                {
                    flip = SpriteEffects.FlipVertically;
                }

                offset.X = 0.3f;
                offset.Y = 0.5f;

            }
            else
            {

                gunRotation = 0f;

                if (self.Facing == Facings.Left)
                {
                    flip = SpriteEffects.FlipHorizontally;
                    offset.X = 0.9f;
                    offset.Y = 0.5f;
                }
                else
                {
                    flip = SpriteEffects.None;
                    offset.X = 0.1f;
                    offset.Y = 0.5f;
                }

            }

            gunTexture.DrawJustified(
            self.Center,
            offset,
            Color.White,
            1f,
            gunRotation,
            flip
            );

        }

        private void GunLevelRender(On.Celeste.Level.orig_Render orig, Level self)
        {
            orig(self);

            if (!(Settings.IWBTGGunOptions.IWBTGGunEnableOverride || Session.IWBTGGunEnabled)) return;
            if (!(Settings.IWBTGGunOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled)) return;

            Draw.SpriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);

            Color aimColor = Color.White;

            if (self.Tracker.CountEntities<IWBTGBullet>() >= Session.Maxbullets)
            {
                aimColor = Color.Red;
            }
            else
            {
                aimColor = Color.White;
            }

            Draw.SpriteBatch.Draw(crossTexture, CursorPos, null, aimColor, 0f, new Vector2(crossTexture.Width / 2f, crossTexture.Height / 2f), 4f, 0, 0f);
            Draw.SpriteBatch.End();
        }

        private void GunPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {

            orig(self);

            bool boolToCheck;

            if (!(Settings.IWBTGGunOptions.IWBTGGunEnableOverride || Session.IWBTGGunEnabled)) return;

            // cursor pos update
            if (joystickAim.Value.LengthSquared() > 0.04f)
            {
                usingJoystickAim = true;
            }
            else if (MouseCursorPos != oldMouseCursorPos)
            {
                usingJoystickAim = false;
            }

            if (usingJoystickAim && self.Scene != null)
            {
                CursorPos = (PlayerPosScreenSpace(self) + oldJoystickAim * 70f) * 6f;
                if (joystickAim.Value.LengthSquared() > 0.04f)
                {
                    oldJoystickAim = joystickAim.Value;
                }
            }
            else
            {
                CursorPos = MouseCursorPos;
            }

            oldMouseCursorPos = MouseCursorPos;

            if (self.Scene == null || self.Scene.TimeActive <= 0f || (TalkComponent.PlayerOver != null && Input.Talk.Pressed))
            {
                return;
            }

            float turnOffset;

            if (self.Facing == Facings.Left)
            {
                turnOffset = -20f;
            }
            else
            {
                turnOffset = 0f;
            }


            Vector2 mouseposition = new Vector2(self.Center.X, self.Center.Y - 4.5f);
            Vector2 position = new Vector2(self.Center.X + 7f + turnOffset, self.Center.Y - 4.5f);

            if (Settings.IWBTGGunOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled)
            {
                boolToCheck = Settings.IWBTGGunOptions.ShootBullet.Pressed || MInput.Mouse.PressedLeftButton;
                if (Settings.IWBTGGunOptions.IWBTGGunAutoFireOverride || Session.IWBTGGunAutofireEnabled)
                {
                    boolToCheck = Settings.IWBTGGunOptions.ShootBullet.Check || MInput.Mouse.CheckLeftButton;
                }
            }
            else
            {
                boolToCheck = Settings.IWBTGGunOptions.ShootBullet.Pressed;
                if (Settings.IWBTGGunOptions.IWBTGGunAutoFireOverride || Session.IWBTGGunAutofireEnabled)
                {
                    boolToCheck = Settings.IWBTGGunOptions.ShootBullet.Check;
                }
            }

            if (boolToCheck)
            {

                if (self.Scene.Tracker.CountEntities<IWBTGBullet>() >= Session.Maxbullets) return;

                if (Settings.IWBTGGunOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled)
                {
                    new IWBTGBullet(mouseposition, ToCursor(self, CursorPos) * 5f, self);
                }
                else
                {
                    new IWBTGBullet(position, (self.Facing == Facings.Left ? new Vector2(-1, 0) : new Vector2(1, 0)) * 5f, self);
                }

                switch (Settings.IWBTGGunOptions.GunSound)
                {
                    case 0:
                        break;
                    case 1:
                        Audio.Play("event:/CyrusSandbox_stuff/fire2", "fade", 0.5f);
                        break;
                    case 2:
                        Audio.Play("event:/CyrusSandbox_stuff/fire1", "fade", 0.5f);
                        break;
                    case 3:
                        Audio.Play("event:/CyrusSandbox_stuff/fire3", "fade", 0.5f);
                        break;
                }

            }

        }

        private void HitboxPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {

            orig(self);

            if (!Settings.HitboxOptions.PlayerHitboxEnableOverride) return;

            string[] NormalHitboxSize = Settings.HitboxSizeMenu.NormalHitboxSize.Split(',');
            string[] NormalHurtboxSize = Settings.HitboxSizeMenu.NormalHurtboxSize.Split(',');
            string[] DuckHitboxSize = Settings.HitboxSizeMenu.DuckHitboxSize.Split(',');
            string[] DuckHurtboxSize = Settings.HitboxSizeMenu.DuckHurtboxSize.Split(',');
            string[] FeatherHitboxSize = Settings.HitboxSizeMenu.FeatherHitboxSize.Split(',');
            string[] FeatherHurtboxSize = Settings.HitboxSizeMenu.FeatherHurtboxSize.Split(',');

            string[] NormalHitboxOffset = Settings.HitboxOffsetMenu.NormalHitboxOffset.Split(',');
            string[] NormalHurtboxOffset = Settings.HitboxOffsetMenu.NormalHurtboxOffset.Split(',');
            string[] DuckHitboxOffset = Settings.HitboxOffsetMenu.DuckHitboxOffset.Split(',');
            string[] DuckHurtboxOffset = Settings.HitboxOffsetMenu.DuckHurtboxOffset.Split(',');
            string[] FeatherHitboxOffset = Settings.HitboxOffsetMenu.FeatherHitboxOffset.Split(',');
            string[] FeatherHurtboxOffset = Settings.HitboxOffsetMenu.FeatherHurtboxOffset.Split(',');

            Vector2 newHitboxSize = new Vector2(8f, 11f);
            Vector2 newHurtboxSize = new Vector2(8f, 6f);
            Vector2 newDuckHitboxSize = new Vector2(8f, 9f);
            Vector2 newDuckHurtboxSize = new Vector2(8f, 4f);
            Vector2 newFeatherHitboxSize = new Vector2(8f, 8f);
            Vector2 newFeatherHurtboxSize = new Vector2(6f, 6f);

            Vector2 newHitboxOffset = new Vector2(-4f, -11f);
            Vector2 newHurtboxOffset = new Vector2(-4f, -6f);
            Vector2 newDuckHitboxOffset = new Vector2(-4f, -11f);
            Vector2 newDuckHurtboxOffset = new Vector2(-4f, -6f);
            Vector2 newFeatherHitboxOffset = new Vector2(-4f, -10f);
            Vector2 newFeatherHurtboxOffset = new Vector2(-3f, -9f);

            try
            {
                newHitboxSize = new Vector2(float.Parse(NormalHitboxSize[0]), float.Parse(NormalHitboxSize[1]));
                newHurtboxSize = new Vector2(float.Parse(NormalHurtboxSize[0]), float.Parse(NormalHurtboxSize[1]));
                newDuckHitboxSize = new Vector2(float.Parse(DuckHitboxSize[0]), float.Parse(DuckHitboxSize[1]));
                newDuckHurtboxSize = new Vector2(float.Parse(DuckHurtboxSize[0]), float.Parse(DuckHurtboxSize[1]));
                newFeatherHitboxSize = new Vector2(float.Parse(FeatherHitboxSize[0]), float.Parse(FeatherHitboxSize[1]));
                newFeatherHurtboxSize = new Vector2(float.Parse(FeatherHurtboxSize[0]), float.Parse(FeatherHurtboxSize[1]));

                newHitboxOffset = new Vector2(float.Parse(NormalHitboxOffset[0]), float.Parse(NormalHitboxOffset[1]));
                newHurtboxOffset = new Vector2(float.Parse(NormalHurtboxOffset[0]), float.Parse(NormalHurtboxOffset[1]));
                newDuckHitboxOffset = new Vector2(float.Parse(DuckHitboxOffset[0]), float.Parse(DuckHitboxOffset[1]));
                newDuckHurtboxOffset = new Vector2(float.Parse(DuckHurtboxOffset[0]), float.Parse(DuckHurtboxOffset[1]));
                newFeatherHitboxOffset = new Vector2(float.Parse(FeatherHitboxOffset[0]), float.Parse(FeatherHitboxOffset[1]));
                newFeatherHurtboxOffset = new Vector2(float.Parse(FeatherHurtboxOffset[0]), float.Parse(FeatherHurtboxOffset[1]));
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "CyrusSandbox", "one or more hitbox fields did not follow 'x,y' convention!");
                Logger.Log(LogLevel.Error, "CyrusSandbox", e.Message);
            }

            self.normalHitbox.Width = newHitboxSize.X;
            self.normalHitbox.Height = newHitboxSize.Y;

            self.duckHitbox.Width = newDuckHitboxSize.X;
            self.duckHitbox.Height = newDuckHitboxSize.Y;

            self.starFlyHitbox.Width = newFeatherHitboxSize.X;
            self.starFlyHitbox.Height = newFeatherHitboxSize.Y;

            self.normalHurtbox.Width = newHurtboxSize.X;
            self.normalHurtbox.Height = newHurtboxSize.Y;

            self.duckHurtbox.Width = newDuckHurtboxSize.X;
            self.duckHurtbox.Height = newDuckHurtboxSize.Y;

            self.starFlyHurtbox.Width = newFeatherHurtboxSize.X;
            self.starFlyHurtbox.Height = newFeatherHurtboxSize.Y;

            self.normalHitbox.Position.X = newHitboxOffset.X;
            self.normalHitbox.Position.Y = newHitboxOffset.Y;

            self.duckHitbox.Position.X = newDuckHitboxOffset.X;
            self.duckHitbox.Position.Y = newDuckHitboxOffset.Y;

            self.starFlyHitbox.Position.X = newFeatherHitboxOffset.X;
            self.starFlyHitbox.Position.Y = newFeatherHitboxOffset.Y;

            self.normalHurtbox.Position.X = newHurtboxOffset.X;
            self.normalHurtbox.Position.Y = newHurtboxOffset.Y;

            self.duckHurtbox.Position.X = newDuckHurtboxOffset.X;
            self.duckHurtbox.Position.Y = newDuckHurtboxOffset.Y;

            self.starFlyHurtbox.Position.X = newFeatherHurtboxOffset.X;
            self.starFlyHurtbox.Position.Y = newFeatherHurtboxOffset.Y;

            if (Settings.CircleMadelineOverride)
            {
                Draw.Circle(self.Position, 10, Color.Red, 5);
            }

        }

        private void HPLevelUpdate(On.Celeste.Level.orig_Update orig, Level self)
        {
            if (!(Settings.HealthOptions.HPSystemEnableOverride || Session.HPSystemEnabled)) { return; }
            if (self.Tracker.CountEntities<healthDisplay>() >= 1) { return; }
            self.Add(new healthDisplay());
        }

        [Tracked]
        public class BulletCollider : Component
        {

            private Collider collider;

            public Action<IWBTGBullet> OnCollide;

            public BulletCollider(Action<IWBTGBullet> onCollide, Collider collider = null)
            : base(active: false, visible: false)
            {
                this.collider = collider;
                OnCollide = onCollide;
            }

            public bool Check(IWBTGBullet bullet)
            {
                Collider collider = Entity.Collider;
                if (this.collider != null)
                {
                    Entity.Collider = this.collider;
                }
                bool result = bullet.CollideCheck(Entity);
                Entity.Collider = collider;
                return result;
            }
        }

        [Tracked]
        public class OnlyBlocksPlayer : Component
        {
            public OnlyBlocksPlayer() : base(false, false) { }

            public override void Added(Entity entity)
            {
                base.Added(entity);
                entity.Collidable = false;
            }
        }

        public static void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            var components = self.SceneAs<Level>().Tracker.GetComponents<OnlyBlocksPlayer>();
            foreach (OnlyBlocksPlayer component in components)
            {
                component.Entity.Collidable = true;
            }
            orig(self);
            foreach (OnlyBlocksPlayer component in components)
            {
                component.Entity.Collidable = false;
            }
        }

        public override void OnInputInitialize()
        {
            base.OnInputInitialize();
            joystickAim = new VirtualJoystick(true, new VirtualJoystick.PadRightStick(Input.Gamepad, 0.1f));
        }

        public override void OnInputDeregister()
        {
            base.OnInputDeregister();
            joystickAim?.Deregister();
        }

        public override void Load()
        {
            VanillaHooks.Load();
            On.Celeste.Player.Update += Player_Update;
            On.Celeste.Player.Render += GunPlayerRender;
            On.Celeste.Player.Update += GunPlayerUpdate;
            On.Celeste.Level.Render += GunLevelRender;
            On.Celeste.Player.Update += HitboxPlayerUpdate;
            On.Celeste.Level.Update += HPLevelUpdate;
            // TODO: apply any hooks that should always be active
        }

        public override void Unload()
        {
            VanillaHooks.Unload();
            On.Celeste.Player.Update -= Player_Update;
            On.Celeste.Player.Render -= GunPlayerRender;
            On.Celeste.Player.Update -= GunPlayerUpdate;
            On.Celeste.Level.Render -= GunLevelRender;
            On.Celeste.Player.Update -= HitboxPlayerUpdate;
            On.Celeste.Level.Update -= HPLevelUpdate;
            // TODO: unapply any hooks applied in Load()
        }
    }
}