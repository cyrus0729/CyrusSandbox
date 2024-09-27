using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CyrusSandbox.Entities
{
    [CustomEntity("CyrusHelper/SolidcolorEntity")]
    public class SolidcolorEntity : Entity
    {
        public Color color;
        public Color BorderColor;
        public enum InteractionType { None, Deadly, Solid };
        public readonly InteractionType interactions;
        public enum DrawType { Line, Fill, Bordered };
        public readonly DrawType drawType;
        public new int depth;
        private readonly Solid solid;
        private bool kill;
        public PlayerCollider pc;

        public SolidcolorEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            kill = false;
            depth = data.Int("Depth", 8500);
            base.Depth = depth;
            color = data.HexColor("Color", Calc.HexToColor("#FFFFFF"));
            BorderColor = data.HexColor("LineColor", Calc.HexToColor("#000000"));
            drawType = data.Enum("DrawType", DrawType.Line);
            base.Collider = new Hitbox(data.Width, data.Height, 0f, 0f);
            interactions = data.Enum("Interaction", InteractionType.None);
            Add(pc = new PlayerCollider(OnCollide));
            switch (interactions)
            {
                case InteractionType.Deadly:
                    kill = true;
                    break;
                case InteractionType.Solid:
                    solid = new Solid(Position, base.Width, base.Height, safe: false);
                    break;
                case InteractionType.None:
                    break;
            }
        }

        private void OnCollide(Player player)
        {
            if (OnCollide != null && kill == true)
            {
                player.Die((player.Center - base.Center).SafeNormalize());
            }
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
            if (drawType == DrawType.Line)
            {
                Draw.HollowRect(Collider, color);
            }
            else if (drawType == DrawType.Fill)
            {
                Draw.Rect(Collider, color);
            }
            else if (drawType == DrawType.Bordered)
            {
                Draw.HollowRect(Collider, color);
                Draw.Rect(Collider, color);
            }
            base.Render();
        }

    }
}