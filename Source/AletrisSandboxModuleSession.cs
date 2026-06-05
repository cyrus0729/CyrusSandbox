using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.AletrisSandbox;

public class AletrisSandboxModuleSession : EverestModuleSession
{
    public bool oldEnabledConfig { get; set; }
    public bool IWBTGGunEnabled { get; set; }
    public bool IWBTGGunVisible { get; set; } = true;
    public bool IWBTGGunMouseAimEnabled { get; set; }
    public bool IWBTGGunAutofireEnabled { get; set; }
    public bool IWBTGGunDestroysStuff { get; set; }
    public bool IWBTGGunHitsStuff { get; set; }
    public int MaxBullets { get; set; } = 4;
    public bool IWBTGJumpEnabled { get; set; }

    public bool HPSystemEnabled { get; set; }
    public int HPAmount { get; set; } = 1000;
    public int HPMax { get; set; } = 1000;

    public bool CircleMadelineEnabled { get; set; }
    public float CircleMadelineRadius { get; set; } = 6f;

    public bool mouseControlsState { get; set; } = false; // enabled,

    public bool RelativisticVelocityEnabled { get; set; }
}