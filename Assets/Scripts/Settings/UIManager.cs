using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Range(1, 3)] public uint levelInt = 1;

    Animator PantallaCarga;

    public GameObject menu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        ;
        //PantallaCarga = transform.Find("Pantalla carga").GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.instance.StopAll();
        //PantallaCarga.gameObject.SetActive(true);
        //PantallaCarga.SetInteger("state", 2);
        //AudioManager.instance.PlaySFX2D(MusicLibrary.instance.door_close_sfx);

        if (levelInt == 1)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level1_song);
        else if (levelInt == 2)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level2_song);
        else if (levelInt == 3)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level3_song);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.instance.gameState == GameState.Playing)
            {
                onPauseButton();
            }
            else if (GameManager.instance.gameState == GameState.Paused)
            {
                onResumeGame();
            }
        }
    }

    public void ChangeScene(string sceneName)
    {
        GameManager.instance.ChangeSceneWithTransition(sceneName);
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void onPauseButton()
    {
        if (menu == null)
        {
            Debug.LogError("¡Ojo! No hay ningún objeto asignado a menuUI en el inspector.");
            return;
        }

        if (GameManager.instance.gameState != GameState.Playing)
            return;

        menu.SetActive(true);
        GameManager.instance.PauseGame();
    }

    public void onResumeGame()
    {
        menu.SetActive(false);
       // GameManager.instance.ResumeGame();
    }

    public void onQuitGame()
    {
        Application.Quit();
    }
}