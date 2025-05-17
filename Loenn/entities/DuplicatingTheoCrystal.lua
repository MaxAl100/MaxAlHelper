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
            spritePaths = "theo_crystal",
            currentGeneration = 0
        }
    }
}

duplicatingTheoCrystal.fieldInformation = {
    spritePaths = {
        fieldType = "string",
        editable = true,
        options = {"theo_crystal"},
        defaultValue = "theo_crystal"
    },
    timeBetweenDuplications = {
        fieldType = "number",
        minimumValue = 0.5
    },
    maxGenerations = {
        fieldType = "integer",
        minimumValue = 0
    },
    currentGeneration = {
        fieldType = "integer",
        minimumValue = 0
    }
}

duplicatingTheoCrystal.fieldOrder = {
    "canDuplicateMultipleTimes", "canClonesDuplicate", "maxGenerations", 
    "timeBetweenDuplications", "spritePaths", "currentGeneration"
}

return duplicatingTheoCrystal