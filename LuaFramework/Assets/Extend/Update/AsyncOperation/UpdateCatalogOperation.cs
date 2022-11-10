using AresLuaExtend.Update.Operations;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AresLuaExtend.Update.AsyncOperation
{
    public class UpdateCatalogOperation : AsyncOperationBase
    {
		private enum ESteps
		{
			None,
			CheckCatalog,
			UpdateCatalog,
			GetSize,
			GetSizeDone,
			ClearCache,
			Done,
		}

		public long TotalDownloadSize;
		public string MBSize;
		private string _catalogCachePath;
		private ESteps _eSteps;
		private List<string> catalogs;
		private string _tempResPath;
		private AsyncOperationHandle<List<IResourceLocator>> _updateHandle;
		private int _index;

		public UpdateCatalogOperation(string catalogCachePath)
		{
			_tempResPath = $"{Application.persistentDataPath}/ResTemp.json";
			_catalogCachePath = catalogCachePath;
			_eSteps = ESteps.None;
			TotalDownloadSize = 0;
			_index = 0;
		}

		private AsyncOperationHandle<List<string>> _checkHandle;

		private AsyncOperationHandle<long> _getSizeHandle;

		internal override void Start()
		{
			Debug.Log("CheckForCatalogUpdates");
			_checkHandle = Addressables.CheckForCatalogUpdates(false);
			_eSteps = ESteps.CheckCatalog;
		}

		private bool _isClearCache = false;

		internal override void Update()
		{
			if (_eSteps == ESteps.None || _eSteps == ESteps.Done) return;
			if (_eSteps == ESteps.CheckCatalog)
			{
				if (_isClearCache)
				{
					catalogs = GetKeys();
					if (catalogs != null && catalogs.Count > 0)
					{
						_updateHandle = Addressables.UpdateCatalogs(catalogs, false);
						_eSteps = ESteps.UpdateCatalog;
					}
					else
					{
						Status = EOperationStatus.Succeed;
					}
				}
				else
				{
					catalogs = _checkHandle.Result;
					if (_checkHandle.IsDone)
					{
						if (_checkHandle.Status == AsyncOperationStatus.Succeeded)
						{
							catalogs = _checkHandle.Result;
							if (catalogs != null && catalogs.Count > 0)
							{
								SetKeys(catalogs);
								_updateHandle = Addressables.UpdateCatalogs(catalogs, false);
								_eSteps = ESteps.UpdateCatalog;
							}
							else
							{
								Debug.LogWarning("have catalog cache");
								//CataLogCache 清理文件
								if (!_isClearCache)
								{
									_isClearCache = true;
									if (HasCatalogTemp())
									{
										Debug.LogWarning("存在中途退出问题! 重新获取size");
										Addressables.Release(_checkHandle);
										_eSteps = ESteps.ClearCache;
									}
									else
									{
										Debug.LogWarning("do not has temp cata log, enter game");
										Status = EOperationStatus.Succeed;
									}

									return;
								}
								else
								{
									Debug.LogWarning("no res need update, enter game");
									Status = EOperationStatus.Succeed;
								}
							}
						}
						else
						{
							Debug.LogWarning("Addressables.CheckForCatalogUpdates failed ");
							Status = EOperationStatus.Failed;
						}

						Addressables.Release(_checkHandle);
					}
				}
			}
			else if (_eSteps == ESteps.UpdateCatalog)
			{
				if (_updateHandle.IsDone)
				{
					if (_updateHandle.Status == AsyncOperationStatus.Succeeded)
					{
						_eSteps = ESteps.GetSize;
					}
					else
					{
						Debug.LogWarning("_updateHandle failed ");
						Status = EOperationStatus.Failed;
					}
				}
			}
			else if (_eSteps == ESteps.GetSize)
			{
				if (_index < _updateHandle.Result.Count)
				{
					_getSizeHandle = Addressables.GetDownloadSizeAsync(_updateHandle.Result[_index].Keys);
					_eSteps = ESteps.GetSizeDone;
				}
				else
				{
					Status = EOperationStatus.Succeed;
					MBSize = (TotalDownloadSize / 1024f / 1024f).ToString("0.00");
					Debug.Log($"更新== Size: {MBSize}M");
					//获取大小完成
				}
			}
			else if (_eSteps == ESteps.GetSizeDone)
			{
				if (_getSizeHandle.IsDone)
				{
					if (_getSizeHandle.Status == AsyncOperationStatus.Succeeded)
					{
						if (_getSizeHandle.Result > 0)
						{
							TotalDownloadSize += _getSizeHandle.Result;

							// var result = _updateHandle.Result[_index];
							// foreach (var key in result.Keys)
							// {
							// 	if (!result.Locate(key, typeof(Object), out _))
							// 	{
							// 		mUpdateResKeys.Add(key);
							// 	}
							// }
							Addressables.Release(_getSizeHandle);
						}

						_index += 1;
						_eSteps = ESteps.GetSize;
					}
					else
					{
						Debug.LogWarning("Addressables.GetDownloadSizeAsync failed");
						Status = EOperationStatus.Failed;
					}
				}
			}
			else if (_eSteps == ESteps.ClearCache)
			{
				if (!_checkHandle.IsValid())
				{
					if (_isClearCache)
					{
						Start();
					}
				}
			}
		}

		internal override void Restart()
		{
		}

		private void SetKeys(List<string> catalog)
		{
			JArray array = JArray.FromObject(catalog);
			File.WriteAllText(_tempResPath, array.ToString());
		}

		private bool HasCatalogTemp()
		{
			return File.Exists(_tempResPath);
		}

		private List<string> GetKeys()
		{
			string json = File.ReadAllText(_tempResPath);
			JArray array = JArray.Parse(json);
			List<string> tmp = new List<string>();
			tmp.Capacity = array.Count + 200;
			foreach (var jToken in array)
			{
				tmp.Add(jToken.ToString());
			}

			return tmp;
		}

		public bool RestCatalog()
		{
			if (!string.IsNullOrEmpty(_catalogCachePath))
			{
				string path = Path.Combine(Application.persistentDataPath, _catalogCachePath);
				string jsonPath = path + ".json";
				string hashPath = path + ".hash";
				if (File.Exists(jsonPath))
				{
					File.Delete(jsonPath);
					if (File.Exists(hashPath))
					{
						File.Delete(hashPath);
						return true;
					}
				}
			}

			return false;
		}
	}
}