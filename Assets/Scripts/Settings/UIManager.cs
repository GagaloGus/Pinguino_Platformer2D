using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Lives UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameObject pauseMenu;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLives)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        GameManager.instance.PauseGame();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        GameManager.instance.ResumeGame();
    }

    public void RestartLvl()
    {
        pauseMenu.SetActive(false);
        GameManager.instance.RestartLevel();
    }

    public void RestartGame()
    {
        pauseMenu.SetActive(false);
        //GameManager.instance.ResetRun();
        GameManager.instance.RestartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
