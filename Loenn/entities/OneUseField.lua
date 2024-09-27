local drawableRectangle = require('structs.drawable_rectangle')
local utils = require("utils")

local OneUseField = {
    name = "CyrusSandbox/OneUseField",
    depth = -8500,
    placements = {
        {
            name = "One Use Field",
            data = {
                kill = false,
                depth = -8500,
                inactivecolor = "00ff00",
                inactivebordercolor = "008800",
                activatingcolor =  "ffff00",
                activatingbordercolor = "888800",
                activecolor = "ff0000",
                activebordercolor = "880000",
                width = 8,
                height = 8
            },
        },
    },
}

OneUseField.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true
    },
    interactType = {
        options = {"Kill, Block"},
        editable = false
    }
}

function OneUseField.sprite(room,entity)
    local success,r,g,b = utils.parseHexColor(entity.color)
    local C1 = {r - 0.2 ,g - 0.2 ,b - 0.2}
    local C2 = {r ,g ,b}
    return drawableRectangle.fromRectangle("bordered",entity.x,entity.y,entity.width,entity.height,C1,C2)
end

return OneUseField