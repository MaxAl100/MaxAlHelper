local absorbCutsceneStarter = {}

absorbCutsceneStarter.name = "MaxAlHelper/AbsorbCutsceneStarter"
absorbCutsceneStarter.depth = 2000

-- Create intro type options
local introTypeOptions = {}
table.insert(introTypeOptions, "None")
table.insert(introTypeOptions, "Fall")
table.insert(introTypeOptions, "Transition")
table.insert(introTypeOptions, "Respawn")
table.insert(introTypeOptions, "WalkInRight")
table.insert(introTypeOptions, "WalkInLeft")
table.insert(introTypeOptions, "Jump")
table.insert(introTypeOptions, "WakeUp")
table.insert(introTypeOptions, "ThinkForABit")
table.insert(introTypeOptions, "TempleMirrorVoid")

local wipeTypeOptions = {
    {"Angled","Celeste.AngledWipe"},
    {"Dream","Celeste.DreamWipe"},
    {"Curtain","Celeste.CurtainWipe"},
    {"Drop","Celeste.DropWipe"},
    {"Fade","Celeste.FadeWipe"},
    {"Fall","Celeste.FallWipe"},
    {"Heart","Celeste.HeartWipe"},
    {"KeyDoor","Celeste.KeyDoorWipe"},
    {"Mountain","Celeste.MountainWipe"},
    {"Starfield","Celeste.StarfieldWipe"},
    {"Wind","Celeste.WindWipe"},
    {"Spotlight","Celeste.SpotlightWipe"},
    {"Diamond [Femto Helper]","Celeste.Mod.FemtoHelper.Wipes.DiamondWipe"},
    {"Sinewave [Femto Helper]","Celeste.Mod.FemtoHelper.Wipes.SineWipe"},
    {"Square [Femto Helper]","Celeste.Mod.FemtoHelper.Wipes.SquareWipe"},
    {"Bars [Femto Helper]", "Celeste.Mod.FemtoHelper.Wipes.CirclerWipe"},
    {"Diagrid [Femto Helper]", "Celeste.Mod.FemtoHelper.Wipes.DiagridWipe"},
    {"Triangles [Femto Helper]", "CliffhangerWipe"},
    -- Didn't manage to get these working:
    -- {"Arrow [Jungle Helper + Maddie's Helping Hand]", "MaxHelpingHand/CustomWipe:JungleHelper/Arrow"},
    -- {"Dots [Jungle Helper + Maddie's Helping Hand]", "MaxHelpingHand/CustomWipe:JungleHelper/Dots"},
    -- {"Rocks [Jungle Helper + Maddie's Helping Hand]", "MaxHelpingHand/CustomWipe:JungleHelper/Rocks"},
    -- {"Spinners [Jungle Helper + Maddie's Helping Hand]", "MaxHelpingHand/CustomWipe:JungleHelper/Speen"},
    -- {"Vines [Jungle Helper + Maddie's Helping Hand]", "MaxHelpingHand/CustomWipe:JungleHelper/Vines"}
}

absorbCutsceneStarter.fieldInformation = {
    targetRoom = {
        fieldType = "MaxAlHelper.room_names"
    },
    introType = {
        options = introTypeOptions,
        editable = false
    },
    targetSpawnId = {
        fieldType = "string"
    },
    walkAroundDistance = {
        fieldType = "integer",
        minimumValue = 0
    },
    targetAbsorbPositionX = {
        fieldType = "integer"
    },
    targetAbsorbPositionY = {
        fieldType = "integer"
    },
    animationSpeedMultiplier = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10.0
    },
    walkBelowTeleport = {
        fieldType = "boolean"
    },
    targetRoomTransitionX = {
        fieldType = "integer"
    },
    targetRoomTransitionY = {
        fieldType = "integer"
    },
    zoomIn = {
        fieldType = "boolean"
    },
    setFlag = {
        fieldType = "string"
    },
    wipeType = {
        options = wipeTypeOptions,
        editable = false
    }
}

absorbCutsceneStarter.placements = {
    {
        name = "normal",
        data = {
            targetRoom = "",
            targetSpawnId = "",
            walkAroundDistance = 12,
            introType = "WalkInRight",
            targetAbsorbPositionX = 0,
            targetAbsorbPositionY = 0,
            animationSpeedMultiplier = 1.0,
            walkBelowTeleport = true,
            targetRoomTransitionX = 0,
            targetRoomTransitionY = 0,
            zoomIn = true,
            setFlag = "",
            wipeType = "Celeste.SpotlightWipe",
        }
    }
}

return absorbCutsceneStarter