using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class S_AudioManager : MonoBehaviour
{
    private static S_AudioManager instance;

#pragma warning disable 0649
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer SFX_Group;
    [Header("Sounds")]
    [SerializeField] private AudioClip baseDestroySound;
    [SerializeField] private AudioClip destroyableDestroySound;
    [SerializeField] private AudioClip gameStartSound;
    [SerializeField] private AudioClip missionFailSound;
    [SerializeField] private AudioClip missionSuccessSound;
    [SerializeField] private AudioClip tankMoveSound;
    [SerializeField] private AudioClip tankShootSound;
    [SerializeField] private AudioClip tankDestroySound;
    [SerializeField] private AudioClip wallHitSound;
    [SerializeField] private AudioClip UI_ClickSound;
#pragma warning restore 0649

    public void Awake()
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
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    
    public static void PlaySound(Sounds sound) 
    {
        AudioClip clip = null;
        switch (sound)
        {   
            case Sounds.Click:
                clip = instance.UI_ClickSound;
                break;
            case Sounds.BaseDestroy:
                clip = instance.baseDestroySound;
                break;
            case Sounds.DestroyableDestroy:
                clip = instance.destroyableDestroySound;
                break;
            case Sounds.GameStart:
                clip = instance.gameStartSound;
                break;
            case Sounds.MissionFail:
                clip = instance.missionFailSound;
                break;
            case Sounds.MissionSuccess:
                clip = instance.missionSuccessSound;
                break;
            case Sounds.TankMove:
                clip = instance.tankMoveSound;
                break;
            case Sounds.TankShoot:
                clip = instance.tankShootSound;
                break;
            case Sounds.TankDestroy:
                clip = instance.tankDestroySound;
                break;
            case Sounds.WallHit:
                clip = instance.wallHitSound;
                break;
            default:
                break;
        }

        instance.audioSource.clip = clip;
        instance.audioSource.Play();
    }

    public static void SetSFXactive(bool active) 
    {
        if (active)
        {
            instance.audioSource.mute = true;
        }
        else
        {
            instance.audioSource.mute = false;
        }
    }
}
