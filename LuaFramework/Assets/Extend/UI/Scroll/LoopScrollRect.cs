using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

namespace AresLuaExtend.UI.Scroll
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform)), LuaCallCSharp]
	public abstract class LoopScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
	{
		public float minWidth => throw new NotImplementedException();

		public float preferredWidth => throw new NotImplementedException();

		public float flexibleWidth => throw new NotImplementedException();

		public float minHeight => throw new NotImplementedException();

		public float preferredHeight => throw new NotImplementedException();

		public float flexibleHeight => throw new NotImplementedException();

		public int layoutPriority => throw new NotImplementedException();

		public void CalculateLayoutInputHorizontal()
		{
			throw new NotImplementedException();
		}

		public void CalculateLayoutInputVertical()
		{
			throw new NotImplementedException();
		}

		public void GraphicUpdateComplete()
		{
			throw new NotImplementedException();
		}

		public void LayoutComplete()
		{
			throw new NotImplementedException();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			throw new NotImplementedException();
		}

		public void OnDrag(PointerEventData eventData)
		{
			throw new NotImplementedException();
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			throw new NotImplementedException();
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			throw new NotImplementedException();
		}

		public void OnScroll(PointerEventData eventData)
		{
			throw new NotImplementedException();
		}

		public void Rebuild(CanvasUpdate executing)
		{
			throw new NotImplementedException();
		}

		public void SetLayoutHorizontal()
		{
			throw new NotImplementedException();
		}

		public void SetLayoutVertical()
		{
			throw new NotImplementedException();
		}
	}
}
