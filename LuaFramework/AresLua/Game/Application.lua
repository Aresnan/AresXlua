---@class Game.Application
local M = class()
local SM = require("Services.ServiceManager")
local SEDService = require("Game.NetWork.ServerEndData.ServerEndDataService")
local GV = CS.AresLua.GlobalVariable
local NS = SM.GetService(SM.SERVICE_TYPE.NATIVE)

function M:ctor()
	_APP = self
	_APP.native = GV.NATIVE
	_APP.prod = GV.PROD
	_APP.platform = GV.PLATFORM
	_APP_VERSION = CS.AresLuaExtend.Common.VersionService.CSVERSION
	_APP_LUA_VERSION = CS.AresLuaExtend.Common.VersionService.LUAVERSION
end

function M:Init()
	--注册原生端事件，等等
	NS.AddEvent("Landing", M.Landing, self)
	--update
	local service = SM.GetService(SM.SERVICE_TYPE.TICK)
	service.Register(M.Tick, self)
end

function M:Landing()
	--登陆完成
end

function M:Tick()
	if self.currentState and self.currentState:GetIsReady() then
		self.currentState:Update()
		local ok, err = xpcall(SEDService.Tick, debug.traceback)
		if not ok then
			error(err)
		end
	end
end

function M:GetCurrentState()
	return self.currentState
end

function M:Switch(state, ...)
	if self.currentState ~= state then
		if self.currentState then
			self.currentState:PrepareExit()
			self.currentState:Exit()
			self.currentState = nil
		end
		self.currentState = state
		if self.currentState then
			self.currentState:PrepareEnter()
			self.currentState:Enter(...)
		end
	end
end

function M:Shutdown()
	self.currentState:Exit()
	NS.RemoveEvent("Landing", M.Landing)
	local socket = SEDService.GetSocket()
	if socket ~= nil then
		socket:Close()
	end
end
