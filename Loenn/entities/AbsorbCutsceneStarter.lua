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
            targetAbsorbPositionX = 200,
            targetAbsorbPositionY = 100,
            animationSpeedMultiplier = 1,
            walkBelowTeleport = true,
        }
    }
}

return absorbCutsceneStarter
