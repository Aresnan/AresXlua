---@class Game.Application
local M = class()
local SM = require("Services.ServiceManager")
local SEDService = require("Game.NetWork.ServerEndData.ServerEndDataService")
local GV = CS.AresLua.GlobalVariable
local NS = SM.GetService(SM.SERVICE_TYPE.NATIVE)

function M:ctor()
	_APP = self
	--通过unity宏定义控制
	_APP.native = GV.NATIVE
	_APP.prod = GV.PROD
	_APP.platform = GV.PLATFORM
	-------------------------------
	_APP_VERSION = CS.AresLuaExtend.Common.VersionService.CSVERSION
	_APP_LUA_VERSION = CS.AresLuaExtend.Common.VersionService.LUAVERSION
end

function M:Init()
	--注册原生端事件，等等
	NS.AddEvent("Landing", M.Landing, self)
	M:Landing({callFrom = "unity"})
	--update
	local service = SM.GetService(SM.SERVICE_TYPE.TICK)
	service.Register(M.Tick, self)
end

--可以由native调用，也可以自己直接触发
function M:Landing(info)
	--登陆完成
	if info then
		if _APP.native then
			warn("Native Landing")
		else
			warn("Unity Landing")
		end
		--获取用户信息
		local account = _ServerEndData.GetAccountData()
		self:Switch(LandingState.new())
	end
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

--切换state（每一个state就是一个scene对应的状态）
--从这里进入下一个状态，并在Tick方法中执行当前state对应的循环
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
