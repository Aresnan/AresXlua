using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Common
{
	public interface IService 
	{
		int ServiceType { get; }
		void Initialize();
		void Destroy();
	}
	public interface IServiceUpdate
	{
		void Update();
	}

	public interface IServiceLateUpdate
	{
		void LateUpdate();
	}

}