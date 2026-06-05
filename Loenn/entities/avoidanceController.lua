local avoidanceController = {}

avoidanceController.name = "AletrisSandbox/avoidanceController"
avoidanceController.depth = -8500
avoidanceController.texture = "objects/AletrisSandbox/avoidanceController/lonn"

avoidanceController.fieldInformation = {
    luaData = { fieldType = "string", default = "" },
    debugMode = { fieldType = "boolean", default = "" }
}

avoidanceController.fieldOrder = { "x", "y", "luaData", "debugMode" }

avoidanceController.placements = {
    name = "Avoidance Controller",
    placementType = "point",
    data = {
        luaData = "",
        debugMode = False,
    }
}

return avoidanceController