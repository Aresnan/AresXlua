using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

namespace AresLuaExtend
{
	public class LuaVM
	{
		public LuaEnv Default { get; set; }
		public void Initialize()
		{
			Default = new LuaEnv();
			//ÉèÖÃÄ¬ÈÏloader
			Default.AddLoader((ref string filename) => LoadFile(ref filename, ".lua"));
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
