using AresLuaExtend.Common;
using AresLuaExtend.Update.AsyncOperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{
    public class GetResSizeOperation : GetSizeOperation
    {
		public GetResSizeOperation(VersionService service) : base(service)
		{
		}
		public override string GetUpdateInfo()
		{
			return "获取岛屿资源信息中...";
		}
		public static long DownloadSize { get; protected set; }
		public override IEnumerator Start()
		{
			DownloadSize = 0;
			if (_versionService.CompareVersion(VersionService.RES_KEY))
			{
				var catalogCachePath = Application.persistentDataPath;
				Debug.LogWarning($"Res Update ");
				var updateRes = new UpdateCatalogOperation(catalogCachePath);
				VersionManager.StartOperation(updateRes);
				yield return updateRes;
				if (updateRes.Status == EOperationStatus.Succeed)
				{
					Debug.LogWarning($"need download size is {updateRes.TotalDownloadSize}");
					DownloadSize = updateRes.TotalDownloadSize;
					TotalDownloadSize += updateRes.TotalDownloadSize;
					Status = EUpdateOperationStatus.Succeed;
				}
				else
				{
					Debug.LogWarning("GetResSizeOperation DownloadFailed");
					Status = EUpdateOperationStatus.DownloadFailed;
				}
			}
			else
			{
				Status = EUpdateOperationStatus.Succeed;
			}
		}
	}
}
