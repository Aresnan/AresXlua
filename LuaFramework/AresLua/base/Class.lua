--o = o or {} 就是如果 o 为 false 或 nil ，则 o ={ }
function class(super)
	local child_class = {}
	child_class.ctor = {}
	child_class.super = super
	setmetatable(child_class, super)

	child_class.new = function(...)
		local child_class_instance = {}
		local meta = { __index = child_class }
		setmetatable(child_class_instance, meta)
		do
			local recursive
			recursive = function(r, ...)
				if r.super then
					recursive(r.super, ...)
				end
				if r.ctor then
					r.ctor(child_class_instance, ...)
				end
			end
			recursive(child_class, ...)
		end
		return child_class_instance
	end
	return child_class
end
