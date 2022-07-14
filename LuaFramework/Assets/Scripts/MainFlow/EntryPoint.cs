using AresLuaExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{   
    void Start()
    {
        LuaVM vm = new LuaVM();
        vm.Initialize();
        vm.Default.DoString("require 'base.main'");
    }
        
    void Update()
    {
        
    }
}
