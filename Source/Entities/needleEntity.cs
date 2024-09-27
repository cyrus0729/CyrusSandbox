using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.CyrusSandbox.Entities
{
    [CustomEntity("CyrusSandbox/NeedleUp = BigUp",
                "CyrusSandbox/NeedleDown = BigDown",
                "CyrusSandbox/NeedleLeft = BigLeft",
                "CyrusSandbox/NeedleRight = BigRight",
                "CyrusSandbox/miniNeedleUp = MiniUp",
                "CyrusSandbox/miniNeedleDown = MiniDown",
                "CyrusSandbox/miniNeedleLeft = MiniLeft",
                "CyrusSandbox/miniNeedleRight = MiniRight")]
    public class NeedleEntity : Entity
    {
        public enum HitboxType { NeedleHelper, Forgiving, Unforgiving };

        public bool Attached;

        private PlayerCollider pc;

        private Sprite sprite;
        private EntityData data;
        private Vector2 offset;
        private ColliderList colliderList;
        private bool isMini;
        public NeedleEntity(EntityData data, Vector2 offset, ColliderList colliderList, Sprite sprite, float rotation) : base(data.Position + offset)
        {
            sprite.Rotation = rotation;
            isMini = data.Bool("isMini", false);
            this.data = data;
            this.offset = offset;
            this.colliderList = colliderList;
            pc = new PlayerCollider(OnCollide);
            Logger.Log("cy", "does this thing even work?");
        }

        public static Entity BigUp(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            Logger.Log("cy", "does this thing even work 2?");
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(16f, 3f, -8f, -8f),
                        new Hitbox(12f, 2f, -6f, -6f),
                        new Hitbox(10f, 2f, -5f, -4f),
                        new Hitbox(8f, 2f, -4f, -2f),
                        new Hitbox(6f, 2f, -3f, -0f),
                        new Hitbox(4f, 2f, -2f, 2f),
                        new Hitbox(2f, 2f, -1f, 4f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 0f);

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(14f, 2f, -7f, 6f),
                        new Hitbox(12f, 2f, -6f, 4f),
                        new Hitbox(10f, 2f, -5f, 2f),
                        new Hitbox(8f, 2f, -4f, 0f),
                        new Hitbox(6f, 2f, -3f, -2f),
                        new Hitbox(4f, 2f, -2f, -4f),
                        new Hitbox(2f, 2f, -1f, -6f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 0f);

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(16f, 2f, -8f, 6f),
                        new Hitbox(14f, 2f, -7f, 4f),
                        new Hitbox(12f, 2f, -6f, 2f),
                        new Hitbox(10f, 2f, -5f, 0f),
                        new Hitbox(8f, 2f, -4f, -2f),
                        new Hitbox(6f, 2f, -3f, -4f),
                        new Hitbox(4f, 2f, -2f, -6f),
                        new Hitbox(2f, 2f, -1f, -8f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 0f);

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 0f);
            }
        }
        public static Entity BigDown(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(16f, 3f, -8f, -8f),
                        new Hitbox(12f, 2f, -6f, -6f),
                        new Hitbox(10f, 2f, -5f, -4f),
                        new Hitbox(8f, 2f, -4f, -2f),
                        new Hitbox(6f, 2f, -3f, -0f),
                        new Hitbox(4f, 2f, -2f, 2f),
                        new Hitbox(2f, 2f, -1f, 4f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 180f.ToRad());

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(14f, 2f, -7f, -8f),
                        new Hitbox(12f, 2f, -6f, -6f),
                        new Hitbox(10f, 2f, -5f, -4f),
                        new Hitbox(8f, 2f, -4f, -2f),
                        new Hitbox(6f, 2f, -3f, -0f),
                        new Hitbox(4f, 2f, -2f, 2f),
                        new Hitbox(2f, 2f, -1f, 4f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 180f.ToRad());

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(16f, 2f, -8f, -8f),
                        new Hitbox(14f, 2f, -7f, -6f),
                        new Hitbox(12f, 2f, -6f, -4f),
                        new Hitbox(10f, 2f, -5f, -2f),
                        new Hitbox(8f, 2f, -4f, -0f),
                        new Hitbox(6f, 2f, -3f, 2f),
                        new Hitbox(4f, 2f, -2f, 4f),
                        new Hitbox(2f, 2f, -1f, 6f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 180f.ToRad());

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 180f.ToRad());
            }
        }
        public static Entity BigLeft(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(3f, 16f, 6f, -8f),
                        new Hitbox(2f, 12f, 4f, -6f),
                        new Hitbox(2f, 10f, 2f, -5f),
                        new Hitbox(2f, 8f, 0f, -4f),
                        new Hitbox(2f, 6f, -2f, -3f),
                        new Hitbox(2f, 4f, -4f, -2f),
                        new Hitbox(2f, 2f, -6f, -1f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 270f.ToRad());

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 14f, 6f, -7f),
                        new Hitbox(2f, 12f, 4f, -6f),
                        new Hitbox(2f, 10f, 2f, -5f),
                        new Hitbox(2f, 8f, 0f, -4f),
                        new Hitbox(2f, 6f, -2f, -3f),
                        new Hitbox(2f, 4f, -4f, -2f),
                        new Hitbox(2f, 2f, -6f, -1f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 270f.ToRad());

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 16f, 6f, -8f),
                        new Hitbox(2f, 14f, 4f, -7f),
                        new Hitbox(2f, 12f, 2f, -6f),
                        new Hitbox(2f, 10f, 0f, -5f),
                        new Hitbox(2f, 8f, -2f, -4f),
                        new Hitbox(2f, 6f, -4f, -3f),
                        new Hitbox(2f, 4f, -6f, -2f),
                        new Hitbox(2f, 2f, -8f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 270f.ToRad());

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 270f.ToRad());
            }
        }
        public static Entity BigRight(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(3f, 16f, -8f, -8f),
                        new Hitbox(2f, 12f, -6f, -6f),
                        new Hitbox(2f, 10f, -4f, -5f),
                        new Hitbox(2f, 8f, -2f, -4f),
                        new Hitbox(2f, 6f, -0f, -3f),
                        new Hitbox(2f, 4f, 2f, -2f),
                        new Hitbox(2f, 2f, 4f, -1f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 90f.ToRad());

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 14f, -8f, -7f),
                        new Hitbox(2f, 12f, -6f, -6f),
                        new Hitbox(2f, 10f, -4f, -5f),
                        new Hitbox(2f, 8f, -2f, -4f),
                        new Hitbox(2f, 6f, -0f, -3f),
                        new Hitbox(2f, 4f, 2f, -2f),
                        new Hitbox(2f, 2f, 4f, -1f),
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 90f.ToRad());

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 16f, -8f, -8f),
                        new Hitbox(2f, 14f, -6f, -7f),
                        new Hitbox(2f, 12f, -4f, -6f),
                        new Hitbox(2f, 10f, -2f, -5f),
                        new Hitbox(2f, 8f, -0f, -4f),
                        new Hitbox(2f, 6f, 2f, -3f),
                        new Hitbox(2f, 4f, 4f, -2f),
                        new Hitbox(2f, 2f, 6f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 90f.ToRad());

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "needleEntity")), 90f.ToRad());
            }
        }

        public static Entity MiniUp(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(8f, 2f, -4f, 2f),
                        new Hitbox(4f, 2f, -2f, 0f),
                        new Hitbox(2f, 2f, -1f, -2f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 0f);

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(6f, 2f, -3f, 2f),
                        new Hitbox(4f, 2f, -2f, 0f),
                        new Hitbox(2f, 2f, -1f, -2f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 0f);

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(8f, 2f, -4f, 2f),
                        new Hitbox(6f, 2f, -3f, 0f),
                        new Hitbox(4f, 2f, -2f, -2f),
                        new Hitbox(2f, 2f, -1f, -4f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 0f);

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 0f);
            }
        }
        public static Entity MiniDown(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(8f, 2f, -4f, -4f),
                        new Hitbox(4f, 2f, -2f, -2f),
                        new Hitbox(2f, 2f, -1f, 0f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 180f.ToRad());

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(6f, 2f, -3f, -4f),
                        new Hitbox(4f, 2f, -2f, -2f),
                        new Hitbox(2f, 2f, -1f, 0f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 180f.ToRad());

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(8f, 2f, -4f, -4f),
                        new Hitbox(6f, 2f, -3f, -2f),
                        new Hitbox(4f, 2f, -2f, 0f),
                        new Hitbox(2f, 2f, -1f, 2f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 180f.ToRad());

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)])
                        , GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 180f.ToRad());
            }
        }
        public static Entity MiniLeft(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 8f, 2f, -4f),
                        new Hitbox(2f, 4f, 0f, -2f),
                        new Hitbox(2f, 2f, -2f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 270f.ToRad());

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 6f, 2f, -3f),
                        new Hitbox(2f, 4f, 0f, -2f),
                        new Hitbox(2f, 2f, -2f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 270f.ToRad());

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 8f, 2f, -4f),
                        new Hitbox(2f, 6f, 0f, -3f),
                        new Hitbox(2f, 4f, -2f, -2f),
                        new Hitbox(2f, 2f, -4f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 270f.ToRad());

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 270f.ToRad());
            }
        }
        public static Entity MiniRight(Level level, LevelData leveldata, Vector2 offset, EntityData data)
        {
            HitboxType hitboxes = data.Enum("hitbox", HitboxType.NeedleHelper);
            switch (hitboxes)
            {
                case HitboxType.NeedleHelper:
                    return new NeedleEntity(data, offset, new ColliderList([
                    new Hitbox(2f, 8f, -4f, -4f),
                        new Hitbox(2f, 4f, -2f, -2f),
                        new Hitbox(2f, 2f, 0f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 90f.ToRad());

                case HitboxType.Forgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 6f, -4f, -3f),
                        new Hitbox(2f, 4f, -2f, -2f),
                        new Hitbox(2f, 2f, 0f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 90f.ToRad());

                case HitboxType.Unforgiving:
                    return new NeedleEntity(data, offset, new ColliderList([
                        new Hitbox(2f, 8f, -4f, -4f),
                        new Hitbox(2f, 6f, -2f, -3f),
                        new Hitbox(2f, 4f, 0f, -2f),
                        new Hitbox(2f, 2f, 2f, -1f)
                    ]), GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 90f.ToRad());

                default:
                    Logger.Log("CyrusSandbox", "Returned default");
                    return new NeedleEntity(data, offset,
                        new ColliderList([new Hitbox(0f, 0f, 0f, 0f)]),
                        GFX.SpriteBank.Create(data.Attr("Sprite", "miniNeedleEntity")), 90f.ToRad());
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnCollide(Player player)
        {
            if (OnCollide != null)
            {
                player.Die((player.Center - Center).SafeNormalize());
            }
        }

    }
}