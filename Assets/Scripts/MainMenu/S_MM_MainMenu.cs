using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MM_MainMenu : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private S_GUIFadeInOut transition;
    [SerializeField] private S_MM_Settings settingsScreen;
    [SerializeField] private S_OverlayCanvas overlay;
#pragma warning restore 0649

    private bool gameStarting = false;
    //private S_OverlayCanvas overlay;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        settingsScreen.Init();   
        
        //overlay = S_OverlayCanvas.GetReference();
        overlay.Init();
    }


    public void BackToMenu()
    {
        settingsScreen.Hide();
        transition.FadeIn();
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void GoToSettings()
    {
        settingsScreen.Show();
        transition.FadeOut();
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void StartGame()
    {
        if(!gameStarting)
        {
            gameStarting = true;
            S_AudioManager.PlaySound(Sounds.Click);
            StartCoroutine(StartIE());
        }
    }

    private IEnumerator StartIE()
    {
        S_OverlayCanvas.Show();
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("TankScene");
    }

    public void Exit()
    {
        S_AudioManager.PlaySound(Sounds.Click);
        Application.Quit();
    }
}
