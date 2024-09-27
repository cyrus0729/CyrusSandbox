local InteractiveEntity = {}

InteractiveEntity.name = "CyrusSandbox/InteractiveEntity"
InteractiveEntity.depth = -8500

InteractiveEntity.placements = {
    {
        name = "Interactive Entity",
        data = {
            TalkSound = "",
            TalkEndSound = "",
            TalkDuration = 2,
            MakeHairInvisible = false,
            Spacing = 0,
            ShowIndicator = true,
            playerAnimation = "plrPetting",
            talkAnimation = "sunnyPetting",
            idleAnimation = "sunnyIdle",
            flag = ""
        }
    }
}

function InteractiveEntity.texture(room, entity)
    local sprite = "characters/CyrusSandbox/InteractiveEntity/sunny/idle00"
    return sprite
end

return InteractiveEntity