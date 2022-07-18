using AresLuaExtend.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

namespace AresLuaExtend
{
	[CSharpCallLua]
	public delegate void SendCSharpMessage(string message, PointerEventData eventData);
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
		public SendCSharpMessage SendCSharpMessage { get; private set; }
		public static Action OnPreRequestLoaded;
		public static readonly Type[] ExportToLua = {          /*typeof(ddd)*/			};

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
		public int ServiceType => (int)CSharpServiceManager.ServiceType.LUA_SERIVCE;

		public void Destroy()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void Initialize()
		{
			Default = new LuaEnv();
			//ÉèÖÃÄ¬ÈÏloader
			Default.AddLoader((ref string filename) => LoadFile(ref filename, ".lua"));
		}

		public void Update()
		{
			throw new NotImplementedException();
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
	}

}
