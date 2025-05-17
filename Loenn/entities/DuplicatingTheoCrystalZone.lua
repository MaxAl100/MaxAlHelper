local duplicatingTheoCrystalZone = {}

duplicatingTheoCrystalZone.name = "MaxAlHelper/DuplicatingTheoCrystalZone"
duplicatingTheoCrystalZone.depth = -100 -- Corresponds to Depths.FakeWalls in the C# code
duplicatingTheoCrystalZone.nodeLimits = {1, -1}
duplicatingTheoCrystalZone.nodeVisibility = "selected"

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
    }
}

duplicatingTheoCrystalZone.placements = {
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
        removeOriginal = false
    }
}

-- Display information for Loenn editor
function duplicatingTheoCrystalZone.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 32, entity.height or 32
    
    -- Base color similar to the C# code's Light Gray
    local baseColor = {0.75, 0.75, 0.75, 0.25}
    local borderColor = {0.5, 0.5, 0.5, 1.0}
    
    -- Create visual representation
    local rectangle = require('structs.drawable_rectangle')
    local text = require('structs.drawable_text')
    local drawableLine = require('structs.drawable_line')
    
    local elements = {
        -- Background fill
        rectangle.fromRectangle("fill", x, y, width, height, baseColor),
        -- Border
        rectangle.fromRectangle("line", x, y, width, height, borderColor),
    }
    
    -- Add text description
    table.insert(elements, text.fromData({
        text = "Duplicating Theo Zone",
        x = x + width / 2,
        y = y + height / 2,
        alignX = 0.5,
        alignY = 0.5,
        font = "smallFont",
        color = {1.0, 1.0, 1.0, 1.0}
    }))
    
    -- Additional information about settings
    local gens = entity.maxGenerations or 1
    local info = string.format("Gen: %d", gens)
    table.insert(elements, text.fromData({
        text = info,
        x = x + width / 2,
        y = y + height / 2 + 10,
        alignX = 0.5,
        alignY = 0.5,
        font = "pixelFont",
        color = {1.0, 1.0, 1.0, 0.8}
    }))
    
    -- Draw arrows if there's movement defined
    if (entity.speedX and entity.speedX ~= 0) or (entity.speedY and entity.speedY ~= 0) then
        local centerX, centerY = x + width / 2, y + height / 2
        local speedX, speedY = entity.speedX or 0, entity.speedY or 0
        local length = math.min(width, height) * 0.4
        local normalizedLen = math.sqrt(speedX * speedX + speedY * speedY)
        
        if normalizedLen > 0 then
            local dirX, dirY = speedX / normalizedLen, speedY / normalizedLen
            table.insert(elements, drawableLine.fromPoints(
                centerX, centerY,
                centerX + dirX * length, centerY + dirY * length,
                {1.0, 0.5, 0.0, 1.0}, 2
            ))
        end
    end
    
    return elements
end

return duplicatingTheoCrystalZone