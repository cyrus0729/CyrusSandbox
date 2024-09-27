using CyrusSandbox.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Celeste
{
    // Token: 0x020003B0 RID: 944
    [Tracked(false)]
    public class UnholdableBarrierRenderer : Entity
    {
        public UnholdableBarrier Parent;
        public bool Visible;
        public Vector2 A;
        public Vector2 B;
        public Vector2 Min;
        public Vector2 Max;
        public Vector2 Normal;
        public Vector2 Perpendicular;
        public float[] Wave;
        public float Length;

        private List<UnholdableBarrier> list;
        private List<UnholdableBarrierRenderer.Edge> edges;
        private VirtualMap<bool> tiles;
        private Rectangle levelTileBounds;
        private bool dirty;
        private class Edge;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Edge(UnholdableBarrier parent, Vector2 a, Vector2 b)
        {
            this.Parent = parent;
            this.Visible = true;
            this.A = a;
            this.B = b;
            this.Min = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
            this.Max = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
            this.Normal = (b - a).SafeNormalize();
            this.Perpendicular = -this.Normal.Perpendicular();
            this.Length = (a - b).Length();
        }

        // Token: 0x06001960 RID: 6496 RVA: 0x00078218 File Offset: 0x00076418
        [MethodImpl(MethodImplOptions.NoInlining)]
        public UnholdableBarrierRenderer()
        {
            this.list = new List<UnholdableBarrier>();
            this.edges = new List<UnholdableBarrierRenderer.Edge>();
            base.Tag = (Tags.Global | Tags.TransitionUpdate);
            base.Depth = 0;
            base.Add(new CustomBloom(new Action(this.OnRenderBloom)));
        }

        // Token: 0x06001961 RID: 6497 RVA: 0x0007827C File Offset: 0x0007647C
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Track(UnholdableBarrier block)
        {
            this.list.Add(block);
            if (this.tiles == null)
            {
                this.levelTileBounds = (base.Scene as Level).TileBounds;
                this.tiles = new VirtualMap<bool>(this.levelTileBounds.Width, this.levelTileBounds.Height, false);
            }
            int num = (int)block.X / 8;
            while ((float)num < block.Right / 8f)
            {
                int num2 = (int)block.Y / 8;
                while ((float)num2 < block.Bottom / 8f)
                {
                    this.tiles[num - this.levelTileBounds.X, num2 - this.levelTileBounds.Y] = true;
                    num2++;
                }
                num++;
            }
            this.dirty = true;
        }

        // Token: 0x06001962 RID: 6498 RVA: 0x00078344 File Offset: 0x00076544
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Untrack(UnholdableBarrier block)
        {
            this.list.Remove(block);
            if (this.list.Count <= 0)
            {
                this.tiles = null;
            }
            else
            {
                int num = (int)block.X / 8;
                while ((float)num < block.Right / 8f)
                {
                    int num2 = (int)block.Y / 8;
                    while ((float)num2 < block.Bottom / 8f)
                    {
                        this.tiles[num - this.levelTileBounds.X, num2 - this.levelTileBounds.Y] = false;
                        num2++;
                    }
                    num++;
                }
            }
            this.dirty = true;
        }

        // Token: 0x06001963 RID: 6499 RVA: 0x000783E2 File Offset: 0x000765E2
        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Update()
        {
            if (this.dirty)
            {
                this.RebuildEdges();
            }
            this.UpdateEdges();
        }

        // Token: 0x06001964 RID: 6500 RVA: 0x000783F8 File Offset: 0x000765F8
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void UpdateEdges()
        {
            Camera camera = (base.Scene as Level).Camera;
            Rectangle rectangle = new Rectangle((int)camera.Left - 4, (int)camera.Top - 4, (int)(camera.Right - camera.Left) + 8, (int)(camera.Bottom - camera.Top) + 8);
            for (int i = 0; i < this.edges.Count; i++)
            {
                if (this.edges[i].Visible)
                {
                    if (base.Scene.OnInterval(0.25f, (float)i * 0.01f) && !this.edges[i].InView(ref rectangle))
                    {
                        this.edges[i].Visible = false;
                    }
                }
                else if (base.Scene.OnInterval(0.05f, (float)i * 0.01f) && this.edges[i].InView(ref rectangle))
                {
                    this.edges[i].Visible = true;
                }
                if (this.edges[i].Visible && (base.Scene.OnInterval(0.05f, (float)i * 0.01f) || this.edges[i].Wave == null))
                {
                    this.edges[i].UpdateWave(base.Scene.TimeActive * 3f);
                }
            }
        }

        // Token: 0x06001965 RID: 6501 RVA: 0x00078564 File Offset: 0x00076764
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RebuildEdges()
        {
            this.dirty = false;
            this.edges.Clear();
            if (this.list.Count > 0)
            {
                Level level = base.Scene as Level;
                int left = level.TileBounds.Left;
                int top = level.TileBounds.Top;
                int right = level.TileBounds.Right;
                int bottom = level.TileBounds.Bottom;
                Point[] array = new Point[]
                {
                    new Point(0, -1),
                    new Point(0, 1),
                    new Point(-1, 0),
                    new Point(1, 0)
                };
                foreach (UnholdableBarrier seekerBarrier in this.list)
                {
                    int num = (int)seekerBarrier.X / 8;
                    while ((float)num < seekerBarrier.Right / 8f)
                    {
                        int num2 = (int)seekerBarrier.Y / 8;
                        while ((float)num2 < seekerBarrier.Bottom / 8f)
                        {
                            foreach (Point point in array)
                            {
                                Point point2 = new Point(-point.Y, point.X);
                                if (!this.Inside(num + point.X, num2 + point.Y) && (!this.Inside(num - point2.X, num2 - point2.Y) || this.Inside(num + point.X - point2.X, num2 + point.Y - point2.Y)))
                                {
                                    Point point3 = new Point(num, num2);
                                    Point point4 = new Point(num + point2.X, num2 + point2.Y);
                                    Vector2 value = new Vector2(4f) + new Vector2((float)(point.X - point2.X), (float)(point.Y - point2.Y)) * 4f;
                                    while (this.Inside(point4.X, point4.Y) && !this.Inside(point4.X + point.X, point4.Y + point.Y))
                                    {
                                        point4.X += point2.X;
                                        point4.Y += point2.Y;
                                    }
                                    Vector2 a = new Vector2((float)point3.X, (float)point3.Y) * 8f + value - seekerBarrier.Position;
                                    Vector2 b = new Vector2((float)point4.X, (float)point4.Y) * 8f + value - seekerBarrier.Position;
                                    this.edges.Add(new UnholdableBarrierRenderer.Edge(seekerBarrier, a, b));
                                }
                            }
                            num2++;
                        }
                        num++;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool Inside(int tx, int ty)
        {
            return this.tiles[tx - this.levelTileBounds.X, ty - this.levelTileBounds.Y];
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnRenderBloom()
        {
            Camera camera = (base.Scene as Level).Camera;
            new Rectangle((int)camera.Left, (int)camera.Top, (int)(camera.Right - camera.Left), (int)(camera.Bottom - camera.Top));
            foreach (UnholdableBarrier seekerBarrier in this.list)
            {
                if (seekerBarrier.Visible)
                {
                    Draw.Rect(seekerBarrier.X, seekerBarrier.Y, seekerBarrier.Width, seekerBarrier.Height, Color.White);
                }
            }
            foreach (UnholdableBarrierRenderer.Edge edge in this.edges)
            {
                if (edge.Visible)
                {
                    Vector2 value = edge.Parent.Position + edge.A;
                    edge.Parent.Position + edge.B;
                    int num = 0;
                    while ((float)num <= edge.Length)
                    {
                        Vector2 vector = value + edge.Normal * (float)num;
                        Draw.Line(vector, vector + edge.Perpendicular * edge.Wave[num], Color.White);
                        num++;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Render()
        {
            if (this.list.Count <= 0)
            {
                return;
            }
            Color color = Color.White * 0.15f;
            Color value = Color.White * 0.25f;
            foreach (UnholdableBarrier seekerBarrier in this.list)
            {
                if (seekerBarrier.Visible)
                {
                    Draw.Rect(seekerBarrier.Collider, color);
                }
            }
            if (this.edges.Count > 0)
            {
                foreach (UnholdableBarrierRenderer.Edge edge in this.edges)
                {
                    if (edge.Visible)
                    {
                        Vector2 value2 = edge.Parent.Position + edge.A;
                        edge.Parent.Position + edge.B;
                        Color.Lerp(value, Color.White, edge.Parent.Flash);
                        int num = 0;
                        while ((float)num <= edge.Length)
                        {
                            Vector2 vector = value2 + edge.Normal * (float)num;
                            Draw.Line(vector, vector + edge.Perpendicular * edge.Wave[num], color);
                            num++;
                        }
                    }
                }
            }
        }

        {

            [MethodImpl(MethodImplOptions.NoInlining)]
        public void UpdateWave(float time)
        {
            if (this.Wave == null || (float)this.Wave.Length <= this.Length)
            {
                this.Wave = new float[(int)this.Length + 2];
            }
            int num = 0;
            while ((float)num <= this.Length)
            {
                this.Wave[num] = this.GetWaveAt(time, (float)num, this.Length);
                num++;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private float GetWaveAt(float offset, float along, float length)
        {
            if (along <= 1f || along >= length - 1f)
            {
                return 0f;
            }
            if (this.Parent.Solidify >= 1f)
            {
                return 0f;
            }
            float num = offset + along * 0.25f;
            float num2 = (float)(Math.Sin((double)num) * 2.0 + Math.Sin((double)(num * 0.25f)));
            return (1f + num2 * Ease.SineInOut(Calc.YoYo(along / length))) * (1f - this.Parent.Solidify);
        }

        // Token: 0x0600196C RID: 6508 RVA: 0x00078DB0 File Offset: 0x00076FB0
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool InView(ref Rectangle view)
        {
            return (float)view.Left < this.Parent.X + this.Max.X && (float)view.Right > this.Parent.X + this.Min.X && (float)view.Top < this.Parent.Y + this.Max.Y && (float)view.Bottom > this.Parent.Y + this.Min.Y;
        }

    }
}
}
