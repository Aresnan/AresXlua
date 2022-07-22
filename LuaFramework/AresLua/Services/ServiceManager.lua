local M = {}
local services = {}
M.SERVICE_TYPE = {
	CONFIG = 1,
	TICK = 2,
	CONSOLE_COMMAND = 3,
	UI = 4,
	GLOBAL_VM = 5,
	MOCK = 6,
	GLOBAL_EVENT = 7,
	EVENT_BINDING = 8,
	SERVER_END_DATA = 9,
	TRACK = 10,
	NATIVE = 11
}
function M.GetService(type)
	print("service type is " .. type)
	return services[type]
end

function M.RegisterService(type, service)
	assert(service)
	service.Init()
	services[type] = service
end
function M.UnregisterService(type)
	services[type] = nil
end
_ServiceManager = M

return M
