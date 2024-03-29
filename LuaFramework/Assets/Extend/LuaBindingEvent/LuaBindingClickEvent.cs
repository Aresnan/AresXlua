﻿using AresLuaExtend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AresLuaExtend.LuaBindingEvent
{
	[RequireComponent(typeof(Button))]
	public class LuaBindingClickEvent : LuaBindingEventBase
	{
		[ReorderList, LabelText("On Click ()"), SerializeField]
		private List<BindingEvent> m_clickEvent;

		private void Start()
		{
			var button = GetComponent<Button>();
			button.onClick.AddListener(OnPointerClick);
		}

		public void OnPointerClick()
		{
			TriggerPointerEvent("OnClick", m_clickEvent, null);
		}
	}
}
