local drawableSprite = require('structs.drawable_sprite')

local HitboxController = {}

HitboxController.name = "CyrusSandbox/HitboxController"

HitboxController.placements = {
    {
        name = "Hitbox Controller",
        data = {
            advancedMode = false,
            modifyHitbox = true,
            Hitbox = "8,11",
            duckHitbox = "8,6",
            featherHitbox = "8,8",
            Hurtbox = "8,9",
            duckHurtbox = "8,4",
            featherHurtbox = "6,6",

            HitboxOffset = "-4,-11",
            duckHitboxOffset = "-4,-6",
            featherHitboxOffset = "-4,-10",
            HurtboxOffset = "-4,-11",
            duckHurtboxOffset = "-4,-6",
            featherHurtboxOffset = "-3,-9",
        }
    }
}

function HitboxController.texture(room, entity)
    if entity.modifyHitbox then
        return "objects/CyrusSandbox/HitboxController/catplushthebubble"
    else
        return "objects/CyrusSandbox/HitboxController/catplushthenuhble"
    end
end

return HitboxController