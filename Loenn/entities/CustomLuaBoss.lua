local jautils = require("mods").requireFromPlugin("libraries.jautils", "FrostHelper")
local enums = require("consts.celeste_enums")

local boss = {
    name = "CyrusSandbox/CustomLuaBoss",
    depth = 0,
    nodeLineRenderType = "line",
    texture = "badeline_projectile",
    nodeLimits = {0, -1},
}

jautils.createPlacementsPreserveOrder(boss, "default", {
    { "filename", "Assets/FrostHelper/LuaBoss/example" },
	{ "sprite", "badeline_boss" },
    { "color", "ffffff", "color" },
	{ "lockCamera", false },
    { "cameraLockY", false },
    { "startHit", false },
    { "defaultWaveStrength", 0 },
    { "noIdleAudio", false },
    { "noShatterSound", false},
    { "noShotShootSound", false },
    { "noBeamShootSound", false },
    { "noShotChargeSound", false },
    { "noBeamChargeSound", false },
    { "bossWaveFrequency", 0 }
})

return boss
