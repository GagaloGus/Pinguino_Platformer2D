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

    TMP_Text livesText;

    private void Awake()
    {
        instance = this;
        livesText = transform.Find("vidas text").GetComponent<TMP_Text>();
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

    private void Update()
    {
        livesText.text = $"Lives: {PlayerController.instance.life}";
    }
}
