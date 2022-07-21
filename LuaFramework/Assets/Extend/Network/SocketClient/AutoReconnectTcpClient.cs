using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace AresLuaExtend.Network
{
	//TODO
	[LuaCallCSharp, CSharpCallLua]
	public class AutoReconnectTcpClient
	{
		public enum Status
		{
			NONE,
			CONNECTED,
			DISCONNECTED,
			RECONNECT,
		}
	}
}
