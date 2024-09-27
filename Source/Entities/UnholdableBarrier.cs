using Celeste;
using Celeste.Mod.CyrusSandbox;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

// THANK YOU *SAMAH*, USSRNAME, MADDIE, AON, DZHAKE

namespace CyrusSandbox.Entities
{
    [Tracked]
    [CustomEntity("CyrusSandbox/UnholdableBarrier")]
    public class UnholdableBarrier : Solid
    {

        public float Flash;
        public float Solidify;
        public bool Flashing;
        public string colore;
        private float solidifyDelay;
        protected List<Vector2> particles = new List<Vector2>();
        private List<UnholdableBarrier> adjacent = new List<UnholdableBarrier>();
        private float[] speeds = new float[3] { 12f, 20f, 40f };

        public UnholdableBarrier(EntityData data, Vector2 offset)
            : base(data.Position + offset, data.Width, data.Height, true)
        {
            colore = data.Attr("color", "a4911e");
            this.Collidable = false;
            for (int i = 0; (float)i < base.Width * base.Height / 16f; i++)
            {
                particles.Add(new Vector2(Calc.Random.NextFloat(base.Width - 1f), Calc.Random.NextFloat(base.Height - 1f)));
            }
            Collidable = false;
            Add(new CyrusSandboxModule.OnlyBlocksPlayer());
        }

        public override void Render()
        {
            if (Flashing)
            {
                Flash = Calc.Approach(Flash, 0f, Engine.DeltaTime * 4f);
                if (Flash <= 0f)
                {
                    Flashing = false;
                }
            }
            else if (solidifyDelay > 0f)
            {
                solidifyDelay -= Engine.DeltaTime;
            }
            else if (Solidify > 0f)
            {
                Solidify = Calc.Approach(Solidify, 0f, Engine.DeltaTime);
            }
            int num = speeds.Length;
            int i = 0;
            for (int count = particles.Count; i < count; i++)
            {
                Vector2 value = particles[i] + (new Vector2(0f, 1f) * speeds[i % num] * Engine.DeltaTime);
                value.Y = mod(value.Y, Height - 1f);
                value.X = mod(value.X, Width - 1f);
                particles[i] = value;
            }
            Draw.Rect(Collider, Calc.HexToColor(colore));
            base.Render();
        }

        protected float mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }

}