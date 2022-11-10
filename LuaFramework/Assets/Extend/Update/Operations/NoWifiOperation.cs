using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{
	public class NoWifiOperation : UpdaterOperation
	{
		private VersionService _versionService;

		public NoWifiOperation(VersionService service)
		{
			_versionService = service;
			Status = EUpdateOperationStatus.NeedWifi;
		}

		public override IEnumerator CheckWifi()
		{
			if (IsWifi())
			{
				Status = EUpdateOperationStatus.Succeed;
			}
			else
			{
				Status = TotalDownloadSize > 0 ? EUpdateOperationStatus.NoWifi : EUpdateOperationStatus.Succeed;
			}

			yield return null;
		}

		public override IEnumerator Start()
		{
			Status = EUpdateOperationStatus.Succeed;
			yield return 0;
		}

		public override void Release()
		{
		}

		public override void Reset()
		{
			Status = EUpdateOperationStatus.NeedWifi;
		}

		private bool IsWifi()
		{
			//通过unity或c# API来判断是否连接WiFi，或者直接通过native拿到状态
			return false;
		}
	}
}
