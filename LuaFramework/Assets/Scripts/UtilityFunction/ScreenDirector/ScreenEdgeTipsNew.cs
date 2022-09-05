using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEdgeTipsNew : MonoBehaviour
{
    /// <summary> 图片父物体，最好是全屏的Panel </summary>
    private RectTransform directContainer;
    /// <summary> 玩家自己或者摄像机标志位 </summary>
    private GameObject _cameraTarget;
    /// <summary> 目标物体</summary>
    public GameObject TargetObj;

    /// <summary> 主摄像机 </summary>
    public Camera mainCamera;
    /// <summary> UI摄像机 </summary>
    public Camera uicamera;
    private List<Line2D> screenLines;
    /// <summary> 在画面内的UI </summary>
    public Sprite InImage;
    /// <summary> 画面外的UI </summary>
    public Sprite OutImage;
    public InOrOut ImageType = InOrOut.None;
    RectTransform rect;
    private Image image;
    Vector2 finalPos;
    private Vector2 lookPos;
    public void Init()
    {
        _cameraTarget = new GameObject("Target");
        _cameraTarget.transform.parent = mainCamera.transform;
        _cameraTarget.transform.localPosition = new Vector3(0, 0, 0.5f);
        directContainer = transform.parent.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        finalPos = new Vector2();
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
    private void InitWidth(float width, float height)
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

    public void UpdateImp()
    {
        if (_cameraTarget != null)
        {
            Vector3 fromPos = this.WorldToScreenPoint(_cameraTarget.transform.position);
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
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(directContainer, toPos, uicamera, out finalPos);
                    rect.anchoredPosition = finalPos;//ui的绝对布局
                }

            }
            else
            {
                ChangeImage(InOrOut.Out);
                CacleIntersce(fromPos);
            }

        }

    }

    private void CacleIntersce(Vector3 fromPos)
    {
        Vector2 intersecPos = Vector2.zero;
        Vector3 localpos = _cameraTarget.transform.InverseTransformPoint(TargetObj.transform.position);
        Vector3 screenpos = new Vector3(localpos.x, localpos.y, 0);//根据场景坐标不同可以调整
        lookPos = fromPos + screenpos.normalized * 10000;
        Line2D line2 = new Line2D(fromPos, lookPos);
        foreach (Line2D l in this.screenLines)
        {
            if (line2.Intersection(l, out intersecPos) == Line2D.CROSS)
            {
                break;
            }
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(directContainer, intersecPos, uicamera, out finalPos);
        rect.anchoredPosition = finalPos;//ui的绝对布局
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
                image.sprite = InImage;
                break;
            case InOrOut.Out:
                image.sprite = OutImage;
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
                rect.right = lookPos.normalized;//根据图片轴向改变
            }
        }
    }
}


