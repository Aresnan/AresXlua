using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using XLua;
using Object = UnityEngine.Object;
using AddressableReference = UnityEngine.AddressableAssets.AssetReference;

namespace AresLuaExtend.Asset
{
	//TODO
	[CSharpCallLua]
    public delegate void OnInstantiateComplete(GameObject go);

	[Serializable, LuaCallCSharp]
	public class AssetReference : IDisposable
	{
		private AddressableReference m_assetRef;
		public string AssetGUID => m_assetRef.AssetGUID;

		public bool GUIDValid
		{
			get
			{
				if (m_assetRef == null || string.IsNullOrEmpty(m_assetRef.AssetGUID))
				{
					return false;
				}
#if UNITY_EDITOR
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(m_assetRef.AssetGUID);
				return !string.IsNullOrEmpty(path);
#else
				return true;
#endif
			}
		}
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
