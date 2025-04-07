local omniDirectionalSnowballTrigger = {}

-- remember to match the name with the CustomEntity plugin name
omniDirectionalSnowballTrigger.name = "MaxAlHelper/OmniDirectionalSnowballTrigger"

-- you don't need an extra layer of braces if you're defining only one placement
-- this is fine though, no need to edit
omniDirectionalSnowballTrigger.placements = {
    {
        name = "normal",
        data = {
            spritePath = "snowball",
            appearAngle = 0.0,
            speed = 200.0,
            resetTime = 0.8,
            ySineWaveFrequency = 0.5,
            safeZoneSize = 64.0,
            offset = 0.0,
            drawOutline = true,
            replaceExisting = true,
            repetitionsForTurn = 0,
            turnAngle = 0.0
        }
    }
}

return omniDirectionalSnowballTrigger
