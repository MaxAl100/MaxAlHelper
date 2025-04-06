local omniDirectionalSnowballTrigger = {}

omniDirectionalSnowballTrigger.name = "MaxAlHelper/OmniDirectionalSnowballTrigger"

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
            replaceExisting = true
        }
    }
}

return omniDirectionalSnowballTrigger