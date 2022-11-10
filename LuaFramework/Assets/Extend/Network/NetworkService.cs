using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Network
{
	public enum DownloadStatus
	{
		Success,
		Progressing,
		Wait,
		Failed,
		Canceled
	}
	//TODO
	public class NetworkService : IService, IServiceUpdate
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.NETWORK_SERVICE;

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