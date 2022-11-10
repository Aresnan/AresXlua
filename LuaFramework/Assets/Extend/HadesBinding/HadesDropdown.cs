using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using XLua;

namespace AresLuaExtend.HadesBinding
{
	[RequireComponent(typeof(TMP_Dropdown))]
	public class HadesDropdown : MonoBehaviour, ILuaMVVM
	{
		public void Detach()
		{
			throw new NotImplementedException();
		}

		public LuaTable GetDataContext()
		{
			throw new NotImplementedException();
		}

		public void SetDataContext(LuaTable dataSource)
		{
			throw new NotImplementedException();
		}
	}
}
