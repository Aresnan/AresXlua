using AresLuaExtend.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLuaExtend.EventAsset
{
	public class EventParam
	{
		public enum ParamType : byte
		{
			None,
			Int,
			Float,
			String,
			AssetRef
		}

		public int Int;
		public float Float;
		public string Str;
		public AssetReference AssetRef;
		public ParamType Type;
	}
}
