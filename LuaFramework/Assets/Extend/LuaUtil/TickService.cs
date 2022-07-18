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
			throw new System.NotImplementedException();
		}

		public void Initialize()
		{
			LuaVM luaVM = CSharpServiceManager.Get<LuaVM>(CSharpServiceManager.ServiceType.LUA_SERIVCE);
			//TODO
			GetLuaService luaGetService = luaVM.Global.GetInPath<GetLuaService>("");
			LuaTable luaTickService = luaGetService(2);
			m_tick = luaTickService.Get<Action>("Tick");
			m_lateTick = luaTickService.Get<Action>("LateTick");
		}

		public void LateUpdate()
		{
			m_lateTick();
		}

		void IServiceUpdate.Update()
		{
			m_tick();
		}
	}
}
