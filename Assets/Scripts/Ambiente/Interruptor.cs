using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interruptor : MonoBehaviour
{
    public AudioClip activationSound;
    public UnityEvent OnActivation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerBullet>() || collision.CompareTag("PlayerAttackBox"))
        {
            Activate();
        }
    }

    //Cuando el player ataque al interruptor o le tire una bola de nieve
    //Se invoca la funcion puesta en el insepctor
    public void Activate()
    {
        AudioManager.instance.PlaySFX2D(activationSound);
        OnActivation.Invoke();
    }
}
