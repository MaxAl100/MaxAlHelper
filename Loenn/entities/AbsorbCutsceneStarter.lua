local absorbCutsceneStarter = {}

absorbCutsceneStarter.name = "MaxAlHelper/AbsorbCutsceneStarter"
absorbCutsceneStarter.depth = 2000

absorbCutsceneStarter.placements = {
    {
        name = "normal",
        data = {
            targetRoom = "Fill",
            walkAroundDistance = 12,
            introType = "WalkInRight",
            targetAbsorbPositionX = 0,
            targetAbsorbPositionY = 0,
            animationSpeedMultiplier = 1,
            walkBelowTeleport = true,
            targetRoomTransitionX = 0,
            targetRoomTransitionY = 0,
            zoomIn = true,
        }
    }
}

return absorbCutsceneStarter
