using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace AresLuaExtend.Common
{
	[CSharpCallLua]
	public delegate void LuaCommandDelegate(string[] param);
	public class CSharpServiceManager : MonoBehaviour
	{
		//service 类型, 显式的指定数据类型为byte
		public enum ServiceType : byte
		{
			ASSET_SERVICE,
			TICK_SERVICE,
			LUA_SERIVCE
		}
		public static bool Initialized { get; private set; }
		public static CSharpServiceManager Instance { get; private set; }
		private static readonly IService[] m_services = new IService[128];
		private static readonly List<IServiceUpdate> m_servicesUpdate = new List<IServiceUpdate>();
		private static readonly List<IServiceLateUpdate> m_servicesLateUpdate = new List<IServiceLateUpdate>();
		//初始化service manager
		public static void Initialize()
		{
			if (Initialized)
			{
				throw new Exception("CSharpServiceManager already initialized");
			}
			Initialized = true;
			Application.quitting += CleanUp;
		}
		//初始化 service 对象
		public static void InitializeServiceGameObject()
		{
			GameObject go = new GameObject("CSharpServiceManager", typeof(UnityMainThreadDispatcher), typeof(CSharpServiceManager));
			DontDestroyOnLoad(go);
			Instance = go.GetComponent<CSharpServiceManager>();
		}
		//注册服务
		public static void Register(IService service)
		{
			if(m_services[service.ServiceType] !=null)
			{
				throw (new Exception($"Service {service.ServiceType} exist."));
			}
			try
			{
				m_services[service.ServiceType] = service;
				service.Initialize();
				//使用声明模式来检查表达式的运行时类型，符合则转换成对应变量
				if (service is IServiceUpdate update)
				{
					m_servicesUpdate.Add(update);
				}
				if(service is IServiceLateUpdate lateUpdate)
				{
					m_servicesLateUpdate.Add(lateUpdate);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		public static void Unregister(int type)
		{
			if(m_services[type] !=null)
			{
				IService service = m_services[type];
				if(service is IServiceUpdate update)
				{
					m_servicesUpdate.Remove(update);
				}
				if(service is IServiceLateUpdate lateUpdate)
				{
					m_servicesLateUpdate.Remove(lateUpdate);
				}
				service.Destroy();
				m_services[type] = null;
			}
		}
		//获取服务
		//指定T必须是Class类型
		public static T Get<T>(ServiceType type) where T : class
		{
			return Get<T>((int)type);
		}
		public static T Get<T>(int index) where T : class
		{
			IService service = m_services[index];
			if(service != null)
			{
				Debug.LogError($"Service {index} not exist!");
			}
				return (T)service;
		}

		private void Update()
		{
			foreach (var item in m_servicesUpdate)
			{
				item.Update();
			}
		}
		private void LateUpdate()
		{
			foreach (var item in m_servicesLateUpdate)
			{
				item.LateUpdate();
			}
		}
		private static void CleanUp()
		{
			Application.quitting -= CleanUp;
			m_servicesUpdate.Clear();
			for (int i = 0; i < m_services.Length; i++)
			{
				IService service = m_services[i];
				if (service != null)
				{
					Unregister(service.ServiceType);
				}
			}
			Initialized = false;
			Debug.LogWarning("Game Exit!!!");
		}
	}

}