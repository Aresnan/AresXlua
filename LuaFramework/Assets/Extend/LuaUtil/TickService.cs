using AresLuaExtend;
using AresLuaExtend.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
namespace AresLuaExtend.LuaUtil
{
	[CSharpCallLua]
	public class TickService : IService, IServiceUpdate, IServiceLateUpdate
	{
		public int ServiceType => (int)CSharpServiceManager.ServiceType.TICK_SERVICE;
		[CSharpCallLua]
		private Action m_tick;
		[CSharpCallLua]
		private Action m_lateTick;

		public void Destroy()
		{

		}

		public void Initialize()
		{
			LuaVM luaVM = CSharpServiceManager.Get<LuaVM>(CSharpServiceManager.ServiceType.LUA_SERVICE);
			//获取全局表：_ServiceManager 的方法GetService，与delegate GetLuaService 类型相同，并传参调用
			//GetLuaService luaGetService = luaVM.Global.GetInPath<GetLuaService>("_ServiceManager.GetService");
			//LuaTable luaTickService = luaGetService(2);
			//m_tick = luaTickService.Get<Action>("Tick");
			//m_lateTick = luaTickService.Get<Action>("LateTick");
		}

		public void LateUpdate()
		{
			m_lateTick?.Invoke();
		}

		public void Update()
		{
			m_tick?.Invoke();
		}
	}
}
