using AresLuaExtend.Common;
using AresLuaExtend.Network;
using AresLuaExtend.Update.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AresLuaExtend.Update.AsyncOperation
{
    public class UpdateLuaOperation : AsyncOperationBase
    {
		private enum ESteps
		{
			None,
			DownloadLua,
			Downloading,
			VerifyLuaFile,
			Done,
		}

		private VersionService _versionService;
		private string downloadUrl;
		private string _luaFileMd5;
		private ESteps _eSteps;
		private string tempPath;

		public AresDownload download;
		public UpdateLuaOperation(string url, string md5, VersionService service)
		{
			downloadUrl = url;
			_versionService = service;
			_luaFileMd5 = md5;
			_eSteps = ESteps.None;
		}

		internal override void Start()
		{
			tempPath = Application.persistentDataPath + "/luatempfile";
			CheckLuaUpdate();
		}

		internal override void Update()
		{
			if (_eSteps == ESteps.Done || _eSteps == ESteps.None) return;
			if (_eSteps == ESteps.DownloadLua)
			{
				_eSteps = ESteps.Downloading;
				download = AresDownload.DownloadAsync(downloadUrl,
					tempPath,
					(d) =>
					{
						if (d.status == DownloadStatus.Failed)
						{
							Debug.LogWarning("Download lua file failed");
							Status = EOperationStatus.Failed;
							return;
						}

						_eSteps = ESteps.VerifyLuaFile;
					});
			}
			else if (_eSteps == ESteps.VerifyLuaFile)
			{
				if (!CheckMD5(tempPath))
				{
					Debug.LogError("MD5 check failed");
					DeleteTempCache();
					// md5 check failed 
					Error = "lua md5 check failed";
					Status = EOperationStatus.Failed;
				}
				else
				{
					try
					{
						_versionService.DeleteLuaCache();
						File.Copy(tempPath, _versionService._luaSavePath);
						DeleteTempCache();
						_versionService.SaveVersion(VersionService.LUA_KEY);
						Status = EOperationStatus.Succeed;
					}
					catch (Exception e)
					{
						Status = EOperationStatus.Failed;
						Debug.LogError($"lua file operation is error {e}");
					}

				}
			}
		}

		private void DeleteTempCache()
		{
			if (File.Exists(tempPath))
				File.Delete(tempPath);
		}

		internal override void Restart()
		{

		}

		internal protected void CheckLuaUpdate()
		{
			if (_versionService.CompareVersion(VersionService.LUA_KEY))
			{
				if (!string.IsNullOrEmpty(downloadUrl))
				{
					_eSteps = ESteps.DownloadLua;
				}
				else
				{
					Debug.LogWarning($"_luaDownloadUrl is null or empty");
					Status = EOperationStatus.Failed;
					Error = "_luaDownloadUrl is null or empty";
				}
			}
			else
			{
				CheckCache();
				//No need to update
				Status = EOperationStatus.Succeed;
			}
		}

		private void CheckCache()
		{
			if (!CheckMD5(_versionService._luaSavePath))
			{
				if (File.Exists(_versionService._luaSavePath))
				{
					File.Delete(_versionService._luaSavePath);
				}
			}
		}

		private bool CheckMD5(string path)
		{
			string md5 = GetMD5HashFromFile(path);
			Debug.LogWarning($" md5 {md5}  luaFileMd5 {_luaFileMd5}");
			return md5 == _luaFileMd5;
		}

		public string GetMD5HashFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Debug.LogError("GetMD5HashFromFile failed,file is not exits");
				return "";
			}

			try
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
				{
					System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
					byte[] retVal = md5.ComputeHash(fileStream);

					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < retVal.Length; i++)
					{
						sb.Append(retVal[i].ToString("x2"));
					}

					return sb.ToString();
				}
			}
			catch (Exception e)
			{
				throw new Exception("GetMD5HashFromFile failed,error is  " + e.Message);
			}
		}
	}
}