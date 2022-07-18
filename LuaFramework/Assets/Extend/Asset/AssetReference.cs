using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace AresLuaExtend.Asset
{
    //TODO
    [CSharpCallLua]
    public delegate void OnInstantiateComplete(GameObject go);

	[Serializable, LuaCallCSharp]
	public class AssetReference : IDisposable
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
