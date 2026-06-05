using Celeste.Mod.AletrisSandbox.Entities;
using Celeste.Mod.AletrisSandbox.GunSupport;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using Celeste.Mod.AletrisSandbox.Miscstuff;
using NLua;

namespace Celeste.Mod.AletrisSandbox;

public class AletrisSandboxModule : EverestModule
{
    public static AletrisSandboxModule Instance { get; private set; }

    public override Type SettingsType => typeof(AletrisSandboxModuleSettings);
    public static AletrisSandboxModuleSettings Settings => (AletrisSandboxModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(AletrisSandboxModuleSession);
    public static AletrisSandboxModuleSession Session => (AletrisSandboxModuleSession)Instance._Session;

    public AletrisSandboxModule()
    {
        Instance = this;
    #if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(AletrisSandboxModule), LogLevel.Verbose);
    #else

        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(AletrisSandboxModule), LogLevel.Info);
    #endif
    }
    static MouseState State => Mouse.GetState();
    static Vector2 MouseCursorPos => Vector2.Transform(new(State.X, State.Y), Matrix.Invert(Engine.ScreenMatrix));

    public override void LoadContent(bool firstLoad)
    {
    }

    void EverestExitMethod(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
    {
        Settings.IWBTOptions.IWBTGGunEnableOverride = Session.oldEnabledConfig;
        Settings.HealthOptions.HPSystemEnableOverride = Session.oldEnabledConfig;
    }

    void EverestEnterMethod(Session session, bool fromSaveData)
    {
        Session.oldEnabledConfig = Settings.IWBTOptions.IWBTGGunEnableOverride;
        Session.oldEnabledConfig = Settings.HealthOptions.HPSystemEnableOverride;
    }

    public static Vector2 PlayerPosScreenSpace(Actor self)
    {
        if (self?.Scene is Level level && level.Camera != null)
        {
            return self.Center - level.Camera.Position;
        }

        // Return a default Vector2 when nulls are encountered
        return Vector2.Zero;
    }

    public static Vector2 ToCursor(Actor player, Vector2 cursorPos, bool normalize = true)
        => normalize ? Vector2.Normalize(cursorPos / 6f - PlayerPosScreenSpace(player)): cursorPos / 6f - PlayerPosScreenSpace(player);

    static float ToRotation(Vector2 vector)
        => (float)Math.Atan2(vector.Y, vector.X);

    static void GunPlayerRender(On.Celeste.Player.orig_Render orig, Player self)
    {
        orig(self);

        if (!(Settings.IWBTOptions.IWBTGGunEnableOverride || Session.IWBTGGunEnabled) ||
            !(Settings.IWBTOptions.IWBTGGunVisibleOverride || Session.IWBTGGunVisible))
            return;

        var flip = SpriteEffects.None;
        Vector2 offset;

        var gunVector = ToCursor(self, MouseCursorPos);

        float gunRotation;

        if (Settings.IWBTOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled)
        {
            self.Facing = (Facings)Math.Sign(ToCursor(self, MouseCursorPos).X);
            if (self.Facing == 0)
                self.Facing = Facings.Right;
            gunRotation = ToRotation(gunVector);

            if (self.Facing == Facings.Left)
                flip = SpriteEffects.FlipVertically;

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

        MTexture gunTexture = GFX.SpriteBank.Create("IWBTGun").Texture;
        gunTexture.DrawJustified(
            self.Center,
            offset,
            Color.White,
            1f,
            gunRotation,
            flip);
    }

    static void CrosshairRender(On.Celeste.Level.orig_Render orig, Level self)
    {
        orig(self);

        if (!Settings.IWBTOptions.IWBTGGunAimOverride || !Session.IWBTGGunMouseAimEnabled) { return;} // if iwbtg mouse aim is enabled, keep drawing

        Draw.SpriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);

        var aimColor = self.Tracker.CountEntities<IWBTGBullet>() >= Session.MaxBullets ? Color.Red : Color.White;
        Texture2D crossTexture = GFX.SpriteBank.Create("IWBTCross").Texture.Texture.Texture;
        Draw.SpriteBatch.Draw(crossTexture, MouseCursorPos, null, aimColor, 0f, new(crossTexture.Width / 2f, crossTexture.Height / 2f), 4f, 0, 0f);
        Draw.SpriteBatch.End();
    }

    static void GunPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        if (!(Settings.IWBTOptions.IWBTGGunEnableOverride || Session.IWBTGGunEnabled))
            return;

        var components = self.SceneAs<Level>().Tracker.GetComponents<OnlyBlocksPlayer>();
        foreach (var component in components)
            component.Entity.Collidable = true;

        foreach (var component in components)
            component.Entity.Collidable = false;

        /*if (self.Scene != null || self.Scene.TimeActive <= 0f || (TalkComponent.PlayerOver != null && Input.Talk.Pressed))
        {
            Logger.Log(LogLevel.Info,"alsandbox","houijgwsrdf");
            orig(self);
            return;
        }*/

        var turnOffset = self.Facing == Facings.Left ? -20f : 0f;
        var mouseposition = new Vector2(self.Center.X, self.Center.Y - 4.5f);
        var position = new Vector2(self.Center.X + 7f + turnOffset, self.Center.Y - 4.5f);

        bool boolToCheck;
        var isAimOverride = Settings.IWBTOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled;

        if (Settings.IWBTOptions.IWBTGGunAutoFireOverride || Session.IWBTGGunAutofireEnabled)
            boolToCheck = Settings.BulletFirekey.Check || (isAimOverride ? MInput.Mouse.CheckLeftButton : false);
        else
            boolToCheck = Settings.BulletFirekey.Pressed || (isAimOverride ? MInput.Mouse.PressedLeftButton : false);

        if (boolToCheck && self.Scene.Tracker.CountEntities<IWBTGBullet>() < Session.MaxBullets)
        {
            if (Settings.IWBTOptions.IWBTGGunAimOverride || Session.IWBTGGunMouseAimEnabled)
            {
                _ = new IWBTGBullet(mouseposition, ToCursor(self, MouseCursorPos) * 5f, self);
            }
            else
            {
                _ = new IWBTGBullet(position, (self.Facing == Facings.Left ? new(-1, 0) : new Vector2(1, 0)) * 5f, self);
            }

            if (Settings.IWBTOptions.GunSound > 0)
                Audio.Play("event:/aletris_sandbox/fire" + Settings.IWBTOptions.GunSound, "fade", 0f);
        }
    }

    static void HitboxPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        orig(self);

        if (!Settings.HitboxOptions.PlayerHitboxEnableOverride)
            return;

        // god is real and i killed them

        try
        {
            self.normalHitbox.Width = (float)Settings.HitboxSizeOptions.NormalHitboxSizeX;
            self.normalHitbox.Height = (float)Settings.HitboxSizeOptions.NormalHitboxSizeY;
            self.duckHitbox.Width = (float)Settings.HitboxSizeOptions.DuckHitboxSizeX;
            self.duckHitbox.Height = (float)Settings.HitboxSizeOptions.DuckHitboxSizeY;
            self.starFlyHitbox.Width = (float)Settings.HitboxSizeOptions.FeatherHitboxSizeX;
            self.starFlyHitbox.Height = (float)Settings.HitboxSizeOptions.FeatherHitboxSizeY;
            self.normalHurtbox.Width = (float)Settings.HitboxSizeOptions.NormalHurtboxSizeX;
            self.normalHurtbox.Height = (float)Settings.HitboxSizeOptions.NormalHurtboxSizeY;
            self.duckHurtbox.Width = (float)Settings.HitboxSizeOptions.DuckHurtboxSizeX;
            self.duckHurtbox.Height = (float)Settings.HitboxSizeOptions.DuckHurtboxSizeY;
            self.starFlyHurtbox.Width = (float)Settings.HitboxSizeOptions.FeatherHurtboxSizeX;
            self.starFlyHurtbox.Height = (float)Settings.HitboxSizeOptions.FeatherHurtboxSizeY;

            self.normalHitbox.Position.X = (float)Settings.HitboxOffsetOptions.NormalHitboxOffsetX;
            self.normalHitbox.Position.Y = (float)Settings.HitboxOffsetOptions.NormalHitboxOffsetY;
            self.duckHitbox.Position.X = (float)Settings.HitboxOffsetOptions.DuckHitboxOffsetX;
            self.duckHitbox.Position.Y = (float)Settings.HitboxOffsetOptions.DuckHitboxOffsetY;
            self.starFlyHitbox.Position.X = (float)Settings.HitboxOffsetOptions.FeatherHitboxOffsetX;
            self.starFlyHitbox.Position.Y = (float)Settings.HitboxOffsetOptions.FeatherHitboxOffsetY;
            self.normalHurtbox.Position.X = (float)Settings.HitboxOffsetOptions.NormalHurtboxOffsetX;
            self.normalHurtbox.Position.Y = (float)Settings.HitboxOffsetOptions.NormalHurtboxOffsetY;
            self.duckHurtbox.Position.X = (float)Settings.HitboxOffsetOptions.DuckHurtboxOffsetX;
            self.duckHurtbox.Position.Y = (float)Settings.HitboxOffsetOptions.DuckHurtboxOffsetY;
            self.starFlyHurtbox.Position.X = (float)Settings.HitboxOffsetOptions.FeatherHurtboxOffsetX;
            self.starFlyHurtbox.Position.Y = (float)Settings.HitboxOffsetOptions.FeatherHurtboxOffsetY;
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error, nameof(AletrisSandboxModule), "Error assigning hitbox or hurtbox sizes/offsets!");
            Logger.Log(LogLevel.Error, nameof(AletrisSandboxModule), e.Message);
            self.normalHitbox.Width = 8f;
            self.normalHitbox.Height = 11f;
            self.duckHitbox.Width = 8f;
            self.duckHitbox.Height = 9f;
            self.starFlyHitbox.Width = 8f;
            self.starFlyHitbox.Height = 8f;
            self.normalHurtbox.Width = 8f;
            self.normalHurtbox.Height = 6f;
            self.duckHurtbox.Width = 8f;
            self.duckHurtbox.Height = 4f;
            self.starFlyHurtbox.Width = 6f;
            self.starFlyHurtbox.Height = 6f;

            self.normalHitbox.Position.X = -4f;
            self.normalHitbox.Position.Y = -11f;
            self.duckHitbox.Position.X = -4f;
            self.duckHitbox.Position.Y = -11f;
            self.starFlyHitbox.Position.X = -4f;
            self.starFlyHitbox.Position.Y = -10f;
            self.normalHurtbox.Position.X = -4f;
            self.normalHurtbox.Position.Y = -6f;
            self.duckHurtbox.Position.X = -4f;
            self.duckHurtbox.Position.Y = -6f;
            self.starFlyHurtbox.Position.X = -3f;
            self.starFlyHurtbox.Position.Y = -9f;
        }
    }

    static void HitboxPlayerRender(On.Celeste.Player.orig_Render orig, Player self)
    {
        orig(self);

        if (!(Settings.MiscelleaneousMenu.CircleMadelineOverride || Session.CircleMadelineEnabled))
            return;

        Draw.Circle(self.Center, Session.CircleMadelineEnabled ? Session.CircleMadelineRadius : Settings.MiscelleaneousMenu.CircleMadelineRadius, Color.Red, 25);
        Draw.HollowRect(self.Collider, Color.Red);
    }

    static void HPLevelUpdate(On.Celeste.Level.orig_Update orig, Level self)
    {
        orig(self);
        if (!(Settings.HealthOptions.HPSystemEnableOverride || Session.HPSystemEnabled))
            return;

        if (self.Tracker.CountEntities<healthDisplay>() >= 1)
            return;

        self.Add(new healthDisplay());
    }

    // thanks whoever made the static int part of the code
    static int Player_IWBTJumpUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
    {
        if (!(Settings.IWBTOptions.IWBTGJumpEnableOverride || Session.IWBTGJumpEnabled))
            return orig(self);

        if (self.varJumpTimer > 0)
        {
            if (!self.AutoJump && !Input.Jump.Check)
                self.Speed.Y -= Math.Min(self.Speed.Y, self.varJumpSpeed) * 0.35f;
        }

        return orig(self);
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

    public static void UnholdableBarrier_Player_Update(On.Celeste.Player.orig_Update orig, Player self)
    {
        var components = self.SceneAs<Level>().Tracker.GetComponents<OnlyBlocksPlayer>();

        foreach (var component in components)
        {
            if (component.Entity != null)
                component.Entity.Collidable = true;
            else
                Logger.Log(LogLevel.Info, nameof(AletrisSandboxModule), "{component.Entity} is null");
        }
        orig(self);

        foreach (var component in components)
        {
            if (component.Entity != null)
                component.Entity.Collidable = false;
            else
                Logger.Log(LogLevel.Info, nameof(AletrisSandboxModule), "{component.Entity} is null");
        }
    }

    //public override void OnInputInitialize()
    //{
    //    base.OnInputInitialize();
    //    joystickAim = new VirtualJoystick(true, new VirtualJoystick.PadRightStick(Input.Gamepad, 0.1f));
    //}

    //public override void OnInputDeregister()
    //{
    //    base.OnInputDeregister();
    //    joystickAim?.Deregister();
    //}

    public static void MouseControllerCheck(On.Celeste.Player.orig_Update orig, Player self)
    {
        if (self.SceneAs<Level>().Tracker.CountEntities<MouseController>() == 0)
        {
            Binds.MoveX.SetValue(0);
            Binds.MoveY.SetValue(0);
            Binds.Aim.SetDirection(Vector2.Zero);
            Binds.Feather.SetDirection(Vector2.Zero);
        }
        orig(self);
    }

    public static void OnPlrDie(Player player) // dumb dumb stupid dumb idiot
    {
        Logger.Verbose(nameof(AletrisSandboxModule), "calling onPlrDie..");

        AvoidanceController ac = player.SceneAs<Level>().Tracker.GetEntity<AvoidanceController>();

        if (ac != null)
        {
            try
            {
                var fn = ac.lua["onPlrDie"] as LuaFunction;

                if (fn == null)
                {
                    Logger.Verbose(nameof(AletrisSandboxModule), "function onPlrDie does not exist, you could add one!");

                    return;
                }
                fn!.Call(ac.timer);
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AletrisSandboxModule), ex.Message);
                Logger.Verbose(nameof(AletrisSandboxModule), ex.StackTrace);
                Logger.Verbose(nameof(AletrisSandboxModule), ex.InnerException?.ToString());
                ac.RemoveSelf();
            }
        }
    }

    // somebody tell me how the FUCK do I add a lib reference again

	private static void ResetInput()
	{
		Binds.MoveX.SetNeutral();
        Binds.MoveY.SetNeutral();
        Binds.GliderMoveY.SetNeutral();

        Binds.Aim.SetNeutral();
        Binds.Feather.SetNeutral();
        Binds.Jump.Release();
        Binds.Dash.Release();
        Binds.Grab.Release();
        Binds.Talk.Release();
        Binds.CrouchDash.Release();
	}

	public override void OnInputInitialize()
	{
		base.OnInputInitialize();

		Input.MoveX.Nodes.Add(Binds.MoveX);
		Input.MoveY.Nodes.Add(Binds.MoveY);
		Input.GliderMoveY.Nodes.Add(Binds.GliderMoveY);

		Input.Aim.Nodes.Add(Binds.Aim);
		Input.Feather.Nodes.Add(Binds.Feather);
		Input.Jump.Nodes.Add(Binds.Jump);
		Input.Dash.Nodes.Add(Binds.Dash);
		Input.Grab.Nodes.Add(Binds.Grab);
		Input.Talk.Nodes.Add(Binds.Talk);
		Input.CrouchDash.Nodes.Add(Binds.CrouchDash);
	}

	public override void OnInputDeregister()
	{
		base.OnInputDeregister();
		Input.MoveX?.Nodes.Remove(Binds.MoveX);
		Input.MoveY?.Nodes.Remove(Binds.MoveY);
		Input.GliderMoveY?.Nodes.Remove(Binds.GliderMoveY);

		Input.Aim?.Nodes.Remove(Binds.Aim);
		Input.Feather?.Nodes.Remove(Binds.Feather);
		Input.Jump?.Nodes.Remove(Binds.Jump);
		Input.Dash?.Nodes.Remove(Binds.Dash);
		Input.Grab?.Nodes.Remove(Binds.Grab);
		Input.Talk?.Nodes.Remove(Binds.Talk);
		Input.CrouchDash?.Nodes.Remove(Binds.CrouchDash);
	}

    public override void Load()
    {
        VanillaHooks.Load();
        On.Celeste.Player.Render += GunPlayerRender;
        On.Celeste.Player.Update += GunPlayerUpdate;
        On.Celeste.Level.Render += CrosshairRender;
        On.Celeste.Player.Render += HitboxPlayerRender;
        On.Celeste.Player.Update += HitboxPlayerUpdate;
        On.Celeste.Level.Update += HPLevelUpdate;
        On.Celeste.Player.NormalUpdate += Player_IWBTJumpUpdate;
        On.Celeste.Player.Update += UnholdableBarrier_Player_Update;
        On.Celeste.Player.Update += MouseControllerCheck;
        Everest.Events.Player.OnDie += OnPlrDie;

        // TODO: apply any hooks that should always be active
    }

    public override void Unload()
    {
        VanillaHooks.Unload();
        On.Celeste.Player.Render -= GunPlayerRender;
        On.Celeste.Player.Update -= GunPlayerUpdate;
        On.Celeste.Level.Render -= CrosshairRender;
        On.Celeste.Player.Render -= HitboxPlayerRender;
        On.Celeste.Player.Update -= HitboxPlayerUpdate;
        On.Celeste.Level.Update -= HPLevelUpdate;
        On.Celeste.Player.NormalUpdate -= Player_IWBTJumpUpdate;
        On.Celeste.Player.Update -= UnholdableBarrier_Player_Update;
        On.Celeste.Player.Update -= MouseControllerCheck;
        Everest.Events.Player.OnDie -= OnPlrDie;

        // TODO: unapply any hooks applied in Load()
    }
}