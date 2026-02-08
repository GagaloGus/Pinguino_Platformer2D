using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("Movimiento")]
    public bool followPlayer;
    public float velocidadCamara = 0.1f;
    public Vector3 desplazamiento;

    [Header("Limites")]
    public Vector2 limiteMin; // abajo-izquierda
    public Vector2 limiteMax; // arriba-derecha

    private void Awake()
    {
        instance = this;
        followPlayer = true;
    }

    private void LateUpdate()
    {
        if (!followPlayer)
            return;

        Vector3 posicionDeseada = PlayerController.instance.transform.position + desplazamiento;


        // Clamp con los limites
        posicionDeseada.x = Mathf.Clamp(
            posicionDeseada.x,
            limiteMin.x,
            limiteMax.x
        );

        posicionDeseada.y = Mathf.Clamp(
            posicionDeseada.y,
            limiteMin.y,
            limiteMax.y
        );

        posicionDeseada.z = transform.position.z;

        // Suavizado
        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            velocidadCamara *0.01f
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Esquina minima
        Gizmos.DrawSphere(transform.position, 3);
        Gizmos.DrawRay(limiteMin, Vector2.right);
        Gizmos.DrawRay(limiteMin, new(limiteMin.x, limiteMax.y));

        //Esquina maxima
        Gizmos.DrawRay(limiteMax, new(limiteMax.x, limiteMin.y));
        Gizmos.DrawRay(limiteMax, new(limiteMin.x, limiteMax.y));
    }
#endif
}
