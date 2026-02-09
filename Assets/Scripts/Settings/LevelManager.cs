using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int levelID;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayLevelSong();
    }

    public void ChangeScene(string sceneName)
    {
        GameManager.instance.ChangeSceneWithTransition(sceneName);
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void PlayLevelSong()
    {
        AudioManager.instance.PlayLevelSong(levelID);
    }

    public void PasarNivel(string levelName)
    {
        PlayerController.instance.life += GameManager.instance.startingLives;
        GameManager.instance.ChangeSceneWithTransition(levelName);
    }
}
