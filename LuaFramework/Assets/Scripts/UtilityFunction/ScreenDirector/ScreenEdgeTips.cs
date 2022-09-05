using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEdgeTips : MonoBehaviour
{
	/// <summary> 图片父物体，最好是全屏的Panel </summary>
	private RectTransform directContainer;
	/// <summary> 玩家自己或者摄像机标志位 </summary>
	public GameObject Player;
	/// <summary> 目标物体</summary>
	public GameObject TargetObj;

	/// <summary> 主摄像机 </summary>
	public Camera mainCamera;
	/// <summary> UI摄像机 </summary>
	public Camera uicamera;
	private List<Line2D> screenLines;
	/// <summary> 在画面内的UI </summary>
	public GameObject InImage;
	/// <summary> 画面外的UI </summary>
	public GameObject OutImage;
	public InOrOut ImageType = InOrOut.None;
	RectTransform rect;
	RectTransform arrow;
	private float lookPos;
	public void Init()
	{
		directContainer = transform.parent.GetComponent<RectTransform>();
		rect = GetComponent<RectTransform>();
		arrow = OutImage.GetComponent<RectTransform>();
	}

	private void Update()
	{
		UpdateImp();
		UpdateLookAt();
	}

	private void Start()
	{
		Init();
		InitWidth(50, 50);
	}
	private void InitWidth(float width,float height)
	{
		Vector3 point1 = new Vector3(width, height);
		Vector3 point2 = new Vector3(width, Screen.height - height);
		Vector3 point3 = new Vector3(Screen.width - width, Screen.height - height); //     P2------------P3           
		Vector3 point4 = new Vector3(Screen.width - width, height);                //       |           |
		this.screenLines = new List<Line2D>();                                   //       |           |
		this.screenLines.Add(new Line2D(point1, point2));                        //     P1------------ P4
		this.screenLines.Add(new Line2D(point2, point3));
		this.screenLines.Add(new Line2D(point3, point4));
		this.screenLines.Add(new Line2D(point4, point1));
	}

	/// <summary>
	/// 点是否在屏幕内
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	private bool PointIsInScreen(Vector3 pos)
	{
		if (pos.x <= this.screenLines[0].point1.x
			|| pos.x >= this.screenLines[1].point2.x
			|| pos.y <= this.screenLines[0].point1.y
			|| pos.y >= this.screenLines[1].point2.y)
		{
			return false;
		}
		return true;
	}

	//世界坐标转换为屏幕坐标
	private Vector3 WorldToScreenPoint(Vector3 pos)
	{
		if (null != this.mainCamera)
		{
			return mainCamera.WorldToScreenPoint(pos);
		}
		return Vector3.zero;
	}
	Vector2 finalPos;
	public void UpdateImp()
	{
		if (Player != null)
		{
			Vector3 fromPos = WorldToScreenPoint(Player.transform.position);
			Vector3 toPos = WorldToScreenPoint(TargetObj.transform.position);

			if (PointIsInScreen(toPos))//如果目标在屏幕内
			{
				if (toPos.z < 0)
				{
					ChangeImage(InOrOut.Out);
					CacleIntersce(fromPos);
				}
				else
				{
					ChangeImage(InOrOut.In);
					RectTransformUtility.ScreenPointToLocalPointInRectangle(directContainer, toPos, null, out finalPos);
					rect.anchoredPosition = finalPos;//ui的绝对布局
					Debug.Log("Screen postion is : " + toPos + "/" + finalPos);
				}

			}
			else
			{
				ChangeImage(InOrOut.Out);
				CacleIntersce(fromPos);
			}

		}

	}

	private void CacleIntersce(Vector2 fromPos)
	{
		Vector2 intersecPos = Vector2.zero;
		Vector3 toPos = WorldToScreenPoint(TargetObj.transform.position);
		if (Mathf.Abs(toPos.x) > 30000 || Mathf.Abs(toPos.y) > 30000)
		{
			toPos /= 100;
		}
		if (toPos.z < 0)
		{
			toPos = -toPos;
		}
		Line2D line2 = new Line2D(fromPos, toPos);
		foreach (Line2D l in this.screenLines)
		{
			if (line2.Intersection(l, out intersecPos) == Line2D.CROSS)
			{
				break;
			}
		}
		lookPos = Vector2.Angle(Vector2.up, intersecPos);
		Debug.Log(intersecPos + "/" + toPos);
		float x = intersecPos.x > Screen.width / 2 ? intersecPos.x - rect.sizeDelta.x / 2 : intersecPos.x + rect.sizeDelta.x / 2;
		float y = intersecPos.y > Screen.height / 2 ? intersecPos.y - rect.sizeDelta.y / 2 : intersecPos.y + rect.sizeDelta.y / 2;
		Vector2 finalToPos = new Vector2(x, y);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(directContainer, finalToPos, null, out finalPos);
		rect.anchoredPosition = finalPos;

		//计算角度
		RectTransformUtility.ScreenPointToLocalPointInRectangle(directContainer, fromPos, null, out finalPos);
		Vector2 dir = rect.anchoredPosition - finalPos;
		lookPos = Vector2.SignedAngle(dir, Vector2.up);
		//Debug.Log(lookPos);
	}

	/// <summary>
	/// 改变图片
	/// </summary>
	private void ChangeImage(InOrOut type)
	{
		if (type == ImageType)
		{
			return;
		}
		switch (type)
		{
			case InOrOut.In:
				InImage.SetActive(false);
				break;
			case InOrOut.Out:
				InImage.SetActive(true);
				break;
			case InOrOut.None:
				break;
			default:
				break;
		}
		ImageType = type;
	}
	private void UpdateLookAt()
	{
		if (TargetObj != null)
		{
			if (ImageType == InOrOut.Out)
			{
				arrow.localEulerAngles = new Vector3(0f, 0f, -lookPos);
			}
		}
	}
}
public enum InOrOut
{
	In, Out, None
}

