using AresLuaExtend.Common;
using AresLuaExtend.Update.AsyncOperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{

	public class CheckVersionOperation : UpdaterOperation
	{
		private VersionService _versionService;

		public CheckVersionOperation(VersionService service)
		{
			_versionService = service;
		}

		public override string GetUpdateInfo()
		{
			return "获取版本信息中...";
		}

		public override IEnumerator Start()
		{
			TotalDownloadSize = 0;
			var requestOperation = new RequestServerVersionOperation(_versionService);
			VersionManager.StartOperation(requestOperation);
			yield return requestOperation;
			if (requestOperation.Status == EOperationStatus.Succeed)
			{
				Debug.LogWarning($"requestOperation is succeed result is {requestOperation.Result}");
				//这里是核心，将服务器的Result存到VersionService里
				_versionService.ParseServerInfo(requestOperation.Result);
			}
			else
			{
				Debug.LogWarning($"requestOperation is succeed result is {requestOperation.Result}"); 
				if (requestOperation.Error == "NotMatch")
				{
					Debug.LogWarning($"server deviece id is not match");
					Status = EUpdateOperationStatus.EnterGame;
				}
				else
				{
					Debug.LogWarning("CheckVersionOperation ConnectFailed");
					Status = EUpdateOperationStatus.ConnectFailed;
				}
				yield break;
			}

			Status = EUpdateOperationStatus.Succeed;
		}
	}
}