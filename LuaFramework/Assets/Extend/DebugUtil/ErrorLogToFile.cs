using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.DebugUtil
{
	//TODO
	public class ErrorLogToFile : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.ERROR_LOG_TO_FILE;

		public void Destroy()
		{
			
		}

		public void Initialize()
		{
			
		}
	}
}
