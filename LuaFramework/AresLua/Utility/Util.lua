---@class Util
local M = {}
M.traceback = debug.traceback
--调用方法f
function M.xpcall_catch(f, ...)
	if not f then
		return
	end
	local ok, err = xpcall(f, M.traceback, ...)
	if not ok then
		error(err)
	end
	return ok, err
end

util = M
return M
