using AresLuaExtend.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Common
{	
	public class VersionService : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.VERSION;
		//This function returns the current version of the Application. This is read-only. To set the version number in Unity, go to Edit > Project Settings > Player.
		public static readonly string CSVERSION = Application.version;
		public static readonly string LUAVERSION = "0.0.0";

		public void Destroy()
		{
			
		}

		public void Initialize()
		{
			if (CompareLuaVersion())
			{
				Debug.Log("-----download lua file...");
				DownloadLuaFile();
			}
			var co_service = CSharpServiceManager.Get<GlobalCoroutineRunnerService>(CSharpServiceManager.ServiceType.COROUTINE_SERVICE);
			var asset_service = CSharpServiceManager.Get<AssetService>(CSharpServiceManager.ServiceType.ASSET_SERVICE);
			co_service.StartCoroutine(asset_service.InitAddressableCoroutine((_) =>
			{
				Debug.Log("update addressable complete ");
			}));
		}

		private bool CompareLuaVersion()
		{
			return true;
		}
		private void DownloadLuaFile()
		{
			
		}
	}
}
