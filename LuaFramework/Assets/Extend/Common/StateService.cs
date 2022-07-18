using AresLuaExtend.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateService : IService
{
	public int ServiceType => (int)CSharpServiceManager.ServiceType.STATE;

	public void Destroy()
	{
		throw new System.NotImplementedException();
	}

	public void Initialize()
	{
		throw new System.NotImplementedException();
	}
}
