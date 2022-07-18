using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Network
{
	//TODO
	public class DownLoadService : IService, IServiceUpdate
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.DOWNLOAD;

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
