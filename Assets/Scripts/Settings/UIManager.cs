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

    [Header("Lives UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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

    public void ChangeScene(string sceneName)
    {
        GameManager.instance.ChangeSceneWithTransition(sceneName);
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
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
}
