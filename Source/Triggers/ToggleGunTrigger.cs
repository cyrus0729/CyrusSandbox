using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.CyrusSandbox.Triggers
{
    [CustomEntity("CyrusSandbox/ToggleGunTrigger")]
    public class ToggleGunTrigger : Trigger
    {
        public bool enableGun;
        public bool Autofire;
        public bool mouseControl;
        public bool destroyStuff;
        public bool hitsStuff;
        public int maxBullets;

        public ToggleGunTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            enableGun = data.Bool("Enable", true);
            Autofire = data.Bool("Autofire", false);
            mouseControl = data.Bool("MouseControl", false);
            destroyStuff = data.Bool("DestroyStuff", false);
            hitsStuff = data.Bool("InteractsWithStuff", false);
            maxBullets = data.Int("BulletsAllowed", 3);
        }

        public override void OnEnter(Player player) // start
        {
            base.OnEnter(player);
            if (player.StateMachine.state != 0) return;
            CyrusSandboxModule.Session.IWBTGGunEnabled = enableGun;
            CyrusSandboxModule.Session.Maxbullets = maxBullets;
            CyrusSandboxModule.Session.IWBTGGunMouseAimEnabled = mouseControl;
            CyrusSandboxModule.Session.IWBTGGunAutofireEnabled = Autofire;
            CyrusSandboxModule.Session.IWBTGGunDestroysStuff = destroyStuff;
            CyrusSandboxModule.Session.IWBTGGunHitsStuff = hitsStuff;

        }

    }
}
