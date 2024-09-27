using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste.Mod.CyrusSandbox.Entities
{
    [Tracked]
    [CustomEntity("CyrusSandbox/healthDisplay")]
    public class healthDisplay : Entity
    {

        public int currentHP = CyrusSandboxModule.Session.HPAmount;

        public int maxHP = CyrusSandboxModule.Session.HPMax;

        public healthDisplay()
        {
            Tag = Tags.HUD | Tags.Global;
            Add(new BeforeRenderHook(new Action(DrawHP)));
        }

        private static void StartSpriteBatch()
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
        }
        private static void EndSpriteBatch()
        {
            Draw.SpriteBatch.End();
        }

        private void DrawHP()
        {
            ActiveFont.Draw(CyrusSandboxModule.Session.HPAmount.ToString() + "/" + CyrusSandboxModule.Session.HPMax.ToString(), new Vector2(720f, 144f), Color.White);
        }

    }
}
