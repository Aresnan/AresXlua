using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Common
{
	//TODO
	public class GlobalCoroutineRunnerService : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.COROUTINE_SERVICE;

		public void Destroy()
		{
			throw new System.NotImplementedException();
		}

		public void Initialize()
		{
			throw new System.NotImplementedException();
		}
	}
}
