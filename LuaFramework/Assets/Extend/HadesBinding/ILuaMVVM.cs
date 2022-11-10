using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace AresLuaExtend.HadesBinding
{
	[LuaCallCSharp]
	public interface ILuaMVVM
	{
		void SetDataContext(LuaTable dataSource);
		LuaTable GetDataContext();
		void Detach();
	}
}
