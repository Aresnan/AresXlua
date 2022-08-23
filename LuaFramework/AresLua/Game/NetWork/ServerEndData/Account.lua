local M = {}
local account = {
	userId = "000",
	name = "",
	image = "EmptyUser",
	state = "Online",
	gender = "male",
	appVersion = _APP_VERSION,
}

function M.Init()
	print("init account")
end

function M.GetData()
	return account
end

function M.ParseFromServerData(serverData)
	account.userId = serverData["userId"]
	account.name = serverData["name"]
	account.image = serverData["image"]
end

return M