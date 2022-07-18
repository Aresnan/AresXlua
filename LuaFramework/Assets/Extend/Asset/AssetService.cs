using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Asset
{
	//TODO
	public class AssetService : IService, IServiceUpdate
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.ASSET_SERVICE;

		public void Destroy()
		{
			throw new System.NotImplementedException();
		}

		public void Initialize()
		{
			throw new System.NotImplementedException();
		}

		public void Update()
		{
			throw new System.NotImplementedException();
		}
	}
}
