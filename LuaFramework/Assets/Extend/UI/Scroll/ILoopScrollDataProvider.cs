using AresLuaExtend.Asset;
using UnityEngine;

namespace AresLuaExtend.UI.Scroll
{
	public interface ILoopScrollDataProvider
	{
		void ProvideData(Transform t, int index);

		AssetReference ProvideAssetReference(int index);
	}
}