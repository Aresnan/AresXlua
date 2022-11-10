using AresLuaExtend.Asset;
using AresLuaExtend.LuaBindingEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace AresLuaExtend.HadesBinding
{
	[RequireComponent(typeof(ScrollRect)), LuaCallCSharp]
	public class HadesSystemScroll : LuaBindingEventBase, IMVVMAssetReference, ILuaMVVM
	{
		public void Detach()
		{
			throw new NotImplementedException();
		}

		public LuaTable GetDataContext()
		{
			throw new NotImplementedException();
		}

		public AssetReference GetMVVMReference()
		{
			throw new NotImplementedException();
		}

		public void SetDataContext(LuaTable dataSource)
		{
			throw new NotImplementedException();
		}
	}
}
