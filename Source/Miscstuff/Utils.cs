namespace Celeste.Mod.AletrisSandbox;

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

public static class Utils
{
    public abstract class Wrapper(Vector2 position) : Entity(position)
    {
        // can somebody tell me how to add libs so i don't have to keep copying shit :gladeline:
        public List<Entity> FindTargets(Vector2 node, Vector2[] nodes, Vector2 nodeOffset, bool allEntities, string onlyType)
        {
            List<Entity> entities = new();

            //don't look for entity if allEntities and type is set
            Entity targetEntity = null;

            if (!allEntities || onlyType?.Length == 0)
                targetEntity = FindNearest(node, onlyType);

            if (allEntities)
            {
                foreach (var e in SceneAs<Level>().Entities)
                {
                    if ((onlyType?.Length == 0 && e.GetType() == targetEntity?.GetType()) || e.GetType().FullName == onlyType || e.GetType().Name == onlyType)
                        entities.Add(e);
                }
            }
            else
            {
                entities.Add(targetEntity);

                foreach (var n in nodes)
                    entities.Add(FindNearest(n + nodeOffset, onlyType));
            }

            return entities;
        }

        public Entity FindNearest(Vector2 pos, string type, Entity notEntity = null)
        {
            Entity entity = null;
            var minDistance = float.MaxValue;

            foreach (var e in SceneAs<Level>().Entities)
            {
                var typeCorrect = e.GetType().FullName == type || e.GetType().Name == type;

                if (
                    e != notEntity &&
                    e is not Wrapper &&
                    e is not TrailManager &&
                    (type?.Length == 0 || typeCorrect) &&
                    Vector2.Distance(e.Center, pos) < minDistance
                )
                {
                    entity = e;
                    minDistance = Vector2.Distance(e.Center, pos);
                }
            }

            return entity;
        }

        public T FindNearest<T>(Vector2 pos) where T : Entity
        {
            Entity entity = null;
            var minDistance = float.MaxValue;

            foreach (var e in SceneAs<Level>().Entities.FindAll<T>())
            {
                if (Vector2.Distance(e.Center, pos) < minDistance)
                {
                    entity = e;
                    minDistance = Vector2.Distance(e.Center, pos);
                }
            }

            return (T)entity;
        }

        public Entity FindById(int id)
        {
            foreach (var e in SceneAs<Level>().Entities)
            {
                if (e.SourceId.ID == id)
                    return e;
            }

            return null;
        }
    }

    public static Collider ParseCollider(string str) // in format (R/C:X,Y,oX,oY)
    {
        var rgc = new Regex(@"(C:(-?\d+),(-?\d+),(-?\d+))");
        var rgr = new Regex(@"(R:(-?\d+),(-?\d+),(-?\d+),(-?\d+))");
        var matchr = rgr.Match(str);
        var matchc = rgc.Match(str);

        Collider[] colliders = [];

        if (matchr.Length > 0)
        {
            return new Hitbox(
                float.Parse(matchr.Groups[2].Value),
                float.Parse(matchr.Groups[3].Value),
                float.Parse(matchr.Groups[4].Value),
                float.Parse(matchr.Groups[5].Value));
        }

        if (matchc.Length > 0)
        {
            return new Circle(
                float.Parse(matchc.Groups[2].Value),
                float.Parse(matchc.Groups[3].Value),
                float.Parse(matchc.Groups[4].Value));
        }
        Logger.Log(LogLevel.Warn, nameof(AletrisSandboxModule), "Wrong syntax for string " + str + "! (R:X,Y,oX,oY)/(C:R,X,Y)");

        return new Hitbox(8f, 8f, -4f, -8f);
    }

    public static void origUpdCollideHook_1(On.Celeste.Player.orig_Update orig, Player self) // uhh idrk what to do to swap4
    {
        if (!self.Dead && self.StateMachine.State != 21)
        {
            Collider collider = self.Collider;
            self.Collider = self.hurtbox;

            using (List<Component>.Enumerator enumerator = self.Scene.Tracker.GetComponents<PlayerCollider>().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if ((((PlayerCollider)enumerator.Current)!).Check(self) && self.Dead)
                    {
                        self.Collider = collider;

                        return;
                    }
                }
            }

            if (self.Collider == self.hurtbox)
            {
                self.Collider = collider;
            }
        }
    }

    public static List<Image> BuildSprite(MTexture source, Entity entity, Color color)
    {
        List<Image> list = new();
        int num = source.Width / 8;
        int num2 = source.Height / 8;

        for (int i = 0; i < entity.Width; i += 8)
        {
            for (int j = 0; j < entity.Height; j += 8)
            {
                int num3 = i != 0 ? !(i >= entity.Width - 8f) ? Calc.Random.Next(1, num - 1) : num - 1 : 0;
                int num4 = j != 0 ? !(j >= entity.Height - 8f) ? Calc.Random.Next(1, num2 - 1) : num2 - 1 : 0;
                Image image = new(source.GetSubtexture(num3 * 8, num4 * 8, 8, 8)){Position = new Vector2(i, j), Color = color };
                list.Add(image);
            }
        }

        return list;
    }

    public static string ReadModAsset(string filename)
    {
        if (Everest.Content.TryGet(filename, out var asset))
        {
            return ReadModAsset(asset);
        }

        return null;
    }

    public static string ReadModAsset(ModAsset asset)
    {
        using var reader = new StreamReader(asset.Stream);

        return reader.ReadToEnd();
    }

}