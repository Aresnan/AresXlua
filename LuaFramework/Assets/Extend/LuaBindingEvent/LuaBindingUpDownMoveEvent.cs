using AresLuaExtend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AresLuaExtend.LuaBindingEvent
{
	public class LuaBindingUpDownMoveEvent : LuaBindingEventBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		[ReorderList, LabelText("On Down ()"), SerializeField]
		private List<BindingEvent> m_downEvent;

		[ReorderList, LabelText("On Up ()"), SerializeField]
		private List<BindingEvent> m_upEvent;

		[ReorderList, LabelText("On Move ()"), SerializeField]
		private List<BindingEvent> m_moveEvent;

		public void OnPointerDown(PointerEventData eventData)
		{
			TriggerPointerEvent("OnDown", m_downEvent, eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			TriggerPointerEvent("OnUp", m_upEvent, eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			TriggerPointerEvent("OnDrag", m_moveEvent, eventData);
		}
	}
}
