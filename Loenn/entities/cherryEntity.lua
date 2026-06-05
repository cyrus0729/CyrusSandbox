local drawableSprite = require('structs.drawable_sprite')
local utils = require("utils")
local love = require("love")

local CherryEntity = {}

local shader_invert = love.graphics.newShader[[ vec4 effect(vec4 color, Image texture, vec2 texture_coords, vec2 pixel_coords) { vec4 col = texture2D( texture, texture_coords ); return vec4(1-col.r, 1-col.g, 1-col.b, col.a); } ]]

CherryEntity.name = "AletrisSandbox/CherryEntity"
CherryEntity.fieldInformation = {
    unforgivingHitbox = { fieldType = "boolean", default = false },
    animatedHitbox = { fieldType = "boolean", default = false },
    bigHitbox = { fieldType = "boolean", default = false },
    color = { fieldType = "color", allowXNAColors = true, default = "#ffffff" },
    animationRate = { fieldType = "integer", default = 1 },
    Inverted = { fieldType = "boolean", default = false },
}

CherryEntity.fieldOrder = {"x","y","color","animationRate","animatedHitbox","unforgivingHitbox","bigHitbox","Inverted"}

CherryEntity.texture = function(room, entity) 
    return "danger/AletrisSandbox/CherryEntity/"..(entity.bigHitbox and "bigidle00" or "idle00") 
end

CherryEntity.placements = {
    name = "Delicious Fruit",
    placementType = "point",
    data = {
        unforgivingHitbox = false,
        animatedHitbox = false,
        bigHitbox = false,
        color = "#ffffff",
        animationRate = 1,
        Inverted = false,
    }
}

return CherryEntity