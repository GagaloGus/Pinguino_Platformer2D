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
    public Sprite fullHeart;
    public Sprite emptyHeart;
    Transform HeartsContainer;
    GameObject pauseMenu;

    private void Awake()
    {
        instance = this;
        HeartsContainer = transform.Find("HeartsContainer");
        pauseMenu = transform.Find("PauseMenu").gameObject;
    }

    public void UpdateLives()
    {
        for (int i = 0; i < HeartsContainer.childCount; i++)
        {
            GameObject heart = HeartsContainer.GetChild(i).gameObject;
            heart.SetActive(i < GameManager.instance.currentLives);
            /*if (i < GameManager.instance.currentLives)
                heart.sprite = fullHeart;
            else
                heart.sprite = emptyHeart;*/
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

        UpdateLives();
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
