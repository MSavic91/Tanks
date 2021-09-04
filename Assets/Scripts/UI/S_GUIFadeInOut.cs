using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles various GUI transformations such as fade in, out, slide transitions, scaling, etc.
/// </summary>

[RequireComponent(typeof(CanvasGroup))]
public class S_GUIFadeInOut : MonoBehaviour
{
    public enum AnimationType {
        Fade = 1,
        ScaleIn = 2,
        FlyInLeft = 4,
        FlyInRight = 8,
        FlyInBottom = 16,
        FlyInTop = 32,
        ScaleInRect = 64
    };

    public enum EasingType { Linear, EaseInSine, EaseOutSine, EaseInOutSine, EaseInSquare, EaseOutSquare, EaseInOutSquare, EaseInFourth, EaseOutFourth, EaseInOutFourth };

    private float t = 1;
	private bool fade = true;
	public AnimationType fadeInType = AnimationType.Fade;
	public AnimationType fadeOutType = AnimationType.Fade;
	public EasingType easingType = EasingType.Linear;
	public bool animateAlpha = true;
	public static float speed = 3;
	public float speedMultiplier = 1;
	public float startDistance = 1080;
	public bool initiallyHidden = false;
	private Vector2 startPosition;
	private Vector3 startSize;
	public Vector2 rectSize;

    private RectTransform rt;
    private CanvasGroup cg;

	void Awake () {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        if (rt != null){
			startPosition = rt.anchoredPosition;
		}
	}

	// Use this for initialization
	void Start () {
		if (initiallyHidden)
		{
			t = 0;
			fade = false;
			if(animateAlpha && cg != null){
                cg.alpha = 0;
			}
			gameObject.SetActive(false);
			if(rt){
				startPosition = rt.anchoredPosition;
				if(fadeInType == AnimationType.ScaleInRect){
					rt.sizeDelta = Vector2.zero;
				}
			}
			startSize = transform.localScale;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (fade && t < 1)
		{
			t += Time.unscaledDeltaTime * speed * speedMultiplier;
			if(t >= 1)
			{
				t = 1;
                if (cg != null)
                {
                    cg.blocksRaycasts = true;
                    cg.interactable = true;
                }
			}

			float x = t;
			switch(easingType){
			case EasingType.EaseInOutSine:
				x = SineEaseInOut(x);
				break;
			case EasingType.EaseInSine:
				x = SineEaseIn(x);
				break;
			case EasingType.EaseOutSine:
				x = SineEaseOut(x);
				break;
			case EasingType.EaseInOutSquare:
				x = SquareEaseInOut(x);
				break;
			case EasingType.EaseInSquare:
				x = SquareEaseIn(x);
				break;
			case EasingType.EaseOutSquare:
				x = SquareEaseOut(x);
				break;
			case EasingType.EaseInOutFourth:
				x = FourthEaseInOut(x);
				break;
			case EasingType.EaseInFourth:
				x = FourthEaseIn(x);
				break;
			case EasingType.EaseOutFourth:
				x = FourthEaseOut(x);
				break;
			}

			switch (fadeInType) {
			case AnimationType.ScaleIn:
				transform.localScale = Vector3.Lerp(Vector3.one * 0.001f, startSize, Mathf.Clamp01(x));
				break;
			case AnimationType.FlyInLeft:
				rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.left * startDistance, startPosition, Mathf.Clamp01(x));
				break;
			case AnimationType.FlyInRight:
				rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.right * startDistance, startPosition, Mathf.Clamp01(x));
				break;
			case AnimationType.FlyInTop:
				rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.up * startDistance, startPosition, Mathf.Clamp01(x));
				break;
			case AnimationType.FlyInBottom:
				rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.down * startDistance, startPosition, Mathf.Clamp01(x));
				break;
			case AnimationType.ScaleInRect:
				rt.sizeDelta = new Vector2(rectSize.x * Mathf.Clamp01( t * 2 ), rectSize.y * Mathf.Clamp01( -1 + x * 2 ));
				break;
			}

			if(animateAlpha || fadeInType == AnimationType.Fade)
            {
                if (cg != null)
                {
                    cg.alpha = Mathf.Clamp01(x);
                }
			}
		}
		if(!fade && t > 0)
		{
			t -= Time.unscaledDeltaTime * speed * speedMultiplier;
			if (t <= 0)
			{
				t = 0;
				gameObject.SetActive(false);
			}

			float x = t;
			switch(easingType){
			    case EasingType.EaseInOutSine:
				    x = SineEaseInOut(x);
				    break;
			    case EasingType.EaseInSine:
				    x = SineEaseIn(x);
				    break;
			    case EasingType.EaseOutSine:
				    x = SineEaseOut(x);
				    break;
			    case EasingType.EaseInOutSquare:
				    x = SquareEaseInOut(x);
				    break;
			    case EasingType.EaseInSquare:
				    x = SquareEaseIn(x);
				    break;
			    case EasingType.EaseOutSquare:
				    x = SquareEaseOut(x);
				    break;
			    case EasingType.EaseInOutFourth:
				    x = FourthEaseInOut(x);
				    break;
			    case EasingType.EaseInFourth:
				    x = FourthEaseIn(x);
				    break;
			    case EasingType.EaseOutFourth:
				    x = FourthEaseOut(x);
				    break;
			}
			switch (fadeOutType) {
			    case AnimationType.ScaleIn:
				    transform.localScale = Vector3.Lerp(Vector3.one * 0.001f, startSize, x);
				    break;
			    case AnimationType.FlyInLeft:
				    rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.left * startDistance, startPosition, x);
				    break;
			    case AnimationType.FlyInRight:
				    rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.right * startDistance, startPosition, x);
				    break;
			    case AnimationType.FlyInTop:
				    rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.up * startDistance, startPosition, x);
				    break;
			    case AnimationType.FlyInBottom:
				    rt.anchoredPosition = Vector2.Lerp(startPosition + Vector2.down * startDistance, startPosition, x);
				    break;
			    case AnimationType.ScaleInRect:
				    rt.sizeDelta = new Vector2(rectSize.x * Mathf.Clamp01( x * 2 ), rectSize.y * Mathf.Clamp01( -1 + x * 2 ));
				    break;
			}

			if((animateAlpha || fadeOutType == AnimationType.Fade) && cg != null){
				cg.alpha = Mathf.Min(cg.alpha, x);
			}
		}
	}

	public void FadeInWithDelay(float delay){
        if(cg != null)
        {
            cg.alpha = 0;
        }
		t = -delay * speed * speedMultiplier;
		FadeIn ();
	}
	
	public void FadeIn()
	{
		if (initiallyHidden) {
			if(animateAlpha || fadeInType == AnimationType.Fade){
				if(cg != null){
				    cg.alpha = 0;
				}
			}
            if(rt == null)
            {
                rt = GetComponent<RectTransform>();
                cg = GetComponent<CanvasGroup>();
            }

			if(fadeOutType == AnimationType.ScaleIn){
				transform.localScale = Vector3.one;
			} else if(fadeOutType == AnimationType.ScaleInRect){
				rt.sizeDelta = rectSize;
			} else {
				rt.anchoredPosition = startPosition;
			}

			initiallyHidden = false;
			switch(fadeInType){
			case AnimationType.ScaleIn:
				transform.localScale = new Vector3(1, 0, 1);
				break;
			case AnimationType.FlyInLeft:
				rt.anchoredPosition = startPosition + Vector2.left * startDistance;
				break;
			case AnimationType.FlyInRight:
				rt.anchoredPosition = startPosition + Vector2.right * startDistance;
				break;
			case AnimationType.FlyInTop:
				rt.anchoredPosition = startPosition + Vector2.up * startDistance;
				break;
			case AnimationType.FlyInBottom:
				rt.anchoredPosition = startPosition + Vector2.down * startDistance;
				break;
			case AnimationType.ScaleInRect:
				rt.sizeDelta = Vector2.zero;
				break;
			}
		}
		gameObject.SetActive(true);
		t = Mathf.Min(t, 0);
		fade = true;
    }

    public void FadeOutWithDelay(float delay)
    {
        t = 1 + delay * speed * speedMultiplier;
        FadeOut();
    }

    public void FadeOut()
	{
		if(cg != null){
		    cg.blocksRaycasts = false;
		    cg.interactable = false;
		}
		t = Mathf.Min(t, 1);
		fade = false;
    }

    public void Toggle()
    {
        if(fade)
        {
            FadeOut();
        }
        else
        {
            FadeIn();
        }
    }

    public static float SineEaseInOut(float x)
    {
        return Mathf.Sin(x * Mathf.PI - Mathf.PI / 2) / 2 + 0.5f;
    }

    public static float SineEaseOut(float x)
    {
        return Mathf.Sin(x * Mathf.PI / 2);
    }

    public static float SineEaseIn(float x)
    {
        return 1 - Mathf.Sin(x * Mathf.PI / 2 + Mathf.PI / 2);
    }

    public static float SquareEaseInOut(float x)
    {
        return x * x / (x * x + (1 - x) * (1 - x));
    }

    public static float SquareEaseIn(float x)
    {
        return SquareEaseInOut(x / 2) * 2;
    }

    public static float SquareEaseOut(float x)
    {
        return SquareEaseInOut((x + 1) / 2) * 2 - 1;
    }

    public static float FourthEaseInOut(float x)
    {
        return x * x * x * x / (x * x * x * x + (1 - x) * (1 - x) * (1 - x) * (1 - x));
    }

    public static float FourthEaseIn(float x)
    {
        return FourthEaseInOut(x / 2) * 2;
    }

    public static float FourthEaseOut(float x)
    {
        return FourthEaseInOut((x + 1) / 2) * 2 - 1;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(S_GUIFadeInOut))]
public class S_GUIFadeInOut_Editor : Editor
{
    S_GUIFadeInOut t;
    SerializedObject getTarget;

    private void OnEnable()
    {
        t = (S_GUIFadeInOut)target;
        getTarget = new SerializedObject(t);
    }

    public override void OnInspectorGUI()
    {
        getTarget.Update();

        EditorGUI.BeginChangeCheck();

        SerializedProperty initiallyHidden = getTarget.FindProperty("initiallyHidden");
        EditorGUILayout.PropertyField(initiallyHidden);

        SerializedProperty speedMultiplier = getTarget.FindProperty("speedMultiplier");
        EditorGUILayout.PropertyField(speedMultiplier);

        SerializedProperty easingType = getTarget.FindProperty("easingType");
        EditorGUILayout.PropertyField(easingType);

        SerializedProperty fadeInType = getTarget.FindProperty("fadeInType");
        EditorGUILayout.PropertyField(fadeInType);
        SerializedProperty fadeOutType = getTarget.FindProperty("fadeOutType");
        EditorGUILayout.PropertyField(fadeOutType);

        var fadeIn = (S_GUIFadeInOut.AnimationType)fadeInType.intValue;
        var fadeOut = (S_GUIFadeInOut.AnimationType)fadeOutType.intValue;

        if (fadeIn != S_GUIFadeInOut.AnimationType.Fade || fadeOut != S_GUIFadeInOut.AnimationType.Fade)
        {
            SerializedProperty animateAlpha = getTarget.FindProperty("animateAlpha");
            EditorGUILayout.PropertyField(animateAlpha);
        }

        if(fadeIn == S_GUIFadeInOut.AnimationType.ScaleInRect || fadeOut == S_GUIFadeInOut.AnimationType.ScaleInRect)
        {
            SerializedProperty rectSize = getTarget.FindProperty("rectSize");
            EditorGUILayout.PropertyField(rectSize);
        }

        if ((fadeIn & (S_GUIFadeInOut.AnimationType.FlyInBottom |
            S_GUIFadeInOut.AnimationType.FlyInLeft |
            S_GUIFadeInOut.AnimationType.FlyInRight |
            S_GUIFadeInOut.AnimationType.FlyInTop)) > 0 ||
            (fadeOut & (S_GUIFadeInOut.AnimationType.FlyInBottom |
            S_GUIFadeInOut.AnimationType.FlyInLeft |
            S_GUIFadeInOut.AnimationType.FlyInRight |
            S_GUIFadeInOut.AnimationType.FlyInTop)) > 0)
        {
            SerializedProperty startDistance = getTarget.FindProperty("startDistance");
            EditorGUILayout.PropertyField(startDistance);
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }

        getTarget.ApplyModifiedProperties();
    }
}
#endif