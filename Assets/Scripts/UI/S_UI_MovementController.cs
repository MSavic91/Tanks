using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_UI_MovementController : MonoBehaviour
{
    private class Touch
    {
        public bool controlsController;
        public Vector2 startPosition;
        public Vector2 currentPosition;
        public float duration;
    }

    public static S_UI_MovementController instance;
    private const float TIME_TO_FIRE = 0.25f;

    public static float Size = 100f;

#pragma warning disable 0649
    [SerializeField] private RectTransform boundingRect;
    [SerializeField] private S_GUIFadeInOut controllerHolder;
    [SerializeField] private RectTransform controllerRT;
    [SerializeField] private RectTransform thumbIndicator;
#pragma warning restore 0649

    private List<Touch> touches;
    private bool controllerVisible;

    public static bool WhantsToMove = false;

    //public void Init()
    private void Start()
    {
        instance = this;

        Size = controllerRT.sizeDelta.x / 2f;

        touches = new List<Touch>();

        controllerVisible = false;
    }

    private void Update()
    {
        if (Time.timeScale < 0.01f)
        {
            return;
        }
        int newTouchCount = 0;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            newTouchCount = 1;
        }
        else
        {
            newTouchCount = 0;
        }
#else
        newTouchCount = Input.touchCount;
#endif
        //if (S_PlayerBlade.instance == null || (!S_PlayerBlade.instance.Alive || S_PlayerBlade.instance.Snapped))
        //{
        //    newTouchCount = 0;
        //}

        int lastTouchCount = touches.Count;

        if (newTouchCount < lastTouchCount)
        {
            for (int i = lastTouchCount - 1; i >= newTouchCount; i--)
            {
                UnregisterTouch(i);
            }
            lastTouchCount = touches.Count;
        }
        else if (newTouchCount > lastTouchCount)
        {
            for (int i = lastTouchCount; i < newTouchCount; i++)
            {
                SetupTouch(i);
            }
            lastTouchCount = touches.Count;
        }
        for (int i = 0; i < lastTouchCount; i++)
        {
            UpdateTouch(i);
        }

        if (Time.timeScale < 0.01f)
        {
            lastTouchCount = 0;
            touches.Clear();
            WhantsToMove = false;
        }
    }

    private void SetupTouch(int index)
    {

        Vector2 touchPosition = Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
        touchPosition = Input.mousePosition;
#else
        touchPosition = Input.GetTouch(index).position;
#endif
        //if (RectTransformUtility.RectangleContainsScreenPoint(portraitRect, touchPosition)
        //    || RectTransformUtility.RectangleContainsScreenPoint(attackRect, touchPosition)
        //    || RectTransformUtility.RectangleContainsScreenPoint(attackSpecialRect, touchPosition))
        //{
        //    //touches.Clear();
        //    return;
        //}

        Touch newTouch = new Touch() { };
        touches.Add(newTouch);

        Vector2 tapScreenPos = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(boundingRect, touchPosition, null, out tapScreenPos);
        newTouch.startPosition = newTouch.currentPosition = tapScreenPos;

        if (!controllerVisible)
        {
            ShowController(tapScreenPos);
            newTouch.controlsController = true;
        }
    }

    private void UpdateTouch(int index)
    {
        touches[index].duration += Time.unscaledDeltaTime;
        Vector2 touchPosition = Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
        touchPosition = Input.mousePosition;
#else
        touchPosition = Input.GetTouch(index).position;
#endif

        Vector2 tapScreenPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boundingRect, touchPosition, null, out tapScreenPos);
        touches[index].currentPosition = tapScreenPos;

        if (touches[index].controlsController)
        {
            Vector2 dir = MovementDirection;
            float dist = dir.magnitude;

            if (dist > Size)
            {
                var offset = MovementDirection - dir.normalized * Size;
                touches[index].startPosition += offset;
                instance.controllerRT.anchoredPosition += offset;
                dir = dir.normalized * Size;
            }

            if (Mathf.Abs(dir.x) < 50f && Mathf.Abs(dir.y) < 50f)
            {
                thumbIndicator.anchoredPosition = Vector2.zero;
                WhantsToMove = false;
            }
            else
            {
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    if (dir.x > 0)
                    {
                        thumbIndicator.anchoredPosition = Vector2.right * 100;
                    }
                    else
                    {
                        thumbIndicator.anchoredPosition = Vector2.left * 100;
                    }
                    WhantsToMove = true;
                }
                else
                {
                    if (dir.y > 0)
                    {
                        thumbIndicator.anchoredPosition = Vector2.up * 100;
                    }
                    else
                    {
                        thumbIndicator.anchoredPosition = Vector2.down * 100;
                    }
                    WhantsToMove = true;
                }
            }
        }
    }

    private void UnregisterTouch(int index)
    {
        if (touches[index].controlsController)
        {
            HideController();
            WhantsToMove = false;
        }
        if (touches[index].duration < TIME_TO_FIRE/* && S_PlayerBlade.instance.Alive && !S_PlayerBlade.instance.Snapped*/)
        {
            //Debug.Log("Fire!");
            //S_PlayerTank.instance.Fire();
            S_PlayerTank.FirePlayerStatic();
        }
        touches.RemoveAt(index);
    }

    public static void ShowController(Vector2 position)
    {
        if (instance != null)
        {
            instance.controllerVisible = true;
            instance.controllerHolder.FadeIn();
            instance.controllerRT.anchoredPosition = position;
        }
    }

    public static void HideController()
    {
        if (instance != null)
        {
            instance.controllerVisible = false;
            instance.controllerHolder.FadeOut();
        }
    }

    public static Vector2 MovementDirection
    {
        get
        {
            Vector2 result = Vector2.zero;
            int touchesCount = instance.touches.Count;
            for (int i = 0; i < touchesCount; i++)
            {
                if (instance.touches[i].controlsController)
                {
                    result = instance.touches[i].currentPosition - instance.touches[i].startPosition;
                    if (Mathf.Abs(result.x) > Mathf.Abs(result.y))
                    {
                        result = new Vector2(result.x, 0f);
                    }
                    else
                    {
                        result = new Vector2(0f, result.y);
                    }
                    break;
                }
            }
            return result;
        }
    }

    public static void ClearAllTouches()
    {
        instance.touches.Clear();
        HideController();
    }

    public void UpdateOrientation()
    {
        RectTransform parentRT = this.GetComponent<RectTransform>();
        float scale = Mathf.Max((parentRT.rect.width / parentRT.rect.height) - 0.3f, 1);
        
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}
