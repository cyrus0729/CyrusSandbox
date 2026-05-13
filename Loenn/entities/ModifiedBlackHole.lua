local utils = require("utils")

local blackhole = {}

blackhole.name = "AletrisSandbox/ModifiedBlackHole"

blackhole.fieldInformation = {
        SpeedModifier = { fieldType = "number", default = 1.02 },
        ForceModifier = { fieldType = "number", default = 0.8 },
        auraRadius = { fieldType = "number", default = 48 },
        holeRadius = { fieldType = "number", default = 8 },
}

blackhole. fieldOrder = {"x","y","SpeedModifier","ForceModifier","auraRadius","holeRadius"}

blackhole.placements = {
    name = "Modified Black Hole",
    placementType = "point",
    data = {
        SpeedModifier = 1.02,
        ForceModifier = 0.8,
        auraRadius = 48,
        holeRadius = 8
    }
}

blackhole.texture = "objects/AletrisSandbox/ModifiedBlackHole/LoennPreview"

function blackhole.selection(room, entity)
    return utils.rectangle(entity.x - 15, entity.y - 15, 30, 30)
end

function blackhole.scale(room,entity)
    return entity.auraRadius/48, entity.auraRadius/48
end

return blackhole