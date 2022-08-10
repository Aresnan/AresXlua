local SM = require "Services.ServiceManager"
local ConfigService = require "Services.ConfigService"
local TS = require "Services.TickService"
local MockContext = require "Services.MockService"
local EventBindingService = require "Services.EventBindingService"
local GlobalEventDispatcher = require "Services.GlobalEventDispatcher"
local ServerEndDataService = require "Services.ServerEndDataService"
local NativeService = require "Services.NativeService"
local UIService = require "Services.UIService"
local Binding = require "Mvvm.Binding"

SM.RegisterService(SM.SERVICE_TYPE.CONFIG, ConfigService)
SM.RegisterService(SM.SERVICE_TYPE.TICK, TS)
SM.RegisterService(SM.SERVICE_TYPE.MOCK, MockContext)
SM.RegisterService(SM.SERVICE_TYPE.EVENT_BINDING, EventBindingService)
SM.RegisterService(SM.SERVICE_TYPE.GLOBAL_EVENT, GlobalEventDispatcher)
SM.RegisterService(SM.SERVICE_TYPE.SERVER_END_DATA, ServerEndDataService)
SM.RegisterService(SM.SERVICE_TYPE.NATIVE, NativeService)

local M = {}
local app
function M:Init(flag)
	print(self,flag)
	SM.RegisterService(SM.SERVICE_TYPE.UI, UIService)
	-- Unity
	Binding.SetEnvVariable({
		UnityColor = CS.UnityEngine.Color
	})

	-- Service
	Binding.SetEnvVariable({
		ConfigService = ConfigService
	})

	-- local Application = require("Game.Application")
	-- app = Application.new()
	-- app:Init()

	-- Lua
	Binding.SetEnvVariable(_G)

end

function M:Shutdown()
	print("Shutdown")
end

return M
