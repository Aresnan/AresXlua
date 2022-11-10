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
	public class LuaBindingDragEvent : LuaBindingEventBase, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[ReorderList, LabelText("On Drag Start ()"), SerializeField]
		private List<BindingEvent> m_dragStartEvent;

		[ReorderList, LabelText("On Drag End ()"), SerializeField]
		private List<BindingEvent> m_dragEndEvent;

		[ReorderList, LabelText("On Drag ()"), SerializeField]
		private List<BindingEvent> m_dragEvent;

		public void OnBeginDrag(PointerEventData eventData)
		{
			TriggerPointerEvent("OnBeginDrag", m_dragStartEvent, eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			TriggerPointerEvent("OnDrag", m_dragEvent, eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			TriggerPointerEvent("OnEndDrag", m_dragEndEvent, eventData);
		}
	}
}
