using AresLuaExtend.Common;
using AresLuaExtend.HadesBinding.PropertyChange;
using AresLuaExtend.LuaBindingEvent;
using AresLuaExtend.LuaUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using XLua;

namespace AresLuaExtend.HadesBinding
{
	[Serializable]
	public class HadesBindingOptions
	{
		public HadesOption[] Options;
		public void Sort()
		{
			Array.Sort(Options);
		}
	}

	[Serializable]
	public class HadesOption : IComparable<HadesOption>
	{
		public int CompareTo(HadesOption other)
		{
			if (BindTargetProp == "SetActive")
			{
				return -1;
			}
			if (other.BindTargetProp == "SetActive")
			{
				return 1;
			}
			return BindTarget.GetInstanceID().CompareTo(other.BindTarget.GetInstanceID());
		}

		//显示指定枚举的底层数据类型 byte
		public enum BindMode : byte
		{
			ONE_WAY,
			TWO_WAY,
			ONE_WAY_TO_SOURCE,
			ONE_TIME,
			EVENT
		}

#if UNITY_EDITOR
		public static Action<GameObject> DebugCheckCallback;
#endif

		private static readonly Dictionary<Type, Type> m_sourceBindRelations = new Dictionary<Type, Type> {

		};
		//显示在Inspector的属性
		public Component BindTarget;
		public string BindTargetProp;
		public BindMode Mode = BindMode.ONE_TIME;
		public string Path;
		[SerializeField]
		private bool m_expression;

		private LuaTable m_dataSource;
		private PropertyInfo m_propertyInfo;
		private WatchCallback watchCallback;
		private DetachLuaProperty detach;
		private IUnityPropertyChanged m_propertyChangeCallback;
		private LuaFunction m_bindFunc;
		private bool m_prepared;

		public void Bind(LuaTable dataContext)
		{
			if (m_dataSource != null)
			{
				TryDetach();
			}
			object bindingValue = null;
			try
			{
				m_dataSource = dataContext;
				if (m_expression)
				{
					var function = TempBindingExpressCache.GenerateTempFunction(ref Path);
					using (var setupTempFunc = m_dataSource.GetInPath<LuaFunction>("setup_temp_getter"))
					{
						bindingValue = setupTempFunc.Func<string, LuaFunction, object>(GetExpressionKey(), function);
					}
				}
				else
				{
					if (m_dataSource == null)
					{
						return;
					}
					if (Mode == BindMode.EVENT)
					{
						m_bindFunc = dataContext.GetInPath<LuaFunction>(Path);
						Assert.IsNotNull(m_bindFunc);
						LuaBindingEventBase.BindEvent(BindTargetProp, BindTarget.gameObject, m_bindFunc);
						return;
					}
					bindingValue = Path == "self" ? dataContext : dataContext.GetInPath<object>(Path);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			if (bindingValue == null && Mode != BindMode.ONE_WAY_TO_SOURCE)
			{
				// Debug.LogWarning($"Not found value in path {Path}");
				return;
			}
			switch (Mode)
			{
				case BindMode.ONE_WAY:
				case BindMode.TWO_WAY:
				case BindMode.ONE_TIME:
					{
						if (Mode == BindMode.ONE_WAY || Mode == BindMode.TWO_WAY)
						{
							var watch = dataContext.GetInPath<WatchLuaProperty>("watch");
							watch(dataContext, m_expression ? GetExpressionKey() : Path, watchCallback, m_expression);
							detach = dataContext.Get<DetachLuaProperty>("detach");
							Assert.IsNotNull(detach);

							if (Mode == BindMode.TWO_WAY)
							{
								if (m_expression)
								{
									Debug.LogError("express type can not to source");
								}
								else
								{
									m_propertyChangeCallback = GetOrAddPropertyChange();
									m_propertyChangeCallback.OnPropertyChanged += OnPropertyChanged;
								}
							}
						}
						SetPropertyValue(bindingValue);
						break;
					}
				case BindMode.ONE_WAY_TO_SOURCE:
					if (m_expression)
					{
						Debug.LogError("express type can not to source");
						return;
					}
					m_propertyChangeCallback = GetOrAddPropertyChange();
					dataContext.SetInPath(Path, m_propertyChangeCallback.ProvideCurrentValue());
					m_propertyChangeCallback.OnPropertyChanged += OnPropertyChanged;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		//每次bind之前分离之前已经bind的属性
		public void TryDetach()
		{
			if (!CSharpServiceManager.Initialized)
				return;
			if (m_dataSource == null)
				return;

			if (Mode == BindMode.EVENT)
			{
				if (m_bindFunc == null || !BindTarget)
					return;
				LuaBindingEventBase.UnbindEvent(BindTargetProp, BindTarget.gameObject, m_bindFunc);
				m_bindFunc?.Dispose();
				m_bindFunc = null;
				m_dataSource.Dispose();
				m_dataSource = null;
				return;
			}

			if (detach != null && (Mode == BindMode.ONE_WAY || Mode == BindMode.TWO_WAY))
			{
				detach(m_dataSource, m_expression ? GetExpressionKey() : Path, watchCallback);
				detach = null;
			}

			m_dataSource.Dispose();
			m_dataSource = null;

			if (BindTarget is ILuaMVVM mvvm)
			{
				mvvm.Detach();
			}

			if (m_propertyChangeCallback != null)
			{
				m_propertyChangeCallback.OnPropertyChanged -= OnPropertyChanged;
			}
		}
		public string GetExpressionKey()
		{
			return $"{Path}_{BindTarget.gameObject.GetInstanceID()}".GetHashCode().ToString();
		}
		private void OnPropertyChanged(Component sender, object value)
		{
			m_dataSource.SetInPath(Path, value);
		}
		private IUnityPropertyChanged GetOrAddPropertyChange()
		{
			return BindTarget.GetOrAddComponent(m_sourceBindRelations[BindTarget.GetType()]) as IUnityPropertyChanged;
		}
		private void SetPropertyValue(object val)
		{
			if (!m_prepared)
			{
				Debug.LogError($"LuaMVVMBinding {BindTarget}.{Path} not ready, dont SetDataContext in awake.");
				return;
			}
#if UNITY_EDITOR
			if (BindTarget)
			{
				DebugCheckCallback?.Invoke(BindTarget.gameObject);
			}
			else
			{
				Debug.Log("");
			}
#endif
#if UNITY_DEBUG
			StatService.Get().Increase(StatService.StatName.MVVM_DISPATCH, 1);
#endif
			try
			{
				if (m_propertyInfo == null)
				{
					BindTarget.gameObject.SetActive((bool)val);
				}
				else
				{
					if (m_propertyInfo.PropertyType == typeof(string))
					{
						m_propertyInfo.SetValue(BindTarget, val == null ? "" : val.ToString());
					}
					else if (m_propertyInfo.PropertyType == typeof(float))
					{
						if (val is long i)
						{
							m_propertyInfo.SetValue(BindTarget, (float)i);
						}
						else
						{
							m_propertyInfo.SetValue(BindTarget, (float)(double)val);
						}
					}
					else if (m_propertyInfo.PropertyType == typeof(int))
					{
						if (val is long i)
						{
							m_propertyInfo.SetValue(BindTarget, (int)i);
						}
						else
						{
							m_propertyInfo.SetValue(BindTarget, (int)(double)val);
						}
					}
					else
					{
						m_propertyInfo.SetValue(BindTarget, val);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"MVVM Set Property Error : {BindTarget}.{BindTargetProp} = {Path}:{val}");
				Debug.LogError(e);
			}
		}

		public void Prepare(GameObject owner)
		{
			if (!BindTarget)
			{
				Debug.LogError($"Binding target is null, Path : {Path} Property : {BindTargetProp}, Owner: {owner.name}");
				return;
			}

			m_propertyInfo = BindTargetProp == "SetActive" ? null : BindTarget.GetType().GetProperty(BindTargetProp);
			watchCallback = SetPropertyValue;
			m_prepared = true;
		}

		public void Destroy()
		{
			TryDetach();
		}
	}
}
