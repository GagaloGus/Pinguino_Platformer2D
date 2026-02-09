using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImg;
    private Boss bossController;

    void Start()
    {
        bossController = FindAnyObjectByType<Boss>();
        fillImg = transform.GetChild(0).GetComponent<Image>();
    }
    void Update()
    {
        fillImg.fillAmount = bossController.health / bossController.healthBase;
        if(bossController.health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
