using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend
{
	public static class StaticServiceInitializer
	{
		//This is after the Awake method has been invoked
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void OnInitAssembliesLoaded()
		{
			CSharpServiceManager.Initialize();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnInitBeforeSceneLoad()
		{

		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void OnSceneLoaded()
		{

		}
	}
}
