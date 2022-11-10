using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{
    public class GetLuaSizeOperation : GetSizeOperation
    {
		public override IEnumerator Start()
		{
			var downloadUrl = _versionService.luaUrl;
			if (_versionService.CompareVersion(VersionService.LUA_KEY))
			{
				var request = CreateWebRequest(downloadUrl);
				using (var response = request.GetResponse())
				{
					Debug.LogWarning($"lua download size is {response.ContentLength}");
					TotalDownloadSize += response.ContentLength;
				}
				Status = EUpdateOperationStatus.Succeed;
			}
			else
			{
				Status = EUpdateOperationStatus.Succeed;
			}

			return base.Start();
		}
		public override string GetUpdateInfo()
		{
			return "获取资源信息中...";
		}
		public GetLuaSizeOperation(VersionService service) : base(service)
		{
		}

		private WebRequest CreateWebRequest(string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.ProtocolVersion = HttpVersion.Version10;
			return request;
		}
	}
}