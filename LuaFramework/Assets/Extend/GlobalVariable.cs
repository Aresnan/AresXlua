using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace AresLua
{
	[LuaCallCSharp]
	public class GlobalVariable
	{
#if Ares_NATIVE
	    public static readonly bool NATIVE = true;
#else
		public static readonly bool NATIVE = false;
#endif

#if Ares_PROD
	    public static readonly bool PROD = true;
#else
		public static readonly bool PROD = false;
#endif

#if UNITY_ANDROID
	    public static readonly string PLATFORM = "android";
#elif UNITY_IOS
		public static readonly string PLATFORM = "ios";
#endif
	}
}
