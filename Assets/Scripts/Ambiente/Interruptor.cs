using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interruptor : MonoBehaviour
{
    public float activationRadius;
    public AudioClip activationSound;
    public UnityEvent OnActivation;

    // Update is called once per frame
    void Update()
    {
        //Si el jugador esta lo suficientemente cerca, y le da al click izq
        //La funcion asignada desde el inspector (OnActivation) se ejecuta
        if(Vector3.Distance(transform.position, PlayerController.instance.transform.position) <= activationRadius
            && Input.GetKeyDown(KeyCode.Mouse0))
        {
            AudioManager.instance.PlaySFX2D(activationSound);
            OnActivation.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
#endif
}
