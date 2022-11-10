using AresLuaExtend.EventAsset;
using AresLuaExtend.LuaUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace AresLuaExtend.LuaBindingEvent
{
	[LuaCallCSharp]
	public abstract class LuaBindingEventBase : MonoBehaviour
	{
		private static BindEvent m_bindEvent;
		public static BindEvent BindEvent => m_bindEvent;
		private static BindEvent m_unbindEvent;
		public static BindEvent UnbindEvent => m_unbindEvent;

		private Selectable m_selectable;
		private Dictionary<string, List<int>> m_luaEvents;
		private static BindingEventDispatch m_dispatch;

		protected void TriggerPointerEvent(string eventName, IEnumerable<BindingEvent> events, object data)
		{
			if (m_selectable != null && !m_selectable.interactable)
				return;

			foreach (var evt in events)
			{
				var emmyFunction = evt.Function;
				switch (evt.Param.Type)
				{
					case EventParam.ParamType.None:
						var funcNone = emmyFunction.Binding.GetLuaMethod<NoneEventAction>(emmyFunction.LuaMethodName);
						funcNone(emmyFunction.Binding.LuaInstance, data);
						break;
					case EventParam.ParamType.Int:
						var funcInt = emmyFunction.Binding.GetLuaMethod<IntEventAction>(emmyFunction.LuaMethodName);
						funcInt(emmyFunction.Binding.LuaInstance, data, evt.Param.Int);
						break;
					case EventParam.ParamType.Float:
						var funcFloat = emmyFunction.Binding.GetLuaMethod<FloatEventAction>(emmyFunction.LuaMethodName);
						funcFloat(emmyFunction.Binding.LuaInstance, data, evt.Param.Float);
						break;
					case EventParam.ParamType.String:
						var funcStr = emmyFunction.Binding.GetLuaMethod<StringEventAction>(emmyFunction.LuaMethodName);
						funcStr(emmyFunction.Binding.LuaInstance, data, evt.Param.Str);
						break;
					case EventParam.ParamType.AssetRef:
						var funcAsset = emmyFunction.Binding.GetLuaMethod<AssetEventAction>(emmyFunction.LuaMethodName);
						funcAsset(emmyFunction.Binding.LuaInstance, data, evt.Param.AssetRef);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (m_luaEvents == null || !m_luaEvents.TryGetValue(eventName, out var eventIds))
			{
				return;
			}

			for (int i = 0; i < eventIds.Count;)
			{
				var id = eventIds[i];
				if (id == 0)
				{
					eventIds.RemoveAt(i);
				}
				else
				{
					m_dispatch(id, data);
					i++;
				}
			}
		}

	}
}
