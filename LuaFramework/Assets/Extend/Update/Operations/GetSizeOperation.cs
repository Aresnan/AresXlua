using AresLuaExtend.Common;
using AresLuaExtend.Update.Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{
	public class GetSizeOperation : UpdaterOperation
	{
		private const string downloadInfo = "是否选择移动网络下载 {0}MB 资源";
		protected VersionService _versionService;
		public GetSizeOperation(VersionService service)
		{
			_versionService = service;
		}

		public override IEnumerator Start()
		{
			yield return null;
		}

		public static string GetTotalSize()
		{
			string MBSize = (TotalDownloadSize / 1024f / 1024f).ToString("0.00");
			return MBSize;
		}
		public static string GetTotalSizeInfo()
		{
			return string.Format(downloadInfo, GetTotalSize());
		}
	}
}