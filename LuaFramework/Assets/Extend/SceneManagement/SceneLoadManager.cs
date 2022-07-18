using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.SceneManagement
{
	//TODO
	public class SceneLoadManager : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.SCENE_LOAD;

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
