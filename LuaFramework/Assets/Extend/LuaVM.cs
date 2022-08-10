using AresLuaExtend.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;
using XLua.LuaDLL;

namespace AresLuaExtend
{
	public class LuaVM : IService, IServiceUpdate, IDisposable
	{
		//TODO
		private LuaMemoryLeakChecker.Data leakData;
		//TODO
#if LOAD_FROM_PACK
		private static readonly string LUA_DEBUG_DIRECTORY = Application.persistentDataPath + "/Lua/";
		private static readonly Dictionary<string, byte[]> m_unzipLuaFiles = new Dictionary<string, byte[]>(2048);
#endif
		private LuaFunction OnDestroy;
		private LuaFunction OnInit;
		private LuaEnv Default { get; set; }
		public LuaTable Global => Default.Global;
		public LuaTable DestroyedTableMeta { private set; get; }
		public static Action OnPreRequestLoaded;
		public static readonly Type[] ExportToLua = {          /*typeof(ddd)*/			};

		//Lua
		//public LuaClassCache LuaClassCache { get; private set; }
		public static Action OnVMCreated;
		public static Action OnVMQuiting;
		private LuaTable m_bindingEnv;

		//TODO
#if LUA_WRAP_CHECK && UNITY_EDITOR
		static LuaVM() {
			ObjectTranslator.callbackWhenLoadType = type => {
				if( type.Namespace != null && type.Namespace.StartsWith("UnityEngine") ) {
					return;
				}

				if( type.GetCustomAttribute(typeof(LuaCallCSharpAttribute)) != null ||
				    type.IsSubclassOf(typeof(Delegate)) ||
				    type.GetCustomAttribute(typeof(BlackListAttribute)) != null ) {
					return;
				}

				if( type.FullName == "System.RuntimeType" ) {
					return;
				}

				if( Array.IndexOf(ExportToLua, type) != -1 ) {
					return;
				}

				Debug.LogWarning($"Type : {type} not wrap!\n" + LogCallStack());
			};
		}
#endif

		public LuaTable NewTable()
		{
			return Default.NewTable();
		}

		public long Memory => Default.Memroy;
		public int LuaMapCount => Default.translator.objects.Count;
		public int ServiceType => (int)CSharpServiceManager.ServiceType.LUA_SERVICE;

		public void Initialize()
		{
			Default = new LuaEnv();
			//��ӵ�������չ
			Default.AddBuildin("rapidjson", Lua.LoadRapidJson);
			Default.AddBuildin("chronos", Lua.LoadChronos);
#if EMMY_CORE_SUPPORT
			Default.AddBuildin("emmy_core", Lua.LoadEmmyCore);
#endif
			Default.AddBuildin("lpeg", Lua.LoadLpeg);
			Default.AddBuildin("sproto.core", Lua.LoadSprotoCore);
			Default.AddBuildin("luv", Lua.LoadLUV);
			Default.AddBuildin("lsqlite", Lua.LoadLSqlite3);

			Lua.OverrideLogFunction(Default.rawL);
			//�����Զ���Loader
			Default.AddLoader((ref string filename) => LoadFile(ref filename, ".lua"));
			//���ֵ��÷�ʽ��1.ӳ�䵽LuaTable��2.ӳ�䵽class����interface
			LuaTable result = LoadFileAtPath("PreRequest")[0] as LuaTable;
			OnInit = result.Get<LuaFunction>("Init");
		}

		public void StartUp()
		{
			//Func,���һ��TΪ����ֵ
			//Action������Call��������OnInit������������Ĳ����൱�ڸ�lua����������һ���ص�callback
			//OnInit.Action<Func<string, byte[]>>(filename => LoadFile(ref filename, ".lua"));
			OnInit.Action<LuaVM, int>(this, 1);
			//ͨ��Global�����_BindingEnv��lua�ļ���
			m_bindingEnv = Default.Global.GetInPath<LuaTable>("_BindingEnv");
		}
		public void Update()
		{
			Default.Tick();
		}

		private byte[] LoadFile(ref string filename, string extension)
		{
			filename = filename.Replace('.', '/') + extension;
			var path = $"{Application.dataPath}/../AresLua/{filename}";
			if (File.Exists(path))
			{
				var text = File.ReadAllText(path);
				return Encoding.UTF8.GetBytes(text);
			}
			return null;
		}

		public object[] LoadFileAtPath(string luaFileName)
		{
			//�ٷ�ʾ��DoString("require 'byfile'")����return��Ϊ���õ����صĶ���
			object[] result = Default.DoString($"return require '{luaFileName}'");
			return result;
		}
		//TODO
		public static string LogCallStack()
		{
			var luaVm = CSharpServiceManager.Get<LuaVM>(CSharpServiceManager.ServiceType.LUA_SERVICE);
			var msg = luaVm.Default.Global.GetInPath<Func<string>>("debug.traceback");
			var str = "lua stack : " + msg.Invoke();
			Debug.Log(str);

			return str;
		}
		public void Destroy()
		{

		}

		public void Dispose()
		{

		}
	}

}
