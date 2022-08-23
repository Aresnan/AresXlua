---@class Game.Network.LuaSocketClient
local M = class()
local SocketClient = CS.XiaoIceIsland.Network.SocketClient
local rapidjson = require("rapidjson")
local base64 = require("base64")
local SM = require("ServiceManager")
local TickService = SM.GetTickService()
local SEDService = SM.GetServerEndDataService()

function M:ctor()
	self.serverRequests = {}
	self.client = SocketClient(self)
	TickService = SM.GetTickService()
	self.lastHeartBeatTime = TickService.totalTime
	self.lastConnectedTime = TickService.realtimeSinceStartup
	self.reconnect = false
end

function M:SetOrUpdateHeader(header,data)
	local json = rapidjson.encode(data)
	self.client:SetOrUpdateHeader(header, base64.enc(json))
end

function M:RegisterServerRequest(type, callback, target)
	assert(not self.serverRequests[type], type)
	self.serverRequests[type] = {
		callback = callback,
		target = target
	}
end


function M:UnregisterServerRequest(type)
	self.serverRequests[type] = nil
end

function M:OnStatusChanged(status)
	print("Status", status)
	self.status = status
	_APP:GetCurrentState():SocketStatusChanged(status)
end

function M:GetStatus()
	return self.status
end

function M:ConnectAsync(address)
	self.lastHeartBeatTime = TickService.realtimeSinceStartup
	self.client:ConnectAsync(address)
end

function M:Send(type, data)
	local package = rapidjson.encode({Type = type, Data = data})
	self.client:Send(package)
end

function M:Tick()
	self.client:Tick()
	if TickService.realtimeSinceStartup - self.lastHeartBeatTime > 10 then
		self:Send("CHeartBeat")
		self.lastHeartBeatTime = TickService.realtimeSinceStartup
	end
end

function M:SetPauseSocketEvent(shouldPause)
	self.client:SetPauseSocketEvent(shouldPause)
end

function M:Close()
	self.client:Close()
end