using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers.LegacyMonoMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.AletrisSandbox.Entities;

public class SolidThingyNode
{
    public Vector2 Position;
    public SolidThingyNode nextNode;
    public SolidThingyNode prevNode;

    public SolidThingyNode(Vector2 position)
    {
        Position = position;
        nextNode = null;
        prevNode = null;
    }
}

[CustomEntity("AletrisSandbox/CustomCursedMovingSolidThingy")]
public class CustomCursedMovingSolidThingy : Solid
{

    public int XOffset;
    public int YOffset;
    public Color pathColor;
    public bool nonCollidable;
    public bool legacy;

    public MTexture SolidSprite;
    public Image NodeSprite;
    public List<Vector2> nodes;
    public MTexture[,] NineSliceBlock;

    SolidThingyNode currentNode;
    SolidThingyNode headNode;

    Vector2 f;
    Vector2 b;
    Vector2 d;

    public CustomCursedMovingSolidThingy(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Width, data.Height, true)
    {

        Depth = -8500;

        XOffset = data.Int("XOffset");
        YOffset = data.Int("YOffset");
        pathColor = data.HexColor("pathColor", Color.PaleVioletRed);
        nonCollidable = data.Bool("nonCollidable");
        SolidSprite = GFX.Game[data.Attr("sprite") + "solid"];
        NodeSprite = new(GFX.Game[data.Attr("sprite") + "node"]);
        legacy = data.Bool("legacy");

        foreach (Image i in Utils.BuildSprite(SolidSprite, this, nonCollidable ? new(255, 255, 255, 128) : new(255, 255, 255, 255))) { Add(i); }

        nodes = new(data.Nodes);

        SolidThingyNode head = new(data.Position + offset + Collider.HalfSize);
        currentNode = head;

        foreach (var pos in nodes)
        {
            SolidThingyNode newNode = new(pos + offset + Collider.HalfSize);
            currentNode.nextNode = newNode;
            newNode.prevNode = currentNode;
            currentNode = newNode;
        }

        currentNode = head;
        headNode = head;
        Collidable = !nonCollidable;
        NodeSprite.SetColor(nonCollidable ? new(255, 255, 255, 128) : new(255, 255, 255, 255));
        NodeSprite.JustifyOrigin(0.5f, 0.5f);
    }

        public override void Render()
        {
            SolidThingyNode render = headNode;
            Vector2? previousPosition = null;

            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();

            if (player == null)
                return;

            Draw.Line(player.Center,f,Color.Green);
            Draw.Line(player.Center, b, Color.Blue);
            Draw.Line(player.Center, d, Color.Red);

            while (render != null)
            {
                var currentPosition = render.Position;

                NodeSprite.Position = currentPosition;
                NodeSprite.Render();

                if (previousPosition.HasValue)
                {
                    Draw.Line(previousPosition.Value, currentPosition, pathColor);
                }

                previousPosition = currentPosition;
                render = render.nextNode;
            }

            base.Render();

        }

        public static float InverseLerp(Vector2 start, Vector2 end, Vector2 value)
        {
            // X if the horizontal distance is greater, otherwise Y
            if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
                return (value.X - start.X) / (end.X - start.X);
            return (value.Y - start.Y) / (end.Y - start.Y);
        }

        private Vector2 CalculateTravel(Vector2 startPos, float distanceBudget, bool movingForward)
        {
            float distanceRemaining = distanceBudget;
            Vector2 currentPos = startPos;

            SolidThingyNode targetNode = movingForward ? currentNode.nextNode : currentNode.prevNode;

            while (distanceRemaining > 0f && targetNode != null)
            {
                float distToNext = Math.Abs(currentPos.X - targetNode.Position.X);

                if (distToNext >= distanceRemaining)
                {
                    return Vector2.Lerp(currentPos, targetNode.Position, distanceRemaining / distToNext);
                }

                distanceRemaining -= distToNext;
                currentPos = targetNode.Position;

                targetNode = movingForward ? currentNode.nextNode : currentNode.prevNode;
            }

            return currentPos;
        }

        public override void Update()
        {
            base.Update();

            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();

            if (player == null)
                return;

            if (nodes.Count > 1)
            {

                if (!legacy)
                {
                    float budget = player.Speed.Length() * Engine.DeltaTime;

                    Vector2 forwardStep = CalculateTravel(Center, budget, true);
                    Vector2 backwardStep = CalculateTravel(Center, budget, false);
                    Vector2 doNothing = Center;

                    d = doNothing;
                    f = forwardStep;
                    b = backwardStep;

                    float distToForward = Vector2.Distance(player.Center, forwardStep);
                    float distToBackward = Vector2.Distance(player.Center, backwardStep);
                    float distToNothing = Vector2.Distance(player.Center, doNothing);

                    if (distToForward < distToBackward) // F < B
                     {
                        if (distToNothing < distToForward) // N < F < B
                        {
                            MoveTo(doNothing - Collider.HalfSize);
                        }
                        else // F < N < B
                        {
                            MoveTo(forwardStep - Collider.HalfSize);
                        }

                    }
                    else // B < F
                    {
                        if (distToNothing < distToBackward) // N < B < F
                        {
                            MoveTo(doNothing - Collider.HalfSize);
                        }
                        else // B < ? < ?
                        {
                            MoveTo(backwardStep - Collider.HalfSize);
                        }
                    }
                }
                else
                {
                    Vector2 curPos = currentNode.Position;

                    Vector2 bestSnappedPos = curPos;
                    float bestDistanceSq = float.MaxValue;
                    SolidThingyNode nextBestNode = null;

                    // check segment to next node
                    if (currentNode.nextNode != null)
                    {
                        Vector2 path = currentNode.nextNode.Position - curPos;
                        float len = path.Length();

                        if (len > 0)
                        {
                            Vector2 unit = path / len;
                            float proj = MathHelper.Clamp(Vector2.Dot(player.Position - curPos, unit), 0, len);
                            Vector2 snap = curPos + (unit * proj);

                            float distSq = Vector2.DistanceSquared(player.Position, snap);
                            bestSnappedPos = snap;
                            bestDistanceSq = distSq;

                            if (proj > len - 0.001f)
                                nextBestNode = currentNode.nextNode;
                        }
                    }

                    // ditto (only if closer)
                    if (currentNode.prevNode != null)
                    {
                        Vector2 path = currentNode.prevNode.Position - curPos;
                        float len = path.Length();

                        if (len > 0)
                        {
                            Vector2 unit = path / len;
                            float proj = MathHelper.Clamp(Vector2.Dot(player.Position - curPos, unit), 0, len);
                            Vector2 snap = curPos + (unit * proj);

                            float distSq = Vector2.DistanceSquared(player.Position, snap);

                            if (distSq < bestDistanceSq) // if it's actually closer
                            {
                                bestSnappedPos = snap;

                                if (proj > len - 0.001f)
                                    nextBestNode = currentNode.prevNode;
                            }
                        }
                    }

                    MoveTo(bestSnappedPos - Collider.HalfSize + new Vector2(XOffset,YOffset));

                    if (nextBestNode != null)
                    {
                        currentNode = nextBestNode;
                    }
                }
            }
            else
            {
                MoveH(player.CenterX - CenterX);
            }
        }
}