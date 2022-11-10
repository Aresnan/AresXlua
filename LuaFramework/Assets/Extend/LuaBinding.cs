using AresLuaExtend.Asset;
using AresLuaExtend.LuaUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace AresLuaExtend
{
	[CSharpCallLua, LuaCallCSharp]
	public class LuaBinding : MonoBehaviour, IRecyclable
	{
		public LuaTable LuaInstance { get; set; }
		public LuaClassCache.LuaClass CachedClass { get; private set; }

		public T GetLuaMethod<T>(string methodName) where T : Delegate
		{
			return CachedClass.GetLuaMethod<T>(methodName);
		}

		public void OnRecycle()
		{
			throw new NotImplementedException();
		}
	}
}
