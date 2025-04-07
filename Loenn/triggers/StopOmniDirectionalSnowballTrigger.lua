local stopOmniDirectionalSnowballTrigger = {}

-- remember to match the name with the CustomEntity plugin name
stopOmniDirectionalSnowballTrigger.name = "MaxAlHelper/StopOmniDirectionalSnowballTrigger"

-- here you deleted the line which has data = {}
-- this causes a syntax error and i'm pretty sure lönn does *not* like that
stopOmniDirectionalSnowballTrigger.placements = {
    name = "normal",
    data = {}
}

return stopOmniDirectionalSnowballTrigger
