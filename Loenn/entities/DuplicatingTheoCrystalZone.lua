local duplicatingTheoCrystalZone = {}

duplicatingTheoCrystalZone.name = "MaxAlHelper/DuplicatingTheoCrystalZone"
duplicatingTheoCrystalZone.depth = -100

duplicatingTheoCrystalZone.fieldInformation = {
    maxGenerations = {
        fieldType = "integer",
        minimumValue = 1,
        defaultValue = 1
    },
    timeBetweenDuplications = {
        fieldType = "number",
        minimumValue = 0.1,
        defaultValue = 1.0
    },
    -- Clone settings
    offsetX = {
        fieldType = "number",
        defaultValue = 0.0
    },
    offsetY = {
        fieldType = "number",
        defaultValue = 0.0
    },
    speedX = {
        fieldType = "number",
        defaultValue = 0.0
    },
    speedY = {
        fieldType = "number",
        defaultValue = 0.0
    },
    -- Original theo settings
    originalOffsetX = {
        fieldType = "number",
        defaultValue = 0.0
    },
    originalOffsetY = {
        fieldType = "number",
        defaultValue = 0.0
    },
    originalSpeedX = {
        fieldType = "number",
        defaultValue = 0.0
    },
    originalSpeedY = {
        fieldType = "number",
        defaultValue = 0.0
    },
    -- Other settings
    maxTriggers = {
        fieldType = "integer",
        minimumValue = 0,
        defaultValue = 0
    },
    spritePaths = {
        fieldType = "string",
        editable = true
    }
}

duplicatingTheoCrystalZone.placements = {
    {
        name = "normal",
        data = {
            width = 32,
            height = 32,
            canDuplicateMultipleTimes = false,
            canClonesDuplicate = false,
            maxGenerations = 1,
            timeBetweenDuplications = 1.0,
            spritePaths = "",
            -- Clone settings
            offsetX = 0.0,
            offsetY = 0.0,
            speedX = 0.0,
            speedY = 0.0,
            -- Original theo settings
            originalOffsetX = 0.0,
            originalOffsetY = 0.0,
            originalSpeedX = 0.0,
            originalSpeedY = 0.0,
            bounceBack = false,
            bounceBackUseDirection = true,
            removeOriginal = false,
            maxTriggers = 0
        }
    }
}

function duplicatingTheoCrystalZone.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 32, entity.height or 32
    
    -- Use standard Lönn drawing approach
    local drawableRectangle = require("structs.drawable_rectangle")
    
    -- Light blue colors
    local fillColor = {0.68, 0.85, 1.0, 0.25}
    local borderColor = {0.68, 0.85, 1.0, 1.0}
    
    return {
        drawableRectangle.fromRectangle("fill", x, y, width, height, fillColor),
        drawableRectangle.fromRectangle("line", x, y, width, height, borderColor)
    }
end

function duplicatingTheoCrystalZone.selection(room, entity)
    return utils.rectangle(entity.x or 0, entity.y or 0, entity.width or 32, entity.height or 32)
end

-- Organize fields in a logical order
duplicatingTheoCrystalZone.fieldOrder = {
    "x", "y", "width", "height",
    -- Core duplication settings
    "canDuplicateMultipleTimes", "canClonesDuplicate", "maxGenerations", 
    "timeBetweenDuplications", "maxTriggers",
    -- Clone settings
    "offsetX", "offsetY", "speedX", "speedY", 
    -- Original theo settings
    "originalOffsetX", "originalOffsetY", "originalSpeedX", "originalSpeedY",
    "bounceBack", "bounceBackUseDirection", "removeOriginal",
    -- Visual settings
    "spritePaths"
}

return duplicatingTheoCrystalZone