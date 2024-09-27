using Celeste.Mod.CyrusSandbox;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.CyrusSandbox.Triggers
{
    internal static class EntitydataExtensions
    {
        internal static Vector2 Vector(this EntityData data, string name) //thanks snip!!!!@!!!!!! :333
        {
            var splitValue = data.Attr(name, "").Split(',');

            if (splitValue.Length != 2)
            {
                throw new InvalidOperationException($"\"{name}\" is not a valid vector; expected 2 comma-separated values, but got {splitValue.Length}");
            }

            if (!float.TryParse(splitValue[0], out var x))
                throw new InvalidOperationException($"\"{name}\" is not a valid vector; \"{splitValue[0]}\" (X component) is not a number");

            if (!float.TryParse(splitValue[1], out var y))
                throw new InvalidOperationException($"\"{name}\" is not a valid vector; \"{splitValue[1]}\" (Y component) is not a number");

            return new Vector2(x, y);
        }

    }

    [CustomEntity("CyrusHelper/PlayerSpriteSizeTrigger")]
    public class PlayerSpriteSizeTrigger : Trigger
    {

        public string flagTrigger;

        public bool IsFlag;

        public bool Persistent;

        public Vector2 size;

        public Vector2 originalsize;

        public PlayerSpriteSizeTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            size = data.Vector("Scale");
            flagTrigger = data.Attr("Flag", "");
            Persistent = data.Bool("Persistent", false);
        }

        public override void OnEnter(Player player) // start
        {
            base.OnEnter(player);

            Level level = base.Scene as Level;
            IsFlag = level.Session.GetFlag(flagTrigger);
            if (flagTrigger != "")
            {
                if (!IsFlag) { return; }
            }
            else { IsFlag = true; }

            originalsize = new Vector2(player.Sprite.Scale.X, player.Sprite.Scale.Y);
            player.Sprite.Scale = new Vector2(size.X, size.Y);
            CyrusSandboxModule.Session.SizeChangeSize = new Vector2(size.X, size.Y);
            CyrusSandboxModule.Session.SizeChangePersistent = Persistent;
        }

        public override void OnStay(Player player) // repeat
        {
            base.OnStay(player);
            if (!IsFlag) { return; }
            player.Sprite.Scale = new Vector2(size.X, size.Y);
        }

        public override void OnLeave(Player player) // leave
        {
            base.OnLeave(player);
            if (Persistent) { return; }
            player.Sprite.Scale = new Vector2(originalsize.X, originalsize.Y);
        }

        public static void hookPlayerRender(On.Celeste.Player.orig_Render orig, Player self, Scene scene)
        {
            orig.Invoke(self);
            if (!CyrusSandboxModule.Session.SizeChangePersistent) { return; }
            Logger.Log(LogLevel.Info, "ch", CyrusSandboxModule.Session.SizeChangeSize.ToString());
            self.Sprite.Scale = new Vector2(CyrusSandboxModule.Session.SizeChangeSize.X, CyrusSandboxModule.Session.SizeChangeSize.Y);
        }
    }
}
