using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

// to the entirety of #code_modding: thank you so much lmao i could not have done this without you guys
// oh and to snip for dealing with all my stupid bullshit .w.

namespace Celeste.Mod.CyrusSandbox.Entities
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

            if (!int.TryParse(splitValue[0], out var x))
                throw new InvalidOperationException($"\"{name}\" is not a valid vector; \"{splitValue[0]}\" (X component) is not a number");

            if (!int.TryParse(splitValue[1], out var y))
                throw new InvalidOperationException($"\"{name}\" is not a valid vector; \"{splitValue[1]}\" (Y component) is not a number");

            return new Vector2(x, y);
        }

        internal static Vector2 Offset(this EntityData data, string name) //modified
        {
            var splitValue = data.Attr(name, "").Split(',');
            if (splitValue.Length != 2)
            {
                throw new InvalidOperationException($"\"{name}\" is not a valid vector; expected 2 comma-separated values, but got {splitValue.Length}");
            }

            if (!int.TryParse(splitValue[0], out var offsetx))
                throw new InvalidOperationException($"\"{name}\" is not a valid offset; \"{splitValue[2]}\" (X component) is not a number");
            if (!int.TryParse(splitValue[1], out var offsety))
                throw new InvalidOperationException($"\"{name}\" is not a valid offset; \"{splitValue[3]}\" (Y component) is not a number");
            return new Vector2(offsetx, offsety);
        }

    }

    [CustomEntity("CyrusHelper/HitboxController")]
    public class HitboxController : Entity
    {

        private Vector2 newHitbox;
        private Vector2 newHurtbox;

        private Vector2 newduckHitbox;
        private Vector2 newduckHurtbox;

        private Vector2 newfeatherHitbox;
        private Vector2 newfeatherHurtbox;

        private Vector2 newHitboxOffset;
        private Vector2 newHurtboxOffset;

        private Vector2 newduckHitboxOffset;
        private Vector2 newduckHurtboxOffset;

        private Vector2 newfeatherHitboxOffset;
        private Vector2 newfeatherHurtboxOffset;

        public Player player;

        public bool IsAdvancedModeOn;
        public bool ModifyHitbox;

        public HitboxController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

            newHitbox = data.Vector("Hitbox");
            newHurtbox = data.Vector("Hurtbox");

            newduckHitbox = data.Vector("duckHitbox");
            newduckHurtbox = data.Vector("duckHurtbox");

            newfeatherHitbox = data.Vector("featherHitbox");
            newfeatherHurtbox = data.Vector("featherHurtbox");

            IsAdvancedModeOn = data.Bool("advancedMode", false);
            ModifyHitbox = data.Bool("modifyHitbox", false);

            if (IsAdvancedModeOn)
            {
                newHitboxOffset = data.Offset("HitboxOffset");
                newHurtboxOffset = data.Offset("HurtboxOffset");

                newduckHitboxOffset = data.Offset("duckHitboxOffset");
                newduckHurtboxOffset = data.Offset("duckHurtboxOffset");

                newfeatherHitboxOffset = data.Offset("featherHitboxOffset");
                newfeatherHurtboxOffset = data.Offset("featherHurtboxOffset");
            }

        }


        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            if (!CyrusSandboxModule.Settings.HitboxOptions.PlayerHitboxEnableOverride || ModifyHitbox) { return; }

            Player player = Scene.Tracker.GetEntity<Player>();

            player.normalHitbox.Width = newHitbox.X;
            player.normalHitbox.Height = newHitbox.Y;

            player.duckHitbox.Width = newduckHitbox.X;
            player.duckHitbox.Height = newduckHitbox.Y;

            player.starFlyHitbox.Width = newfeatherHitbox.X;
            player.starFlyHitbox.Height = newfeatherHitbox.Y;

            player.normalHurtbox.Width = newHurtbox.X;
            player.normalHurtbox.Height = newHurtbox.Y;

            player.duckHurtbox.Width = newduckHurtbox.X;
            player.duckHurtbox.Height = newduckHurtbox.Y;

            player.starFlyHurtbox.Width = newfeatherHurtbox.X;
            player.starFlyHurtbox.Height = newfeatherHurtbox.Y;

            player.normalHitbox.Position.X = (IsAdvancedModeOn ? newHitboxOffset.X : -4);
            player.normalHitbox.Position.Y = (IsAdvancedModeOn ? newHitboxOffset.Y : -11);

            player.duckHitbox.Position.X = (IsAdvancedModeOn ? newduckHitboxOffset.X : -4);
            player.duckHitbox.Position.Y = (IsAdvancedModeOn ? newduckHitboxOffset.Y : -6);

            player.starFlyHitbox.Position.X = (IsAdvancedModeOn ? newfeatherHitboxOffset.X : -4);
            player.starFlyHitbox.Position.Y = (IsAdvancedModeOn ? newfeatherHitboxOffset.Y : -10);

            player.normalHurtbox.Position.X = (IsAdvancedModeOn ? newHurtboxOffset.X : -4);
            player.normalHurtbox.Position.Y = (IsAdvancedModeOn ? newHurtboxOffset.Y : -11);

            player.duckHurtbox.Position.X = (IsAdvancedModeOn ? newduckHurtboxOffset.X : -4);
            player.duckHurtbox.Position.Y = (IsAdvancedModeOn ? newduckHurtboxOffset.Y : -6);

            player.starFlyHurtbox.Position.X = (IsAdvancedModeOn ? newfeatherHurtboxOffset.X : -3);
            player.starFlyHurtbox.Position.Y = (IsAdvancedModeOn ? newfeatherHurtboxOffset.Y : -9);
        }


    }
}