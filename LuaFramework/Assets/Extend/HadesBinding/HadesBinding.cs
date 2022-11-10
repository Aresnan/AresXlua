using AresLuaExtend.Asset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace AresLuaExtend.HadesBinding
{
    [CSharpCallLua, LuaCallCSharp]
    public class HadesBinding : MonoBehaviour, ILuaMVVM
    {		
		[HadesBindOptions]
		public HadesBindingOptions BindingOptions;
		//minimum and maximum lines for the TextArea
		[TextArea(3, 5)]
		public string ExtraInfo;

		private LuaTable m_dataSource;
		public LuaTable DataSource
		{
			get => m_dataSource;
			set => SetDataContext(value);
		}
		public void Detach()
		{
			foreach (var option in BindingOptions.Options)
			{
				option.TryDetach();
			}
		}

		public LuaTable GetDataContext()
		{
			return m_dataSource;
		}

		public void SetDataContext(LuaTable dataSource)
		{
			m_dataSource?.Dispose();
			m_dataSource = dataSource;
			foreach (var option in BindingOptions.Options)
			{
				option.Bind(dataSource);
			}
		}
				
		void Awake()
        {
			BindingOptions.Sort();
			foreach (var option in BindingOptions.Options)
			{
				option.Prepare(gameObject);
			}
		}

    }
}