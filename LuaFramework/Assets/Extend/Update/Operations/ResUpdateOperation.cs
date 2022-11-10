using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace AresLuaExtend.Update.Operations
{
    public class ResUpdateOperation : UpdaterOperation
    {
		private VersionService _versionService;
		private string _tempResPath;

		// The class constructor is called when the class instance is created
		public ResUpdateOperation(VersionService service)
		{
			_versionService = service;
			_tempResPath = $"{Application.persistentDataPath}/ResTemp.json";
		}

		private long totalDownloadSize;
		private const string downloadSpeedInfo = "正在下载 {0}% [{1} MB/{2} MB]\n 速度：{3}MB/秒";
		private long hasDownLoadSize = 0;
		private long _resProgress = 0;
		private string _totalSize = "";
		private string _downloadSize = "";
		private float _downloadSpeed = 0;

		public override IEnumerator Start()
		{
			totalDownloadSize = GetResSizeOperation.DownloadSize;
			//res 更新检查跟操作
			if (_versionService.CompareVersion(VersionService.RES_KEY))
			{
				if (totalDownloadSize > 0)
				{
					yield return DownloadABAssets();
				}
				else
				{
					Debug.LogWarning("res totalDownloadSize is 0");
					Status = EUpdateOperationStatus.Succeed;
				}
			}
			else
			{
				Status = EUpdateOperationStatus.Succeed;
			}
		}

		public override void Release()
		{
			if (downloadHandle.IsValid())
				Addressables.Release(downloadHandle);
		}

		public override string GetUpdateInfo()
		{
			if (totalDownloadSize == 0) return "";
			if (downloadHandle.IsValid())
			{
				DownloadStatus downloadStatus = downloadHandle.GetDownloadStatus();
				_resProgress = ((downloadStatus.DownloadedBytes + hasDownLoadSize) * 100) /
							   totalDownloadSize;
				_downloadSize =
					((downloadStatus.DownloadedBytes + hasDownLoadSize) / 1024f / 1024f).ToString("0.00");

				_downloadSpeed = (downloadStatus.DownloadedBytes) / 1024f / 1024f /
								  (Time.realtimeSinceStartup - _preTime);

				DownloadInfo = string.Format(downloadSpeedInfo, _resProgress.ToString(), _downloadSize,
					_totalSize,
					_downloadSpeed.ToString("0.00"));
			}
			else
			{
				if (_resProgress == 100)
				{
					DownloadInfo = "资源下载完成";
				}
				else if (_resProgress < 100)
				{
					Debug.Log($"hasDownLoadSize {hasDownLoadSize} totalDownloadSize {totalDownloadSize}");
					_resProgress = (hasDownLoadSize * 100) /
								   totalDownloadSize;
					_downloadSize =
						(hasDownLoadSize / 1024f / 1024f).ToString("0.00");
					DownloadInfo = string.Format(downloadSpeedInfo, _resProgress.ToString(), _downloadSize,
						_totalSize,
						_downloadSpeed.ToString("0.00"));
				}
				else
				{
					Debug.LogError("download size more than 100");
					Status = EUpdateOperationStatus.DownloadFailed;
					DownloadInfo = "资源下载失败";
				}
			}

			return DownloadInfo;
		}

		private AsyncOperationHandle downloadHandle;
		private float _preTime = 0;

		private IEnumerator DownloadABAssets()
		{
			_preTime = Time.realtimeSinceStartup;
			var startTime = Time.realtimeSinceStartup;
			var downloadEndTime = Time.realtimeSinceStartup;
			//下载处理
			if (totalDownloadSize > 0)
			{
				_totalSize = (totalDownloadSize / 1024f / 1024f).ToString("0.00");
				var locations = Addressables.LoadResourceLocationsAsync("preload");
				Debug.LogWarning("get preload location");
				yield return locations;

				IList<IResourceLocation> target = new List<IResourceLocation>(locations.Result.Count);
				foreach (var location in locations.Result)
				{
					foreach (var locationDependency in location.Dependencies)
					{
						if (locationDependency.InternalId.StartsWith("http"))
						{
							target.Add(locationDependency);
						}
					}
				}
				downloadHandle = Addressables.DownloadDependenciesAsync(target, false);
				Debug.LogWarning($"locations.Count {locations.Result.Count} target.Count {target.Count}");
				yield return downloadHandle;
				Debug.LogWarning($"time is {Time.realtimeSinceStartup - startTime}");
				if (downloadHandle.Status == AsyncOperationStatus.Failed)
				{
					Debug.LogWarning("AA资源下载失败!");
					Status = EUpdateOperationStatus.DownloadFailed;
					DownloadInfo = "资源下载失败";
					yield break;
				}
				Addressables.Release(downloadHandle);
			}

			Debug.LogWarning("current compress time is " + (Time.realtimeSinceStartup - downloadEndTime));
			Debug.LogWarning("total time is " + (Time.realtimeSinceStartup - startTime));
			Debug.Log("=----下载完毕!");
			DeleteTemp();
			_versionService.SaveVersion(VersionService.RES_KEY);
			//InitializeAsync Addressable
			var init = Addressables.InitializeAsync();
			yield return init;
			Status = EUpdateOperationStatus.Succeed;
			//"下载完毕!"
		}

		private void DeleteTemp()
		{
			if (File.Exists(_tempResPath))
				File.Delete(_tempResPath);
		}
	}
}
