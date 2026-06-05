using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using NLua;

namespace Celeste.Mod.AletrisSandbox.Entities;

[Tracked]
[CustomEntity("AletrisSandbox/avoidanceController")]
public class AvoidanceController : Entity
{

    public string luaData;

    Vector2 StoredOffset;

    public Entity[] followingPlayer;
    public LuaTable[] trackedEntities;
    public Lua lua;
    public float timer;

    public bool debugMode;
    public bool debugSetTEnabled;
    HashSet<Keys> debugKeysConsumed = new HashSet<Keys>();
    string debugTValue = "";

    public AvoidanceController(EntityData data, Vector2 offset)
        : base(data.Position + offset) // conversion from room to world position
    {

        luaData = data.String("luaData", "");
        debugMode = data.Bool("debugMode", false);

        StoredOffset = offset;
        lua = new Lua();
    }

    [Tracked]
    public class ACMovement() : Component(true, false)
    {
        public int? followPlayerSpd { get; set; }

        public int Lifetime;
        public Vector2 speed;
        public int timer;
        public bool rebound;

        public ACMovement(Vector2 vel, int lifetime, int? followPlayerSpd, bool reboun) : this()
        {
            Lifetime = (lifetime > 0) ? lifetime : -1;
            this.followPlayerSpd = followPlayerSpd;
            speed = vel;
            timer = 0;
            rebound = reboun;
        }

        public override void Update()
        {
            if (rebound)
            {
                ActorMoveH(speed.X * Engine.DeltaTime, OnCollideH, null);
                ActorMoveV(speed.Y * Engine.DeltaTime, OnCollideV, null);
            }
            else
            {
                ActorNaiveMove(speed * Engine.DeltaTime);
            }

            Player plr = SceneAs<Level>()?.Tracker.GetEntity<Player>();

            if (plr != null && followPlayerSpd.HasValue && followPlayerSpd.Value != 0)
            {
                Vector2 dir = Entity.Position - plr.Position;
                if (dir != Vector2.Zero)
                    speed = -Vector2.Normalize(dir) * followPlayerSpd.Value;
            }

            if (Lifetime < 0) { return; }
            if (timer < Lifetime)
            { timer++; }
            else
            {
                RemoveSelf();
                Entity.RemoveSelf();
            }
            base.Update();
        }

        private void OnCollideH(CollisionData data)
        {
            if (data.Hit != Entity)
            { speed.X = -speed.X;
            }
        }

        private void OnCollideV(CollisionData data)
        {
            if (data.Hit != Entity)
            {
                speed.Y = -speed.Y;
            }
        }

        public Vector2 movementCounter;
        public bool IgnoreJumpThrus;

        public bool ActorMoveH(float moveH, Collision onCollide = null, Solid pusher = null)
        {
            movementCounter.X += moveH;
            movementCounter.X -= moveH;
            return ActorMoveHExact(moveH, onCollide, pusher);
        }

        public bool ActorMoveV(float moveV, Collision onCollide = null, Solid pusher = null)
        {
            movementCounter.Y += moveV;
            movementCounter.Y -= moveV;
            return ActorMoveVExact(moveV, onCollide, pusher);
        }

        public bool ActorMoveHExact(float moveH, Collision onCollide = null, Solid pusher = null)
        {
            if (moveH == 0f)
                return false;
            Vector2 target = Entity.Position + Vector2.UnitX * moveH;
            float dir = Math.Sign(moveH), moved = 0f;
            float remaining = Math.Abs(moveH);
            int steps = (int)remaining;
            float frac = remaining - steps;
            for (int i = 0; i < steps; i++)
            {
                if (Entity.CollideFirst<Solid>(Entity.Position + Vector2.UnitX * dir) is Solid hit)
                {
                    movementCounter.X = 0f;
                    onCollide?.Invoke( new CollisionData { Direction = Vector2.UnitX * dir, Moved = Vector2.UnitX * moved, TargetPosition = target, Hit = (Platform)hit, Pusher = pusher });
                    return true;
                }
                Entity.X += dir;
                moved += 1f;
            }
            if (frac > 0f)
            {
                if (Entity.CollideFirst<Solid>(Entity.Position + Vector2.UnitX * (dir * frac)) is Solid hit)
                {
                    movementCounter.X = 0f;
                    onCollide?.Invoke(new CollisionData { Direction = Vector2.UnitX * dir, Moved = Vector2.UnitX * moved, TargetPosition = target, Hit = (Platform)hit, Pusher = pusher });
                    return true;
                }
                Entity.X += dir * frac;
            }
            return false;
        }

        public bool ActorMoveVExact(float moveV, Collision onCollide = null, Solid pusher = null)
        {
            if (moveV == 0f)
                return false;

            Vector2 target = Entity.Position + Vector2.UnitY * moveV;
            float dir = Math.Sign(moveV),
                  moved = 0f;
            float remaining = Math.Abs(moveV);
            int steps = (int)remaining;
            float frac = remaining - steps;

            for (int i = 0; i < steps; i++)
            {
                if (Entity.CollideFirst<Solid>(Entity.Position + Vector2.UnitY * dir) is Solid hit)
                {
                    movementCounter.Y = 0f;
                    onCollide?.Invoke( new CollisionData { Direction = Vector2.UnitY * dir, Moved = Vector2.UnitY * moved, TargetPosition = target, Hit = (Platform)hit, Pusher = pusher });
                    return true;
                }
                Entity.Y += dir;
                moved += 1f;
            }

            if (frac > 0f)
            {
                if (Entity.CollideFirst<Solid>(Entity.Position + Vector2.UnitY * (dir * frac)) is Solid hit)
                {
                    movementCounter.Y = 0f;
                    onCollide?.Invoke( new CollisionData { Direction = Vector2.UnitY * dir, Moved = Vector2.UnitY * moved, TargetPosition = target, Hit = (Platform)hit, Pusher = pusher });
                    return true;
                }
                Entity.Y += dir * frac;
            }

            return false;
        }

        public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null)
        {
            ActorMoveToX(Calc.Approach(Entity.Position.X, targetX, maxAmount), onCollide);
        }

        public void ActorMoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
        {
            ActorMoveToY(Calc.Approach(Entity.Position.Y, targetY, maxAmount), onCollide);
        }

        public void ActorMoveToX(float toX, Collision onCollide = null)
        {
            ActorMoveH(toX - Entity.Position.X - movementCounter.X, onCollide);
        }

        public void ActorMoveToY(float toY, Collision onCollide = null)
        {
            ActorMoveV(toY - Entity.Position.Y - movementCounter.Y, onCollide);
        }


        public void ActorNaiveMove(Vector2 amount)
        {
            movementCounter += amount;
            float x = movementCounter.X;
            float y = movementCounter.Y;
            Entity.Position += new Vector2(x, y);
            movementCounter -= new Vector2(x, y);
        }

    }

    #region HelperFunctions

    public LuaTable SpawnObj(string sid , LuaTable position, LuaTable velocity, int lifeTimer, bool rebound, int followPlayerSpd, LuaTable param) // pos,vel = { x = 0, y = 0 }. param = dictionary
    {
        Level scene = SceneAs<Level>();

        Vector2 convertedPos;
        Vector2 convertedVel;
        Dictionary<string, object> convertedParams = new();

        float px = 0f, py = 0f;

        if (position != null)
        {
            px = Convert.ToSingle(position["x"] ?? 0);
            py = Convert.ToSingle(position["y"] ?? 0);
        }
        convertedPos = new Vector2(px, py);

        float vx = 0f, vy = 0f;

        if (velocity != null)
        {
            vx = Convert.ToSingle(velocity["x"] ?? 0);
            vy = Convert.ToSingle(velocity["y"] ?? 0);
        }
        convertedVel = new Vector2(vx, vy);

        if (param != null)
        {
            foreach (var key in param.Keys)
            {
                convertedParams[key.ToString() ?? throw new InvalidOperationException()] = param[key];
            }
        }

        if (Level.EntityLoaders.TryGetValue(sid, out Level.EntityLoader loader))
        {
            Entity Actent = loader(scene,
                                   scene.Session.LevelData,
                                   StoredOffset,
                new EntityData
                {
                    ID = 0,
                    Name = sid,
                    Level = scene.Session.LevelData,
                    Position = convertedPos,
                    Origin = new Vector2(0, 0), // not sure if this is actually it
                    Width = 0,
                    Height = 0,
                    Nodes = new Vector2[] { },
                    Values = convertedParams
                }); // ¯\_(ツ)_/¯
            ACMovement ac = new ACMovement(convertedVel,lifeTimer,followPlayerSpd,rebound);

            Actent.Add(ac);
            Scene.Add(Actent);

            LuaTable tbl = CreateTable();
            tbl["tag"] = Actent.Tag;
            tbl["Entity"] = Actent;
            tbl["Velocity"] = velocity;
            tbl["FollowPlayerSpd"] = followPlayerSpd;
            trackedEntities.Append(tbl);

            return tbl;
            //Logger.Verbose(nameof(AletrisSandboxModule), "spawned " + sid + " at " + Actent.Position + " with velocity " + convertedVel);
        }
        else
        {
            Logger.Warn(nameof(AletrisSandboxModule),"Couldn't find entity loader for entity " + sid);
            return null;
        }

    }

    public void AdjustMovement(Entity obj, LuaTable velTbl)
    {
        double xd = (double)velTbl["x"];
        double yd = (double)velTbl["y"];
        var vec = new Vector2((float)xd, (float)yd);
        var movement = obj.Components.Get<ACMovement>();
        if (movement != null)
            movement.speed = vec;
    }

    private LuaTable CreateTable()
    {
        return (LuaTable)lua.DoString("return {}")[0];
    }

    public LuaTable AngleToCartesian(double angleDegrees, double magnitude)
    {
        double rad = angleDegrees * Math.PI / 180.0;
        var tbl = CreateTable();
        tbl["x"] = magnitude * Math.Cos(rad);
        tbl["y"] = magnitude * Math.Sin(rad);
        return tbl;
    }

    public double CartesianToAngle(LuaTable velocity)
    {
        double x = (double)velocity["x"];
        double y = (double)velocity["y"];
        double rad = Math.Atan2(y, x);

        return rad * 180.0 / Math.PI; // convert to degrees
    }

    public void setFlag(string flag, bool state)
    {
        if (SceneAs<Level>() == null)
            return;
        SceneAs<Level>().Session.SetFlag(flag,state);
    }

    public void cleanObj(int SearchTag)
    {
        for (int i = trackedEntities.Count() - 1; i >= 0; i--)
        {
            var tbl = trackedEntities[i];

            if (tbl == null)
                continue;

            var tagObj = tbl["tag"];

            if (tagObj == null)
                continue;

            int t = Convert.ToInt32(tagObj);

            if (t == SearchTag)
            {
                trackedEntities[i] = null;
            }
        }
    }

    #endregion

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        string luaCode = Utils.ReadModAsset(luaData);

        lua["seed"] = new Random(SceneAs<Level>().Session.LevelData.LoadSeed);

        lua.RegisterFunction("SpawnObj", this, GetType().GetMethod("SpawnObj"));
        lua.RegisterFunction("AngleToCartesian", this, GetType().GetMethod("AngleToCartesian"));
        lua.RegisterFunction("CartesianToAngle", this, GetType().GetMethod("CartesianToAngle"));
        lua.RegisterFunction("setFlag", this, GetType().GetMethod("setFlag"));
        lua.RegisterFunction("AdjustMovement", this, GetType().GetMethod("AdjustMovement"));

        //Logger.Verbose(nameof(AletrisSandboxModule), "luaCode:" + luaCode);
        Logger.Verbose(nameof(AletrisSandboxModule), "running luaData!");

        try
        {
            lua.DoString(luaCode);
        }
        catch (Exception ex)
        {
            Logger.Verbose(nameof(AletrisSandboxModule), "Lua error: " + ex.Message);
            Logger.Verbose(nameof(AletrisSandboxModule), "Inner: " + ex.InnerException);
            Logger.Verbose(nameof(AletrisSandboxModule), ex.StackTrace);
        }
    }

    public override void Render()
    {
        if (debugSetTEnabled)
        {
            Level scene = SceneAs<Level>();
            Player plr = scene?.Tracker.GetEntity<Player>();
            if (plr != null)
                ActiveFont.DrawOutline("T:" + timer, plr.Position, new(0.5f, 2f), new(0.25f, 0.25f), Color.Black, 1, Color.White);
        }
        base.Render();
    }

    public override void Update()
    {
        base.Update();

        Level scene = SceneAs<Level>();
        Player plr = scene?.Tracker.GetEntity<Player>();

        if (Input.MenuJournal.Pressed)
        {
            Logger.Info(nameof(AletrisSandboxModule),"Current Time: " + timer);
            debugSetTEnabled = !debugSetTEnabled;
        }

        var ks = Keyboard.GetState();

        Keys[] numberKeys =
        {
            Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
            Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0
        };

        foreach (var k in numberKeys)
        {
            if (debugSetTEnabled)
            {
                if (ks.IsKeyDown(k) && !debugKeysConsumed.Contains(k))
                {
                    int digit = k.ToString()[^1] - '0';
                    debugTValue += digit;
                    debugKeysConsumed.Add(k);

                    if (int.TryParse(debugTValue, out int asdf))
                    {
                        timer = asdf;
                    }
                }
                else if (ks.IsKeyUp(k) && debugKeysConsumed.Contains(k))
                {
                    debugKeysConsumed.Remove(k);
                }
            }
        }

        if (!debugSetTEnabled) { debugTValue = ""; }

        lua["playerPos"] = plr == null ? Vector2.Zero : plr.Position;

        if (scene.Paused) { return; }

        try
        {
            var fn = lua["update"] as LuaFunction;
            if (fn == null)
            {
                Logger.Error(nameof(AletrisSandboxModule), "function update(t) does not exist");
            }
            else
            {
                fn!.Call(timer);
                timer += 1;
            }

        } catch (Exception ex) {
            Logger.Error(nameof(AletrisSandboxModule), ex.Message);
            Logger.Verbose(nameof(AletrisSandboxModule), ex.StackTrace);
            Logger.Verbose(nameof(AletrisSandboxModule), ex.InnerException?.ToString());
            RemoveSelf();
        }
    }
}