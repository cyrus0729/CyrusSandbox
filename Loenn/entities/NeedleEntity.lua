local drawableSprite = require('structs.drawable_sprite')
local enums = require("consts.celeste_enums")
local utils = require("utils")
local math = require("math")

local sideIndexLookup = {
    "Up",
    "Right",
    "Down",
    "Left",

    Up = 1,
    Right = 2,
    Down = 3,
    Left = 4
}

local function createNeedle(name,displayname,rotation,mini)
    local NeedleEntity = {}
    
    NeedleEntity.name = name
    NeedleEntity.fieldInformation = {
        hitbox = {
        options = {"NeedleHelper", "Forgiving", "Unforgiving"},
        editable = false
        },
        isMini = {
            fieldType = "boolean",
            default = false
        }
    }

    function NeedleEntity.justification(room, entity)
        return not entity.isMini and {0,0} or {0.5,0.5}
    end

    NeedleEntity.placements = {
        {
            name = displayname,
            data = {
                Sprite = (not mini and "needleEntity" or "miniNeedleEntity"),
                hitbox = "NeedleHelper",
                isMini = mini
            }
        }
    }

    NeedleEntity.sprite = function(room,entity)
        local sprite 
        local spritepath = (entity.isMini and "/mini00" or "/needle00")
        sprite = drawableSprite.fromTexture("objects/CyrusSandbox/".."NeedleEntity"..spritepath, entity)
        sprite.rotation = math.rad(90*(rotation-1))
        return sprite
    end

    function NeedleEntity.rotate(room, entity, direction)
        local newFacing = utils.mod1(rotation + direction,4)
        entity._name = "CyrusSandbox/"..(entity.isMini and "miniNeedle" or "Needle")..sideIndexLookup[newFacing]
        return true
    end


    return NeedleEntity
end

local NeedleEntityUp = createNeedle("CyrusSandbox/NeedleUp","Needle (Up)",1,false)
local NeedleEntityDown = createNeedle("CyrusSandbox/NeedleDown","Needle (Down)",3,false)
local NeedleEntityRight = createNeedle("CyrusSandbox/NeedleLeft","Needle (Left)",4,false)
local NeedleEntityLeft =  createNeedle("CyrusSandbox/NeedleRight","Needle (Right)",2,false)

local miniNeedleEntityUp = createNeedle("CyrusSandbox/miniNeedleUp","Mini Needle (Up)",1,true)
local miniNeedleEntityDown = createNeedle("CyrusSandbox/miniNeedleDown","Mini Needle (Down)",3,true)
local miniNeedleEntityRight = createNeedle("CyrusSandbox/miniNeedleLeft","Mini Needle (Left)",4,true)
local miniNeedleEntityLeft =  createNeedle("CyrusSandbox/miniNeedleRight","Mini Needle (Right)",2,true)


return {
    NeedleEntityUp,
    NeedleEntityDown,
    NeedleEntityLeft,
    NeedleEntityRight,
    miniNeedleEntityUp,
    miniNeedleEntityDown,
    miniNeedleEntityRight,
    miniNeedleEntityLeft}