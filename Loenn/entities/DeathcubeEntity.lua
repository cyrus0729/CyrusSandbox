local drawableSprite = require('structs.drawable_sprite')
local enums = require("consts.celeste_enums")

local DeathcubeEntity = {}

DeathcubeEntity.name = "CyrusSandbox/DeathcubeEntity"

DeathcubeEntity.fieldInformation = {
    direction = {
        options = enums.move_block_directions,
        editable = false
    }
}

local directions = {
    ["Up"] = 0,
    ["Down"] = math.rad(180),
    ["Left"] = math.rad(270),
    ["Right"] = math.rad(90)
}

DeathcubeEntity.placements = {
    {
        name = "Death Cube",
        data = {
            direction = "Up",
            unforgivingHitbox = false
        }
    }
}

DeathcubeEntity.sprite = function(room,entity)
    local sprite = drawableSprite.fromTexture("objects/CyrusSandbox/Deathcubeentity/idle00", entity)
    sprite.rotation = directions[entity.direction]
    return sprite
end


return DeathcubeEntity