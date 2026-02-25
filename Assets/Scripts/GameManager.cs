using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Victory, GameOver }

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public GameObject Prefab_Explosion;
    Animator PantallaCarga;

    [Header("Settings")]
    [Range(0, 1)] public float chancePlayLoadSound = 0.5f;
    [Range(0, 1)] public float chancePlaySecondDeathSound = 0.5f;
    public bool hasDied;
    AudioClip lastDeathAudio;

    [Header("Game State")]
    public GameState gameState = GameState.Playing;

    [Header("Score")]
    public int score;
    public int highScore;

    [Header("Score Settings")]
    public int pointsBaseEnemy = 100;

    public int pointsCoinBronze = 10;
    public int pointsCoinSilver = 25;
    public int pointsCoinGold = 50;
    public int pointsCoinSpecial = 150;

    [Header("Lives")]
    public int currentLives;
    public int startingLives = 5, maxLives = 16;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        } else
        {
            Destroy(gameObject);
            return;
        }
       
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        PantallaCarga = transform.Find("Canvas").Find("Pantalla carga").GetComponent<Animator>();
    }

    void Start()
    {
        Time.timeScale = 1f;
        gameState = GameState.Playing;

        currentLives = startingLives;

        StartLevel();
    }

    public void AddScore(int points)
    {
        score += points;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void AddCoinScore(int coinType)
    {
        int points = 0;

        switch (coinType)
        {
            case 0:
                points = pointsCoinBronze;
                break;
            case 1:
                points = pointsCoinSilver;
                break;
            case 2:
                points = pointsCoinGold;
                break;
            case 3:
                points = pointsCoinSpecial;
                break;
        }

        AddScore(points);
    }

    public void LoseLife()
    {
        if (gameState != GameState.Playing) return;

        currentLives--;

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void ResetRun()
    {
        score = 0;
        gameState = GameState.Playing;
    }

    public void PauseGame()
    {
        if (gameState != GameState.Playing) return;

        gameState = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (gameState != GameState.Paused) return;

        gameState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        ResetRun();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel1");
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        Time.timeScale = 0f;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public void Victoria()
    {
        gameState = GameState.Victory;
        Time.timeScale = 0f;
    }

    public void Death()
    {
        currentLives = 0;
        hasDied = true;

        //Audio
        AudioManager.instance.StopAll();
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.lego_breaking_sfx);

        AudioClip clip;
        float length = 0;

        do
        {
            clip = MusicLibrary.instance.GetRandomClip(MusicLibrary.instance.player_die_sfxs);
        }
        while (clip == lastDeathAudio);

        AudioManager.instance.PlaySFX2D(clip);

        if(Random.value < chancePlaySecondDeathSound)
        {
            AudioClip clip2;
            do
            {
                clip2 = MusicLibrary.instance.GetRandomClip(MusicLibrary.instance.player_die_sfxs);
            }
            while (clip2 == clip || clip2 == lastDeathAudio);
            AudioManager.instance.PlaySFX2D(clip2);
            length = Mathf.Min(clip.length, clip2.length);
        }
        else
            length = clip.length;

        lastDeathAudio = clip;

        CoolFunctions.InvokeDelayed(this, Mathf.Clamp(length - 0.15f, 0, 8), () =>
        {
            ResetRun();
            ChangeSceneWithTransition(SceneManager.GetActiveScene().name);
        });
    }

    public void CreateExplosion(Transform objTransform, bool playSound)
    {
        CreateExplosion(objTransform.position, objTransform.localScale, playSound);
    }

    public void CreateExplosion(Vector3 position, Vector3 localScale, bool playSound)
    {
        Transform kaput = Instantiate(Prefab_Explosion).transform;
        AudioManager.instance.PlayRandomSFX2D(MusicLibrary.instance.explosion_sfxs);
        kaput.position = position;
        kaput.localScale = localScale;
    }

    public void StartLevel()
    {
        if (hasDied)
            currentLives = startingLives;

        hasDied = false;
        PantallaCarga.gameObject.SetActive(true);
        PantallaCarga.SetInteger("state", 2);
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.door_close_sfx);
    }

    public void ChangeSceneWithTransition(string sceneName)
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.door_open_sfx);

        bool playLoadClip = Random.value < chancePlayLoadSound;
        float clipLength = 0;
        float waitTime = 0;

        if (playLoadClip)
        {
            clipLength = AudioManager.instance.PlayRandomSFX2D(MusicLibrary.instance.level_load_sfxs).length;
            waitTime = Mathf.Clamp(Random.Range(0.4f, 1f) * clipLength, 0, 8);
        }
        else
        {
            clipLength = MusicLibrary.instance.door_open_sfx.length;
            waitTime = Random.Range(0.9f, 1.2f) * clipLength;
        }

        PantallaCarga.gameObject.SetActive(true);
        PantallaCarga.SetInteger("state", 1);

        CoolFunctions.InvokeDelayed(this, waitTime, () =>
        {
            ChangeScene(sceneName);
        });
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeAsyncScene(sceneName));
    }

    IEnumerator ChangeAsyncScene(string sceneName)
    {
        print($"loading scene async {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        StartLevel();
    }


}
