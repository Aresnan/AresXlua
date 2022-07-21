using AresLuaExtend;
using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EntryPoint : MonoBehaviour
{   
    void Start()
    {
        LuaVM vm = CSharpServiceManager.Get<LuaVM>(CSharpServiceManager.ServiceType.LUA_SERVICE);
		vm.StartUp();
	}
        
    void Update()
    {
        
    }
}
