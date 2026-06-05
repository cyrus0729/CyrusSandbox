local PlaySoundTrigger = {}

PlaySoundTrigger.name = "AletrisSandbox/PlaySoundTrigger"

PlaySoundTrigger.fieldInformation = {
    width = { fieldType = "integer", default = 8 },
    height = { fieldType = "integer", default = 8 },
    soundId = { fieldType = "string", default = "" },
    pitch = { fieldType = "integer", default = 60 },
    volume = { fieldType = "number", default = 1.0 },
    atPlayer = { fieldType = "boolean", default = False },
}

PlaySoundTrigger.fieldOrder = { "x", "y", "width", "height", "soundId", "pitch", "volume", "atPlayer" }

PlaySoundTrigger.placements = {
    name = "Play Sound",
    data = {
        soundId = "",
        pitch = 60,
        volume = 1.0,
        atPlayer = False,
    },
}
return PlaySoundTrigger