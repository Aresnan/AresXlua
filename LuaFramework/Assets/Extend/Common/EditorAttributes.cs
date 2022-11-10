using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AresLuaExtend.Common
{
	public interface IExtendAttribute
	{

	}
	public abstract class SpecialCaseAttribute : PropertyAttribute, IExtendAttribute
	{

	}
	[AttributeUsage(AttributeTargets.Field)]
	public class ReorderListAttribute : SpecialCaseAttribute
	{

	}

	[AttributeUsage(AttributeTargets.Field)]
	public class LabelTextAttribute : PropertyAttribute, IExtendAttribute
	{
		public string Text { get; }

		public LabelTextAttribute(string text)
		{
			Text = text;
		}
	}
}
