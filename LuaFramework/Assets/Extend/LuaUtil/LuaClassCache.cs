using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLua;

namespace AresLuaExtend.LuaUtil
{
	public class LuaClassCache
	{
		public class LuaClass
		{
			public LuaTable ClassMetaTable;
			public readonly List<LuaTable> ChildClasses = new List<LuaTable>();
			private readonly Dictionary<string, Delegate> m_cachedMethods = new Dictionary<string, Delegate>();

			public T GetLuaMethod<T>(string methodName) where T : Delegate
			{
				if (m_cachedMethods.TryGetValue(methodName, out var m))
				{
					return m as T;
				}

				var luaMethod = ClassMetaTable.GetInPath<T>(methodName);
				m_cachedMethods.Add(methodName, luaMethod);
				return luaMethod;
			}
		}
	}
}
