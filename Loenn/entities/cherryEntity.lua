local drawableSprite = require('structs.drawable_sprite')
local utils = require("utils")

local cherryEntity = {}

cherryEntity.name = "CyrusSandbox/cherryEntity"
cherryEntity.fieldInformation = {
    unforgivingHitbox = {
        fieldType = "boolean"
    },
    animatedHitbox = {
        fieldType = "boolean"
    },
    bigHitbox = {
        fieldType = "boolean"
    },
    color = {
        fieldType = "color",
        allowXNAColors = true
    },
    animationRate = {
        fieldType = "integer"
    }
}

function cherryEntity.texture(room, entity)
    local sprite
    if entity.bigHitbox then
        sprite = "objects/CyrusSandbox/cherryEntityBig/idle00" 
    else
        sprite = "objects/CyrusSandbox/cherryEntity/idle00" 
    end
    return sprite
end

cherryEntity.placements = {
    {
        name = "Delicious Cherry",
        data = {
            animatedHitbox = false,
            unforgivingHitbox = false,
            bigHitbox = false,
            color = "ff0000",
            animationRate = 30
        }
    }
}

return cherryEntity