using AresLuaExtend.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Asset
{
	//TODO
	public class AssetService : IService, IServiceUpdate
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.ASSET_SERVICE;
		public IEnumerator InitAddressableCoroutine(Action<bool> onCompelte)
		{
			yield return null;
		}

		public void Destroy()
		{
			
		}

		public void Initialize()
		{
			
		}

		public void Update()
		{
			
		}
	}
}
