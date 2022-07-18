using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Extend.Network
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
