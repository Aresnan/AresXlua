using AresLuaExtend.Common;
using AresLuaExtend.Network;
using AresLuaExtend.Update.Operations;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.AsyncOperation
{
	public class RequestServerVersionOperation : AsyncOperationBase
	{
		//这个类就是为了拿到这个Result
		public string Result;
		private enum ESteps
		{
			None,
			SendVersionRequest,
			SendInfoRequest,
			WaitForRequest,
			Done,
		}

		private string _deviceInfo;
		private string _platformVersion;
		private string _uri;
		private string _acid;
		private ESteps _eSteps;

		public RequestServerVersionOperation(VersionService versionService)
		{
			_deviceInfo = versionService.GetDeviceInfo();
			Debug.LogWarning($"_acid {_acid} versionService.AccountId {versionService.AccountId}");
			_acid = versionService.AccountId;
			_platformVersion = VersionService.GetVersion(VersionService.PLATFROM_KEY);
			_eSteps = ESteps.None;
		}

		internal override void Start()
		{
			string url = GameSystemSetting.Get().SystemSetting.GetString("GAME", "VersionUrl");


			_uri = url.BuildGetUrl(new Dictionary<string, string>()
			{
				{
					"platform", _deviceInfo.ToLower()
				},
				{
					"unityversion", _platformVersion
				},
				{
					"userid", _acid
				}
			});

			Debug.LogWarning("uri is " + _uri);
			_eSteps = ESteps.SendVersionRequest;
		}

		public void RequestServerVersion()
		{
			Debug.LogWarning(
				$"get server version _deviceInfo {_deviceInfo}  _localPlatformVersion {_platformVersion}");
			// send to server to get version
			_uri.Get((dic) =>
			{
				if (dic == null)
				{
					Error = "RequestServerFailed request is null";
					Status = EOperationStatus.Failed;
					return;
				}

				if (string.IsNullOrEmpty(dic["result"]))
				{
					Debug.LogWarning("result is empty get verion back false,deviceinfo or version is not match server");
					Error = "NotMatch";
					Status = EOperationStatus.Failed;
				}
				else
				{
					Debug.LogWarning("request version result is " + dic["result"]);
					JObject result = JObject.Parse(dic["result"]);
					var code = result["Code"]?.ToString();
					if (!string.IsNullOrEmpty(code))
					{
						if (code == "900")
						{
							Debug.LogWarning("result is empty get verion back false,deviceinfo or version is not match server");
							Error = "NotMatch";
							Status = EOperationStatus.Failed;
						}
						else if (code == "1000")
						{
							Result = result["Data"]?.ToString() ?? string.Empty;
							if (string.IsNullOrEmpty(Result))
							{
								Debug.LogWarning("LoadServerVersion failed ,versionUrl json is null");
								Error = "LoadServerVersion failed ,versionUrl json is null";
								Status = EOperationStatus.Failed;
								return;
							}
							Status = EOperationStatus.Succeed;
						}
					}
					else
					{
						Error = "RequestServerFailed request is null";
						Status = EOperationStatus.Failed;
						return;
					}

				}
			}, new Dictionary<string, string>()
			{
				{
					"accept", "text/plain"
				}
			});
		}
		
		internal override void Update()
		{
			if (_eSteps == ESteps.Done || _eSteps == ESteps.None) return;
			if (_eSteps == ESteps.SendVersionRequest)
			{
				RequestServerVersion();
				_eSteps = ESteps.WaitForRequest;
			}
		}

		internal override void Restart()
		{

		}
	}
}