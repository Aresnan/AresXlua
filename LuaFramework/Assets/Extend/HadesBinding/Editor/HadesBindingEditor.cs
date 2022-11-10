using AresLuaExtend.Common;
using AresLuaExtend.LuaBindingEvent;
using AresLuaExtend.UI.Scroll;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AresLuaExtend.HadesBinding.Editor
{
	[CustomEditor(typeof(HadesBinding))]
	public class HadesBindingEditor : UnityEditor.Editor
	{
		//Implement this function to make a custom inspector.
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("HadesBinding"))
			{
				// Button
				var binding = target as HadesBinding;
				var buttons = binding.GetComponentsInChildren<Button>(true);
				foreach (Button button in buttons)
				{
					button.GetOrAddComponent<LuaBindingClickEvent>();
				}

				// Dropdown
				FindBinding<TMP_Dropdown, HadesDropdown>(binding, "LuaArrayData");
				// ScrollRect
				FindBinding<ScrollRect, HadesSystemScroll>(binding, "LuaArrayData");
				// LoopRect
				FindBinding<LoopScrollRect, HadesLoopScroll>(binding, "LuaArrayData");

				serializedObject.UpdateIfRequiredOrScript();
			}
		}

		private static void FindBinding<ComponentT, MvvmT>(HadesBinding binding, string propertyName) where ComponentT : Component where MvvmT : Component
		{
			var dropdowns = binding.GetComponentsInChildren<ComponentT>();
			foreach (ComponentT dropdown in dropdowns)
			{
				var mvvmDropdown = dropdown.GetOrAddComponent<MvvmT>();
				var exist = binding.BindingOptions.Options.Any(option => option.BindTarget == mvvmDropdown);
				if (!exist)
				{
					ArrayUtility.Add(ref binding.BindingOptions.Options, new HadesOption()
					{
						BindTarget = mvvmDropdown,
						Mode = HadesOption.BindMode.ONE_TIME,
						BindTargetProp = propertyName
					});
				}
			}
		}
	}
}
