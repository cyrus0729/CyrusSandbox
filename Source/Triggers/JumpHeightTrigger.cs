using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using static Celeste.TrackSpinner;

namespace Celeste.Mod.CyrusSandbox.Triggers {
    [CustomEntity("CyrusHelper/JumpHeightTrigger")]
    public class JumpHeightTrigger : Trigger {

        public float minJumpHeight;

        public float maxJumpHeight;

        public bool persistent;

        public JumpHeightTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            minJumpHeight = data.Float("minimumJumpHeight", -105);
            maxJumpHeight = data.Float("maximumJumpHeight", 2);
            persistent = data.Bool("persistent", false);
        }

        private static void OverrideJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
        {
            float gravity = 900f;
            self.Speed.Y = self.Scene.Tracker.GetEntity<JumpHeightTrigger>().minJumpHeight;
            gravity = (self.Speed.Y - gravity) / 0.2f;
            self.Speed.Y = gravity;
            float timeToMaxHeight = Math.Abs((self.Speed.Y - gravity) / gravity);
            self.Speed.Y = -gravity;
        }

        private static void OverrideSuperJump(On.Celeste.Player.orig_SuperJump orig, Player self)
        {
            throw new NotImplementedException();
        }

        private static void OverrideSuperWallJump(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
        {
            throw new NotImplementedException();
        }

        public static void Load()
        {
            On.Celeste.Player.Jump += OverrideJump;
            On.Celeste.Player.SuperJump += OverrideSuperJump;
            On.Celeste.Player.SuperWallJump += OverrideSuperWallJump;
        }

        public static void Unload()
        {
            On.Celeste.Player.Jump -= OverrideJump;
            On.Celeste.Player.SuperJump -= OverrideSuperJump;
            On.Celeste.Player.SuperWallJump -= OverrideSuperWallJump;
        }


        public override void OnEnter(Player player)
        {
            
        }
    }
}
