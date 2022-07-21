local M = {}
print("lua start")
function M.Init(callback)
	print("this is test lua")
	callback("this is callback")
	return "this is return"
end

function M.Test(str)
	print("Test function params is " .. str)
	return "dddd"
end

return M
