local duplicatingTheoCrystal = {}

duplicatingTheoCrystal.name = "MaxAlHelper/DuplicatingTheoCrystal"
duplicatingTheoCrystal.depth = 100
duplicatingTheoCrystal.texture = "characters/theoCrystal/idle00"

duplicatingTheoCrystal.placements = {
    {
        name = "normal",
        data = {
            canDuplicateMultipleTimes = false,
            hasDuplicated = false,
            canClonesDuplicate = false,
            maxGenerations = 1,
            timeBetweenDuplications = 1.0,
            spritePaths = "theo_crystal", -- comma-separated list of sprites
            currentGeneration = 1
        }
    }
}

-- optional: custom field types for better editor support
duplicatingTheoCrystal.fieldInformation = {
    spritePaths = {
        fieldType = "string"
    }
}

return duplicatingTheoCrystal
