using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_MM_Settings : MonoBehaviour
{
    public const string SOUND_ACTIVE = "sound-active";
    public const string SHOW_PATHS = "show-paths";
    public const string TERRAIN_WIDTH = "terrain-width";
    public const string TERRAIN_HEIGHT = "terrain-height";
    public const string OBSTACLES_COUNT = "obstacles-count";
    public const string ENEMIES_COUNT = "enemies-count";

#pragma warning disable 0649
    [SerializeField] private S_GUIFadeInOut transition;

    [Header("Sound")]
    [SerializeField] private GameObject soundCheckbox;
    [SerializeField] private GameObject showPathsCheckbox;
    [Header("Terrain")]
    [SerializeField] private TextMeshProUGUI widthLabel;
    [SerializeField] private Slider widthSlider;
    [SerializeField] private TextMeshProUGUI heightLabel;
    [SerializeField] private Slider heightSlider;
    [Header("Obstacles")]
    [SerializeField] private TextMeshProUGUI obstaclesLabel;
    [SerializeField] private Slider obstaclesSlider;
    [Header("Enemies")]
    [SerializeField] private TextMeshProUGUI enemiesLabel;
    [SerializeField] private Slider enemiesSlider;
#pragma warning restore 0649

    public void Init()
    {
        bool soundActive = true;
        if(PlayerPrefs.HasKey(SOUND_ACTIVE))
        {
            soundActive = PlayerPrefs.GetInt(SOUND_ACTIVE) == 1;
        }
        else
        {
            PlayerPrefs.SetInt(SOUND_ACTIVE, soundActive ? 1 : 0);
        }
            
        SetSoundActive(soundActive);

        bool showPaths = true;
        if(PlayerPrefs.HasKey(SHOW_PATHS))
        {
            showPaths = PlayerPrefs.GetInt(SHOW_PATHS) == 1;
        }
        else
        {
            PlayerPrefs.SetInt(SHOW_PATHS, showPaths ? 1 : 0);
        }
        SetShowPathsActive(showPaths);

        int terrainWidth = 9;
        if(PlayerPrefs.HasKey(TERRAIN_WIDTH))
        {
            terrainWidth = PlayerPrefs.GetInt(TERRAIN_WIDTH);
        }
        else
        {
            PlayerPrefs.SetInt(TERRAIN_WIDTH, terrainWidth);
        }
        SetTerrainWidth(terrainWidth);

        int terrainHeight = 17;
        if (PlayerPrefs.HasKey(TERRAIN_HEIGHT))
        {
            terrainHeight = PlayerPrefs.GetInt(TERRAIN_HEIGHT);
        }
        else
        {
            PlayerPrefs.SetInt(TERRAIN_HEIGHT, terrainHeight);
        }
        SetTerrainHeight(terrainHeight);

        int obstaclesCount = 20;
        if (PlayerPrefs.HasKey(OBSTACLES_COUNT))
        {
            obstaclesCount = PlayerPrefs.GetInt(OBSTACLES_COUNT);
        }
        else
        {
            PlayerPrefs.SetInt(OBSTACLES_COUNT, obstaclesCount);
        }
        SetObstaclesCount(obstaclesCount);

        int enemiesCount = 4;
        if (PlayerPrefs.HasKey(ENEMIES_COUNT))
        {
            enemiesCount = PlayerPrefs.GetInt(ENEMIES_COUNT);
        }
        else
        {
            PlayerPrefs.SetInt(ENEMIES_COUNT, enemiesCount);
        }
        SetEnemiesCount(enemiesCount);
    }

    public void Show()
    {
        transition.FadeIn();
    }

    public void Hide()
    {
        transition.FadeOut();
    }

    public void ToggleSound()
    {
        bool soundActive = PlayerPrefs.GetInt(SOUND_ACTIVE) == 1;
        SetSoundActive(!soundActive);
        S_AudioManager.PlaySound(Sounds.Click);
    }

    private void SetSoundActive(bool active)
    {
        soundCheckbox.SetActive(active);
        S_AudioManager.SetSFXactive(!active);
        PlayerPrefs.SetInt(SOUND_ACTIVE, active ? 1 : 0);
    }

    public void ToggleShowPaths()
    {
        bool showPaths = PlayerPrefs.GetInt(SHOW_PATHS) == 1;
        SetShowPathsActive(!showPaths);
        S_AudioManager.PlaySound(Sounds.Click);
    }

    private void SetShowPathsActive(bool active)
    {
        showPathsCheckbox.SetActive(active);

        PlayerPrefs.SetInt(SHOW_PATHS, active ? 1 : 0);
    }

    public void SetTerrainWidth(float width)
    {
        widthSlider.SetValueWithoutNotify(width);
        widthLabel.text = string.Format("Width: {0}", width);

        PlayerPrefs.SetInt(TERRAIN_WIDTH, Mathf.RoundToInt(width));
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void SetTerrainHeight(float height)
    {
        heightSlider.SetValueWithoutNotify(height);
        heightLabel.text = string.Format("Height: {0}", height);

        PlayerPrefs.SetInt(TERRAIN_HEIGHT, Mathf.RoundToInt(height));
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void SetObstaclesCount(float obstacles)
    {
        obstaclesSlider.SetValueWithoutNotify(obstacles);
        obstaclesLabel.text = string.Format("Obstacles: {0}", obstacles);

        PlayerPrefs.SetInt(OBSTACLES_COUNT, Mathf.RoundToInt(obstacles));
        S_AudioManager.PlaySound(Sounds.Click);
    }

    public void SetEnemiesCount(float enemies)
    {
        enemiesSlider.SetValueWithoutNotify(enemies);
        enemiesLabel.text = string.Format("Enemies: {0}", enemies);

        PlayerPrefs.SetInt(ENEMIES_COUNT, Mathf.RoundToInt(enemies));
        S_AudioManager.PlaySound(Sounds.Click);
    }
}
