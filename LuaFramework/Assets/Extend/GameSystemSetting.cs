using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend
{
	//TODO
	public class GameSystemSetting : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.GAME_SYSTEM_SERVICE;

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
