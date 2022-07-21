using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

public class TestEntryPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LuaEnv Default = new LuaEnv();
        Default.AddLoader((ref string filename) => LoadFile(ref filename, ".lua"));
		Default.DoString("require 'Test.Test'");

		object[] result = Default.DoString($"return require 'Test.Test'");
		LuaTable table = result[0]	as LuaTable;
		LuaFunction Init = table.Get<LuaFunction>("Init");
		Init.Action<Func<string, string>>(s => OnInit(s));

		//LuaFunction test = table.Get<LuaFunction>("Test");
		//test.Action<int>(50);
		//string  s = test.Func<string, string>("null");
		//Debug.Log(s);
	}

	private string OnInit(string s)
	{
		Debug.Log(s);
		return s;
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
