local StateBase = require("Game.State.StateBase")
local SM = require("ServiceManager")

---@class Game.State.MainlandState : Game.State.StateBase
local M = class(StateBase)

function M:Enter(...)
	self.ready = false
	--加载当前state对应的场景
	local sceneLoadManager = CS.Extend.SceneManagement.SceneLoadManager.Get()
	sceneLoadManager:LoadSceneAsync("Assets/Scenes/Mainland.unity", false, function()
		self.ready = true
	end)
end

function M:GetStateName()
	return "MainlandState"
end

function M:Update()
end

-- abstract
function M:Exit()
	local sceneLoadManager = CS.Extend.SceneManagement.SceneLoadManager.Get()
	sceneLoadManager:UnloadScene("Assets/Scenes/Mainland.unity")
end

function M:GetIsReady()
	return self.ready
end

return M
