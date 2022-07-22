local SM = require "Services.ServiceManager"
local TS = require "Services.TickService"
local M = {}

SM.RegisterService(SM.SERVICE_TYPE.TICK, TS)


function M:Init(flag)
	print(flag)
end

return M