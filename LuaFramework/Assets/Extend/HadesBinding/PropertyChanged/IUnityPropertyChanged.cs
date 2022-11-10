using AresLuaExtend.LuaUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.HadesBinding.PropertyChange
{
	public interface IUnityPropertyChanged
	{
		event PropertyChangedAction OnPropertyChanged;

		object ProvideCurrentValue();
	}
}
