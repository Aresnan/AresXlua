using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Common
{
	//TODO
	public class VersionService : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.VERSION;

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
