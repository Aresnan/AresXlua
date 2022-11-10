using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AresLuaExtend.Update.Operations
{
	public enum EUpdateOperationStatus
	{
		None,
		NeedWifi,
		NoWifi,
		Succeed,
		ConnectFailed,
		DownloadFailed,
		EnterGame
	}

	public enum EOperationStatus
	{
		None,
		Succeed,
		Failed
	}

	public abstract class UpdaterOperation
    {
		public static long TotalDownloadSize { get; protected set; } = 0;
		public EUpdateOperationStatus Status { get; protected set; } = EUpdateOperationStatus.None;

		public bool NeedUpdateInfo { get; protected set; } = false;

		public string PopupSizeInfo { get; protected set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		public string Error { get; protected set; }

		/// <summary>
		/// 处理进度
		/// </summary>
		public float Progress { get; protected set; }

		/// <summary>
		/// 下载信息
		/// </summary>
		public string DownloadInfo { get; protected set; }

		/// <summary>
		/// 是否已经完成
		/// </summary>
		public bool IsReady
		{
			get { return Status == EUpdateOperationStatus.Succeed; }
		}

		public virtual IEnumerator CheckWifi()
		{
			yield return 0;
		}

		public virtual string GetUpdateInfo()
		{
			return "";
		}

		public abstract IEnumerator Start();

		public virtual void Release()
		{

		}
		public virtual void Reset()
		{
			Status = EUpdateOperationStatus.None;
		}
	}
}