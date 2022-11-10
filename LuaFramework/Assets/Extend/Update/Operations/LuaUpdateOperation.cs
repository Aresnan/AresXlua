using AresLuaExtend.Common;
using AresLuaExtend.Network;
using AresLuaExtend.Update.AsyncOperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{
    public class LuaUpdateOperation : UpdaterOperation
    {
		private VersionService _versionService;

		private const string downloadSpeedInfo = "正在下载 {0}% [{1} MB/{2} MB]\n 速度：{3}MB/秒";
		// The class constructor is called when the class instance is created
		public LuaUpdateOperation(VersionService service)
		{
			_versionService = service;
		}

		public override string GetUpdateInfo()
		{
			if (_updateLuaOperation != null && _updateLuaOperation.download != null)
			{

				var resProgress =
					_updateLuaOperation.download.progress * 100;

				string progress = CheckNanOrInfinity(resProgress);
				var downloadSize = (AresDownload.TotalDownloadedBytes == 0) ? "0" :
					((AresDownload.TotalDownloadedBytes) / 1024f / 1024f).ToString("0.00");

				var totalSize = (AresDownload.TotalSize == 0) ? "0" :
					((AresDownload.TotalSize) / 1024f / 1024f).ToString("0.00");
				var downloadSpeed = AresDownload.TotalBandwidth / 1024f / 1024f;

				string speed = CheckNanOrInfinity(downloadSpeed);

				DownloadInfo = string.Format(downloadSpeedInfo, progress, downloadSize,
					totalSize,
					speed);
			}
			return DownloadInfo;
		}

		private string CheckNanOrInfinity(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				return "0";
			}
			return value.ToString("00.0");
		}

		private UpdateLuaOperation _updateLuaOperation;
		public override IEnumerator Start()
		{
			var downloadUrl = _versionService.luaUrl;
			var luaFileMd5 = _versionService.luaFileMd5;
			if (_versionService.CompareVersion(VersionService.LUA_KEY))
			{
				_updateLuaOperation = new UpdateLuaOperation(downloadUrl, luaFileMd5, _versionService);
				VersionManager.StartOperation(_updateLuaOperation);
				yield return _updateLuaOperation;
				if (_updateLuaOperation.Status == EOperationStatus.Succeed)
				{
					Status = EUpdateOperationStatus.Succeed;
				}
				else
				{
					Status = EUpdateOperationStatus.DownloadFailed;
				}
			}
			else
			{
				Status = EUpdateOperationStatus.Succeed;
			}

			_updateLuaOperation = null;
		}
	}
}
