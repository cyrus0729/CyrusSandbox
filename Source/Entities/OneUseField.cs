using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CyrusSandbox.Entities
{
    [CustomEntity("CyrusHelper/OneUseField")]
    public class OneUseField : Entity
    {
        public Color color;
        public Color BorderColor;
        public Color OnColor;
        public Color OnBorderColor;
        public Color ActivatingColor;
        public Color ActivatingBorderColor;

        public Color currentRectColor;
        public Color currentRectBorderColor;

        public new int depth;
        private readonly Solid solid;
        private bool kill;
        public PlayerCollider pc;

        public OneUseField(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            kill = data.Bool("kill", false);
            depth = data.Int("Depth", 8500);
            base.Depth = depth;
            color = data.HexColor("inactiveolor", Calc.HexToColor("#00FF00"));
            BorderColor = data.HexColor("inactivebordercolor", Calc.HexToColor("#008800"));
            OnColor = data.HexColor("activecolor", Calc.HexToColor("#FF0000"));
            OnBorderColor = data.HexColor("activebordercolor", Calc.HexToColor("#880000"));
            ActivatingColor = data.HexColor("activatingcolor", Calc.HexToColor("#FFFF00"));
            ActivatingBorderColor = data.HexColor("activatingbordercolor", Calc.HexToColor("#888800"));
            base.Collider = new Hitbox(data.Width, data.Height, 0f, 0f);
            Add(pc = new PlayerCollider(OnCollide));
        }

        private void OnCollide(Player player)
        {
            currentRectColor = ActivatingColor;
            currentRectBorderColor = ActivatingBorderColor;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (solid != null)
            {
                scene.Add(solid);
            }
        }

        public override void Render()
        {
            Draw.HollowRect(Collider, currentRectBorderColor);
            Draw.Rect(Collider, currentRectColor);

            if (solid != null)
            {

            }

            base.Render();
        }

    }
}