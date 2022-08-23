local Account = require("Game.Network.ServerEndData.Account")
local NPC = require("Game.Network.ServerEndData.NPC")
local Player = require("Game.Network.ServerEndData.Player")
local LuaSocketClient = require("Game.Network.LuaSocketClient")
local SM = require("ServiceManager")
local M = {}
local socketClient


function M.Init()
	Account.Init()
	NPC.Init()
	Player.Init()
end

function M.Tick()
	if socketClient ~= nil then
		socketClient:Tick()
	end
end

function M:Login()
	if socketClient then
		socketClient:Close()
	else
		socketClient = LuaSocketClient.new()
		--socketClient:RegisterServerRequest("SPlayerState", Player.UpdatePlayerState)
		local serverAddress = CS.AresLuaExtend.GameSystemSetting.Get().SystemSetting:GetString("GAME", "Server")
		socketClient:ConnectAsync(serverAddress)
	end
end

function M.GetSocket()
	return socketClient
end

function M.GetAccountData()
	return Account.GetData()
end

function M.GetNPCData()
	return NPC.GetData()
end

function M.GetPlayerData()
	return Player.GetData()
end

--与服务器同步时间
local serverTimeSyncTime
local serverTime
function M.GetServerTime()
	local diff = SM.GetTickService().realtimeSinceStartup - serverTimeSyncTime
	return serverTime + diff
end

function M.SetServerTime(utcTimeInMilliSeconds, delayInSeconds)
	serverTimeSyncTime = SM.GetTickService().realtimeSinceStartup
	serverTime = utcTimeInMilliSeconds / 1000 + delayInSeconds
end

function M.clear()
end

_ServerEndData = M
return M