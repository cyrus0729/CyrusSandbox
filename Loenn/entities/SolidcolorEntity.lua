local utils = require("utils")
local drawable = require("structs.drawable_rectangle")

local SolidcolorEntity = {}

SolidcolorEntity.name = "CyrusSandbox/SolidcolorEntity"
SolidcolorEntity.minimumSize = {8,8}
SolidcolorEntity.fieldInformation = {
    Color = {
        fieldType = "color",
        allowXNAColors = true
    },
    LineColor = {
        fieldType = "color",
        allowXNAColors = true
    },
    Interaction = {
        options = {"None", "Deadly", "Solid"},
        editable = false
    },
    DrawType = {
        options = {"Line","Fill","Bordered"},
        editable = false
    }
}

SolidcolorEntity.placements = {
    {
        name = "Solid Color Entity",
        data = {
            width = 16,
            height = 16,
            Color = "ffffff",
            LineColor = "000000",
            DrawType = "Line",
            Interaction = "None",
            Depth = 8500,
            --color = "eebbdb" hehe eeby deeby
        }
    }
}

function SolidcolorEntity.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8
    local color = entity.Color --custom
    local bordercolor = entity.LineColor
    local rect = drawable.fromRectangle(string.lower(entity.DrawType), x, y, width, height, color, bordercolor) --"line","fill","bordered"
    return rect
end

return SolidcolorEntity