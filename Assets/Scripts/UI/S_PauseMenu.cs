using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_PauseMenu : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private S_GUIFadeInOut pauseMenuHolder;
    [SerializeField] private S_GUIFadeInOut missionEndHolder;
    [SerializeField] private TextMeshProUGUI missionCompleteText;
#pragma warning restore 0649

    private void Start()
    {
        S_GameManager.MissionOver.AddListener(GameEnd);
    }

    public void GameEnd() 
    {
        Time.timeScale = 0f;
        if (S_WorldManager.IsPlayerAlive() && !S_WorldManager.BaseDestroyed)
        {
            missionCompleteText.text = "mission accomplished";
            S_AudioManager.PlaySound(Sounds.MissionSuccess);
        }
        else
        {
            missionCompleteText.text = "mission failed";
            S_AudioManager.PlaySound(Sounds.MissionFail);
        }
        missionEndHolder.FadeIn();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenuHolder.FadeIn();
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuHolder.FadeOut();
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void BackToMenu()
    {
        S_GameManager.goToMenu = true;
        Time.timeScale = 1f;
        S_AudioManager.PlaySound(Sounds.Click);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator BackToMenuIE()
    {
        yield return new WaitForSecondsRealtime(1f);
    }

    public void ResetGame()
    {
        
        Time.timeScale = 1f;
        S_AudioManager.PlaySound(Sounds.Click);
        UnityEngine.SceneManagement.SceneManager.LoadScene("TankScene");
    }

    private IEnumerator ResetGameIE()
    {
        yield return new WaitForSecondsRealtime(1f);
    }



}
