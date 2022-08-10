local Util = require("Utility.Util")
-- local UV = require("luv")

---@class TickService
local M = {}

local Tickers = {}
local TickerToAdd = {}

local LateTickers = {}
local LateTickerToAdd = {}

local Timeouts = {}
local TimeOutUid = 1

function M.Init()
	-- 弱引用表允许它的key和value被gc回收，通过设置元表的__mode实现，弱引用性质
	-- __mode是一个字符串，如果包含"k"则key是弱引用，如果包含"v"则value是弱引用
	setmetatable(Tickers, { __mode = "k" })
	setmetatable(LateTickers, { __mode = "k" })
end

function M.Register(func, ...)
	local package = table.pack(...)
	-- TickerToAdd[func] = package
	Tickers[func] = package
end

function M.RegisterLate(func, ...)
	local package = table.pack(...)
	-- LateTickerToAdd[func] = package
	LateTickers[func] = package
end

--[Time.time]
--The time at the beginning of this frame
--not update if the Editor is paused
-- Time.timeScale scales and Time.maximumDeltaTime adjusts
--This value is undefined during Awake messages and starts after all of these messages are finished
--[Time.realtimeSinceStartup]
-- Time.timeScale does not affect this property
--[Time.timeSinceLevelLoad]
--This is the time in seconds since the last non-additive scene has finished loading
function M.Tick()
	UV.run("nowait")
	--同步unity时间
	local Time = CS.UnityEngine.Time
	M.deltaTime = Time.deltaTime
	M.totalTime = Time.time
	M.realtimeSinceStartup = Time.realtimeSinceStartup
	M.timeSinceLevelLoad = Time.timeSinceLevelLoad
	--防止对迭代器操作
	-- for func, pack in pairs(TickerToAdd) do
	-- 	Tickers[func] = pack
	-- end
	-- TickerToAdd = {}
	for func, packed in pairs(Tickers) do
		local ok
		if packed.n ~= 0 then
			ok = Util.xpcall_catch(func, table.unpack(packed))
		else
			ok = Util.xpcall_catch(func)
		end
		if not ok then
			M.Unregister(func)
		end
	end
	--timeout
	for uid, timeout in pairs(Timeouts) do
		timeout.timeToTrigger = timeout.timeToTrigger - M.deltaTime
		if timeout.timeToTrigger < 0 then
			local ok, complete
			if timeout.params.n ~= 0 then
				ok, complete = util.xpcall_catch(timeout.callback, table.unpack(timeout.params))
			else
				ok, complete = util.xpcall_catch(timeout.callback)
			end

			if timeout.repeatTimes > 0 then
				timeout.repeatTimes = timeout.repeatTimes - 1
			end

			if not ok then
				error(complete)
				Timeouts[uid] = nil
			elseif complete or timeout.repeatTimes == 0 then
				Timeouts[uid] = nil
			else
				timeout.timeToTrigger = timeout.interval
			end
		end
	end
end

function M.LateTick()
	-- for func, pack in pairs(LateTickerToAdd) do
	-- 	LateTickers[func] = pack
	-- end
	-- LateTickerToAdd = {}

	for func, packed in pairs(LateTickers) do
		local ok
		if packed.n ~= 0 then
			ok = util.xpcall_catch(func, table.unpack(packed))
		else
			ok = util.xpcall_catch(func)
		end

		if not ok then
			M.UnregisterLate(func)
		end
	end
end

---@param seconds number 超时时间
---@param repeatTimes integer 重复次数， -1无限重复
---@return function 调用后移除
function M.Timeout(seconds, repeatTimes, callback, ...)
	--start:几秒后执行; interval：几秒执行一次
	local start, interval
	if type(seconds) == "table" then
		start = seconds.start
		interval = seconds.interval
	else
		start = seconds
		interval = seconds
	end
	TimeOutUid = TimeOutUid + 1
	Timeouts[TimeOutUid] = {
		timeToTrigger = start,
		interval = interval,
		repeatTimes = repeatTimes,
		callback = callback,
		params = table.pack(...)
	}
	--手动注销，比如-1无限重复，可以通过此方式终止
	local uid = TimeOutUid
	return function()
		Timeouts[uid] = nil
	end
end

--给全局变量或者 table 表里的变量赋一个 nil 值，等同于把它们删掉
function M.Unregister(func)
	TickerToAdd[func] = nil
	Tickers[func] = nil
end

function M.UnregisterLate(func)
	LateTickerToAdd[func] = nil
	LateTickers[func] = nil
end

return M
