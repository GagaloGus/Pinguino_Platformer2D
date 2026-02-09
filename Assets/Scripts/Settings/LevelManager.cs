using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayLevelSong()
    {
        AudioManager.instance.PlayLevelSong(levelID);
    }

    public void PasarNivel(string levelName)
    {
        GameManager.instance.ChangeSceneWithTransition(levelName);
    }
}
