using AresLuaExtend.Asset;
using AresLuaExtend.HadesBinding;
using AresLuaExtend.LuaBindingEvent;
using AresLuaExtend.UI.Scroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace AresLuaExtend.HadesBinding
{
	[RequireComponent(typeof(LoopScrollRect))]
	public class HadesLoopScroll : LuaBindingEventBase, ILoopScrollDataProvider, IMVVMAssetReference, ILuaMVVM
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

		public AssetReference ProvideAssetReference(int index)
		{
			throw new NotImplementedException();
		}

		public void ProvideData(Transform t, int index)
		{
			throw new NotImplementedException();
		}

		public void SetDataContext(LuaTable dataSource)
		{
			throw new NotImplementedException();
		}
	}
}
