local duplicatingTheoCrystal = {}

duplicatingTheoCrystal.name = "MaxAlHelper/DuplicatingTheoCrystal"
duplicatingTheoCrystal.depth = 100
duplicatingTheoCrystal.texture = "characters/theoCrystal/idle00"

duplicatingTheoCrystal.placements = {
    {
        name = "normal",
        data = {
            canDuplicateMultipleTimes = false,
            canClonesDuplicate = false,
            maxGenerations = 1,
            timeBetweenDuplications = 1.0,
            spritePaths = "theo_crystal", -- comma-separated list of sprites
            currentGeneration = 0
        }
    }
}

-- Custom field information for better editor support
duplicatingTheoCrystal.fieldInformation = {
    spritePaths = {
        fieldType = "string",
        editable = true
    },
    timeBetweenDuplications = {
        fieldType = "number",
        minimumValue = 0.5
    },
    maxGenerations = {
        fieldType = "integer",
        minimumValue = 0
    }
}

-- Explanation when hovering
duplicatingTheoCrystal.fieldOrder = {
    "canDuplicateMultipleTimes", "canClonesDuplicate", "maxGenerations", 
    "timeBetweenDuplications", "spritePaths", "currentGeneration"
}

return duplicatingTheoCrystal