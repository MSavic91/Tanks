using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_GameManager : MonoBehaviour
{
    private static S_GameManager instance;

#pragma warning disable 0649
    [SerializeField] private S_WorldManager worldManager;
    [SerializeField] private S_VFX_Manager vfxManager;
#pragma warning restore 0649

    public static UnityEvent MissionOver;
    public static bool goToMenu;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (instance != null)
        {
            if (instance == this)
            {
                return;
            }
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        MissionOver = new UnityEvent();

        goToMenu = false;
        worldManager.Init();
        vfxManager.Init();
    }

}
