using AresLuaExtend.Asset;
using AresLuaExtend.Common;
using AresLuaExtend.DebugUtil;
using AresLuaExtend.LuaUtil;
using AresLuaExtend.Network;
using AresLuaExtend.Render;
using AresLuaExtend.SceneManagement;
using AresLuaExtend.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend
{
	public static class StaticServiceInitializer
	{
		//This is after the Awake method has been invoked
		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void OnInitAssembliesLoaded()
		{
			CSharpServiceManager.Initialize();
			//注册其他相关服务
			CSharpServiceManager.Register(new ErrorLogToFile());
			CSharpServiceManager.Register(new StateService());
			CSharpServiceManager.Register(new AssetService());
			CSharpServiceManager.Register(new GameSystemSetting());
			CSharpServiceManager.Register(new DownLoadService());
		}

		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnInitBeforeSceneLoad()
		{
			Application.runInBackground = true;
			//注册其他相关服务
			CSharpServiceManager.Register(new LuaVM());
			CSharpServiceManager.Register(new TickService());
			CSharpServiceManager.Register(new I18nService());
			CSharpServiceManager.Register(new SceneLoadManager());
			CSharpServiceManager.Register(new RenderFeatureService());
			CSharpServiceManager.Register(new SpriteAssetService());
		}

		//[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void OnSceneLoaded()
		{
			CSharpServiceManager.InitializeServiceGameObject();
			//注册其他相关服务
			CSharpServiceManager.Register(new NetworkService());
			CSharpServiceManager.Register(new GlobalCoroutineRunnerService());
			CSharpServiceManager.Register(new VersionService());

			Application.targetFrameRate = 60;
		}
	}
}
