using AresLuaExtend.Common;
using AresLuaExtend.Update.AsyncOperation;
using AresLuaExtend.Update.Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update
{
	public enum UpdateStatus
	{
		None,
		StartUpdate,
		ConnectFailed
	}

	public class VersionManager : MonoBehaviour
	{
		VersionService _versionService;
		UpdaterOperation[] _allOperations;
		UpdateStatus _status;

		static readonly List<AsyncOperationBase> _operations = new List<AsyncOperationBase>(10);
		void Start()
		{
			//初始化VersionService
			_versionService = new VersionService();
			_versionService.Initialize();
			//初始化operation
			_allOperations = new UpdaterOperation[]
			{
				new CheckVersionOperation(_versionService),//获取服务器的版本号等信息并存在VersionService
				new GetLuaSizeOperation(_versionService),//获取Lua文件的Size
				new GetResSizeOperation(_versionService),//TODO
				new NoWifiOperation(_versionService),//获取WIFI信息
				new LuaUpdateOperation(_versionService),//下载Lua代码，并替换源文件
				new ResUpdateOperation(_versionService)//下载Res资源
			};
			//检测版本
			CheckVersion();
		}

		private void CheckVersion()
		{
			if (_status == UpdateStatus.StartUpdate)
			{
				Debug.LogWarning("already is in CheckVersion");
				return;
			}
			Debug.LogWarning("Start CheckVersion");
			StartCoroutine(CheckVersionCoroutine());
		}
		private IEnumerator CheckVersionCoroutine()
		{
			_status = UpdateStatus.StartUpdate;
			foreach (var operation in _allOperations)
			{
				if (operation.Status == EUpdateOperationStatus.NeedWifi)
				{
					yield return operation.CheckWifi();
					if (operation.Status == EUpdateOperationStatus.Succeed)
					{
						continue;
					}
					if (operation.Status == EUpdateOperationStatus.NoWifi)
					{

					}
				}
				//每个Start都是一个协程
				yield return operation.Start();
			}
			if (CheckAllOperation())
			{
				//所有Operation完成后，进入game
				EnterGame("All Operation Ok");
			}
			else
			{

			}
		}
		private bool CheckAllOperation()
		{
			foreach (var operation in _allOperations)
			{
				if (operation.IsReady)
					continue;
				else
				{
					return false;
				}
			}
			return true;
		}

		public static void StartOperation(AsyncOperationBase operationBase)
		{
			_operations.Add(operationBase);
			operationBase.Start();
		}

		private void EnterGame(string sender)
		{
			_versionService.SaveVersion(VersionService.PLATFROM_KEY);
			Debug.LogWarning($"Begin EnterGame  {sender}");
			Dispose();
			_versionService.StartLua();
		}
		void Update()
		{
			//遍历operations，刷新UI的状态显示
			if (_operations.Count > 0)
			{
				for (int i = _operations.Count - 1; i >= 0; i--)
				{
					var operation = _operations[i];
					operation.Update();
					if (operation.IsDone)
					{
						_operations.RemoveAt(i);
						operation.Finish();
					}
				}
			}
		}
		public void Dispose()
		{
			foreach (var op in _allOperations)
			{
				op.Release();
			}
		}
	}
}