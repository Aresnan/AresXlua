---@class MockService
local M = {}

---@class MockContext table key protocName value args
---@type table<string, MockContext>
local responseMocks = {}
---@type table<string, MockContext>
local requestMocksAfterRequest = {}

function M.Init()
end

return M