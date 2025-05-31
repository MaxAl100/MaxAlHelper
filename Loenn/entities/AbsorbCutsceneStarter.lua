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
        }
    }
}

return absorbCutsceneStarter