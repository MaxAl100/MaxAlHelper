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
            offsetX = 0.0,
            offsetY = 0.0,
            speedX = 0.0,
            speedY = 0.0,
            bounceBack = false,
            removeOriginal = false,
            maxTriggers = 0
        }
    }
}

function duplicatingTheoCrystalZone.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 32, entity.height or 32
    
    local baseColor = {0.75, 0.75, 0.75, 0.25}
    local borderColor = {0.5, 0.5, 0.5, 1.0}
    
    local rectangle = require('structs.drawable_rectangle')
    
    local elements = {
        rectangle.fromRectangle("fill", x, y, width, height, baseColor),
        rectangle.fromRectangle("line", x, y, width, height, borderColor)
    }
    
    -- Generation info
    local gens = entity.maxGenerations or 1
    local triggers = entity.maxTriggers or 0
    local info = string.format("Gens: %d", gens)
    if triggers > 0 then
        info = info .. string.format("\nTriggers: %d", triggers)
    end
    
    return elements
end

-- If selection is causing issues, let's define a selection function
function duplicatingTheoCrystalZone.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 32, entity.height or 32
    
    return utils.rectangle(x, y, width, height)
end

-- duplicatingTheoCrystalZone.fieldOrder = {
--     "x", "y", "width", "height",
--     "canDuplicateMultipleTimes", "canClonesDuplicate", "maxGenerations", 
--     "timeBetweenDuplications", "spritePaths", "offsetX", "offsetY",
--     "speedX", "speedY", "bounceBack", "removeOriginal", "maxTriggers"
-- }

return duplicatingTheoCrystalZone