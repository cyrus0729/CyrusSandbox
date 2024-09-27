using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.CyrusSandbox.Triggers
{
    [CustomEntity("CyrusSandbox/ToggleHealthTrigger")]
    public class ToggleHealthTrigger : Trigger
    {
        public bool enableHealth;
        public int defaultHealth;

        public ToggleHealthTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            enableHealth = data.Bool("Enable", true);
            defaultHealth = data.Int("DefaultHealth", 1000);
        }

        public override void OnEnter(Player player) // start
        {
            base.OnEnter(player);
            if (CyrusSandboxModule.Session.HPSystemEnabled) CyrusSandboxModule.Session.HPSystemEnabled = false;
            CyrusSandboxModule.Session.HPSystemEnabled = enableHealth;
            CyrusSandboxModule.Session.HPAmount = defaultHealth;
            CyrusSandboxModule.Session.HPMax = defaultHealth;
        }

    }
}
