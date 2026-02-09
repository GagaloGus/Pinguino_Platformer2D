using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PasarNivel : MonoBehaviour
{
    public string nextLevelName;
    public bool goNextLevel, canGoNextLevel = true;
    public GameObject UpArrowKey;

    private void Start()
    {
        goNextLevel = false;
        UpArrowKey = transform.Find("up_arrow").gameObject;
        UpArrowKey.SetActive(false);
    }

    //Cuando el player sale del rango se desactiva la flechita
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player))
            UpArrowKey.SetActive(false);
    }

    //Cuando el player entra en el trigger, se muestra la flechita
    //Si le da hacia arriba, cambia a la escena escrita
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player) && canGoNextLevel)
        {
            UpArrowKey.SetActive(true);
            if (Input.GetAxis("Vertical") > 0 && !goNextLevel)
            {
                goNextLevel = true;
                LevelManager.instance.PasarNivel(nextLevelName);
            }
        }
    }
}
