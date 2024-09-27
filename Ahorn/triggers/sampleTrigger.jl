module CyrusHelperSampleTrigger

using ..Ahorn, Maple

@mapdef Trigger "CyrusHelper/SampleTrigger" SampleTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    sampleProperty::Integer=0
)

const placements = Ahorn.PlacementDict(
    "Sample Trigger (CyrusHelper)" => Ahorn.EntityPlacement(
        SampleTrigger,
        "rectangle",
    )
)

end