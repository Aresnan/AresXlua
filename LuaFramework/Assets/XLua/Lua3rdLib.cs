using System;
using System.Runtime.InteropServices;
using AresLuaExtend;
using AresLuaExtend.Common;
using UnityEngine;

namespace XLua.LuaDLL {
	public partial class Lua {
		/* 添加扩展步骤：
		    修改build文件、工程设置，把要集成的扩展编译到XLua Plugin里头；
			调用xLua的C# API，使得扩展可以被按需（在lua代码里头require的时候）加载；
			可选，如果你的扩展里头需要用到64位整数，你可以通过XLua的64位扩展库来实现和C#的配合*/

		/* 所有lua的C扩展库都会提供个luaopen_xxx的函数，xxx是动态库的名字，
			比如lua-rapidjson库该函数是luaopen_rapidjson，这类函数由lua虚拟机在加载动态库时自动调用，
			而在手机平台，由于ios的限制我们加载不了动态库，而是直接编译进进程里头。
			为此，XLua提供了一个API来替代这功能（LuaEnv的成员方法）
			public void AddBuildin(string name, LuaCSFunction initer)
			name：buildin模块的名字，require时输入的参数；
			initer：初始化函数，原型是这样的public delegate int lua_CSFunction(IntPtr L)，必须是静态函数，
		    而且带MonoPInvokeCallbackAttribute属性修饰，这个api会检查这两个条件。*/
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern int luaopen_sproto_core(IntPtr L); //[,,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern int luaopen_lpeg(IntPtr L); //[,,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern int luaopen_luv(IntPtr L); //[,,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern int luaopen_rapidjson(IntPtr L); //[,,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern void luaopen_chronos(IntPtr L);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern int luaopen_lsqlite3(IntPtr L);

#if EMMY_CORE_SUPPORT
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		private static extern int luaopen_emmy_core(IntPtr L);

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		public static int LoadEmmyCore(IntPtr L) {
			return luaopen_emmy_core(L);
		}
#endif
		
		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		internal static int LoadSprotoCore(IntPtr L) {
			return luaopen_sproto_core(L);
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		internal static int LoadLpeg(IntPtr L) {
			return luaopen_lpeg(L);
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		internal static int LoadChronos(IntPtr L) {
			luaopen_chronos(L);
			return 1;
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		internal static int LoadRapidJson(IntPtr L) {
			return luaopen_rapidjson(L);
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		internal static int LoadLUV(IntPtr L) {
			return luaopen_luv(L);
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		internal static int LoadLSqlite3(IntPtr L) {
			return luaopen_lsqlite3(L);
		}

		public static void OverrideLogFunction(IntPtr rawL) {
#if !XLUA_GENERAL
			lua_pushstdcallcfunction(rawL, PrintI);
			if( 0 != xlua_setglobal(rawL, "print") ) {
				throw new Exception("call xlua_setglobal fail!");
			}

			lua_pushstdcallcfunction(rawL, PrintW);
			if( 0 != xlua_setglobal(rawL, "warn") ) {
				throw new Exception("call xlua_setglobal fail!");
			}

			lua_pushstdcallcfunction(rawL, PrintE);
			if( 0 != xlua_setglobal(rawL, "error") ) {
				throw new Exception("call xlua_setglobal fail!");
			}
#endif
		}

#if !XLUA_GENERAL
		private static int CollectLog(IntPtr L, out string s) {
			s = string.Empty;
			try {
				int n = lua_gettop(L);

				if( 0 != xlua_getglobal(L, "tostring") ) {
					return luaL_error(L, "can not get tostring in print:");
				}

				for( int i = 1; i <= n; i++ ) {
					lua_pushvalue(L, -1); /* function to be called */
					lua_pushvalue(L, i); /* value to print */
					if( 0 != lua_pcall(L, 1, 1, 0) ) {
						return lua_error(L);
					}

					s += lua_tostring(L, -1);

					if( i != n ) s += "    ";

					lua_pop(L, 1); /* pop result */
				}

				return 0;
			}
			catch( Exception e ) {
				return luaL_error(L, "c# exception in print:" + e);
			}
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		private static int PrintI(IntPtr L) {
			var ret = CollectLog(L, out var s);
			Debug.Log(s);
			return ret;
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		private static int PrintW(IntPtr L) {
			var ret = CollectLog(L, out var s);
			Debug.LogWarning($"{s}\n{LuaVM.LogCallStack()}");
			return ret;
		}

		[MonoPInvokeCallback(typeof(lua_CSFunction))]
		private static int PrintE(IntPtr L) {
			var ret = CollectLog(L, out var s);
			Debug.LogError($"{s}\n{LuaVM.LogCallStack()}");
			return ret;
		}
#endif
	}
}