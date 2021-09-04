using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_OverlayCanvas : MonoBehaviour
{
    private static S_OverlayCanvas instance;

#pragma warning disable 0649
    [SerializeField] private S_GUIFadeInOut holder;
#pragma warning restore 0649

    public void Init()
    {
        if(instance == this)
        {
            return;
        }

        //if(instance != null)
        //{
        //    Destroy(gameObject);
        //}

        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public static void Show()
    {
        instance.holder.FadeIn();
    }

    public static void Hide()
    {
        if (instance == null)
        {
            return;
        }
        instance.holder.FadeOut();
    }

    public static S_OverlayCanvas GetReference() 
    {
        return instance;
    }
}
