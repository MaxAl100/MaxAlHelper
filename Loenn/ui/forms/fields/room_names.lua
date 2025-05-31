local stringField = require("ui.forms.fields.string")
local utils = require("utils")
local loadedState = require("loaded_state")

local room_names = {}
room_names.fieldType = "MaxAlHelper.room_names"

function room_names.getElement(name, value, options)
    -- Add extra options and pass it onto string field
    local roomNames = {}
    

    if loadedState.map then
        for _, room in ipairs(loadedState.map.rooms) do
            table.insert(roomNames, room.name:match("^lvl_(.*)") or room.name)
        end
    end

    options.options = roomNames

    options.validator = function(v)
        local editedRoom = options.editedRoom
        local currentName = v
        if not currentName or currentName == "" then
            return options.allowEmpty
        end
        for _, room in ipairs(roomNames) do 
            if currentName == room then return true end
        end
        return false
    end
    return stringField.getElement(name, value, options)
end

return room_names