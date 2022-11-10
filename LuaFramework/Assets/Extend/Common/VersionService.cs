using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AresLuaExtend.Common
{
	public class VersionService : IService
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.VERSION;
		//This function returns the current version of the Application. This is read-only. To set the version number in Unity, go to Edit > Project Settings > Player.
		public static readonly string CSVERSION = Application.version;
		public static string PLATFORMVERSION;
		public static string LUAVERSION;
		public static string RESVERSION;

		public static readonly string LUA_KEY = "lua";
		public static readonly string PLATFROM_KEY = "platform";
		public static readonly string RES_KEY = "res";

		public static string ResUrl;
		public static bool HotFixing = true;
		public string AccountId;
		public string luaUrl;
		public string luaFileMd5;

		private static Dictionary<string, Version> _localVersionDic;
		private readonly Dictionary<string, Version> _serverVersionDic;
		private string _deviceInfo;
		public string _luaSavePath;

		private string _versionPath;
		private string VersionPath
		{
			get
			{
				if (string.IsNullOrEmpty(_versionPath))
				{
					_versionPath = $"{Application.persistentDataPath}/versionFile.json";
				}

				return _versionPath;
			}
		}


		public VersionService()
		{
			PLATFORMVERSION = GameSystemSetting.Get().SystemSetting.GetString("GAME", "Version");
			LUAVERSION = GameSystemSetting.Get().SystemSetting.GetString("GAME", "LuaVersion");
			RESVERSION = GameSystemSetting.Get().SystemSetting.GetString("GAME", "ResVersion");
			_luaSavePath = $"{Application.persistentDataPath}/LuaHotUpdate";
			_localVersionDic = new Dictionary<string, Version>();
			_serverVersionDic = new Dictionary<string, Version>();
		}

		public void Destroy()
		{

		}

		#region 初始化version
		public void Initialize()
		{
			SetDeviceInfo();
			InitVersionVariable();
		}

		private void SetDeviceInfo()
		{
			var info = GameSystemSetting.Get().SystemSetting.GetString("DEBUG", "DeviceInfo");
			Debug.Log("info is " + info);
			if (string.IsNullOrEmpty(info) || info == "Default")
			{
#if UNITY_ANDROID
				_deviceInfo = "Android";
#elif UNITY_IOS
				_deviceInfo = "iOS";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
				_deviceInfo = "Windows";
#else
				_deviceInfo = "UnSurport";
#endif
			}
			else
			{
				_deviceInfo = info;
			}
		}

		private void InitVersionVariable()
		{
			AddOrUpdate(_localVersionDic, PLATFROM_KEY, Version.Parse(PLATFORMVERSION));
			AddOrUpdate(_localVersionDic, LUA_KEY, Version.Parse(LUAVERSION));
			AddOrUpdate(_localVersionDic, RES_KEY, Version.Parse(RESVERSION));
			if (File.Exists(VersionPath))
			{
				string json = File.ReadAllText(VersionPath);
				JObject JObj = JObject.Parse(json);

				var tmpVer = Version.Parse(JObj[PLATFROM_KEY].ToString());
				var tmpLua = Version.Parse(JObj[LUA_KEY].ToString());
				var tmpRes = Version.Parse(JObj[RES_KEY].ToString());

				//小于零 当前 Version 对象是 version 之前的一个版本。
				//等于零 当前 Version 对象是与 version 相同的版本。
				//大于零 当前 Version 对象是 version 之后的一个版本，或者 version 为 null。

				//本地versionFile.json文件和包里的systemconfig.json版本比较
				//1.先比较大版本
				if (tmpVer.CompareTo(_localVersionDic[PLATFROM_KEY]) >= 0)//
				{
					AddOrUpdate(_localVersionDic, PLATFROM_KEY, tmpVer);
					AddOrUpdate(_localVersionDic, RES_KEY, tmpRes);
					//2.再比较Lua版本
					if (tmpLua.CompareTo(_localVersionDic[LUA_KEY]) >= 0)
					{
						AddOrUpdate(_localVersionDic, LUA_KEY, tmpLua);
					}
					else
					{
						DeleteLuaCache();
					}
				}
				else
				{
					//当前版本小于本地版本
					File.Delete(VersionPath);
					DeleteLuaCache();
				}
			}

			AddOrUpdate(_serverVersionDic, PLATFROM_KEY, _localVersionDic[PLATFROM_KEY]);
			AddOrUpdate(_serverVersionDic, LUA_KEY, _localVersionDic[LUA_KEY]);
			AddOrUpdate(_serverVersionDic, RES_KEY, _localVersionDic[RES_KEY]);
		}

		public void DeleteLuaCache()
		{
			if (File.Exists(_luaSavePath))
				File.Delete(_luaSavePath);
		}

		public void AddOrUpdate(Dictionary<string, Version> dic, string key, Version value)
		{
			if (dic.ContainsKey(key))
			{
				dic[key] = value;
			}
			else
			{
				dic.Add(key, value);
			}
		}

		#endregion

		#region 服务

		public bool CompareVersion(string key)
		{
			if (_localVersionDic.ContainsKey(key))
			{
				return _localVersionDic[key].CompareTo(_serverVersionDic[key]) < 0;
			}
			else
			{
				Debug.LogError("VersionDic not contains key " + key);
				return false;
			}
		}

		public void SaveVersion(string key)
		{
			if (_localVersionDic.ContainsKey(key))
			{
				if (_localVersionDic[key] == _serverVersionDic[key])
				{
					Debug.LogWarning("local equal to server version not need to save file");
					return;
				}

				_localVersionDic[key] = _serverVersionDic[key];
			}
			else
			{
				Debug.LogError("VersionDic not contains key " + key);
			}
			string json = JObject.FromObject(_localVersionDic).ToString(Formatting.None);
			File.WriteAllText(VersionPath, json);
		}

		public string GetDeviceInfo()
		{
			return _deviceInfo;
		}

		public static string GetVersion(string key)
		{
			if (_localVersionDic.ContainsKey(key))
			{
				return _localVersionDic[key].ToString();
			}
			else
			{
				Debug.LogError("VersionDic not contains key " + key);
				return "";
			}
		}

		//解析版本号服务器数据
		public void ParseServerInfo(string info)
		{
			var serverInfo = JObject.Parse(info);

			Debug.Log(info);
			var PlatformVersion = Version.Parse(serverInfo["UnityVersion"]?.ToString() ?? string.Empty);
			var LuaVersion = Version.Parse(serverInfo["LuaVersion"]?.ToString() ?? string.Empty);
			var ResVersion = Version.Parse(serverInfo["ResourceVersion"]?.ToString() ?? string.Empty);
			luaUrl = serverInfo["LuaUrl"]?.ToString();
			luaFileMd5 = serverInfo["LuaFileMd5"]?.ToString();
			AddOrUpdate(_serverVersionDic, PLATFROM_KEY, PlatformVersion);
			AddOrUpdate(_serverVersionDic, LUA_KEY, LuaVersion);
			AddOrUpdate(_serverVersionDic, RES_KEY, ResVersion);

			ResUrl = serverInfo["ResourceUrl"]?.ToString();
			Debug.LogWarning(
				$"localPlatformVersion {_localVersionDic[PLATFROM_KEY]} _localLuaVersion {_localVersionDic[LUA_KEY]} _localResVersion {_localVersionDic[RES_KEY]}" +
				$" serverPlatformVersion {_serverVersionDic[PLATFROM_KEY]} serverLuaVersion {_serverVersionDic[LUA_KEY]} serverResVersion {_serverVersionDic[RES_KEY]}");
		}
		#endregion
		//进入lua的入口
		public void StartLua()
		{

		}
	}
}
