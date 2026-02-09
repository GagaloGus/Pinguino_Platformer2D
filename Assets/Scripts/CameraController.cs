using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("Movimiento")]
    public bool followPlayer;
    public float velocidadCamara = 3.5f;
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
            velocidadCamara * Time.deltaTime
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Camera cam = GetComponent<Camera>();

        // Tamaño real de la cámara
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Vector2 camLimitsMin = new Vector2(limiteMin.x - halfWidth, limiteMin.y - halfHeight);
        Vector2 camLimitsMax = new Vector2(limiteMax.x + halfWidth, limiteMax.y + halfHeight);
        float xDist = Mathf.Abs(camLimitsMin.x) + Mathf.Abs(camLimitsMax.x);
        float yDist = Mathf.Abs(camLimitsMin.y) + Mathf.Abs(camLimitsMax.y);


        Gizmos.color = Color.red;
        //Esquina minima
        Gizmos.DrawRay(camLimitsMin, Vector2.right * xDist);
        Gizmos.DrawRay(camLimitsMin, Vector2.up * yDist);

        //Esquina maxima
        Gizmos.DrawRay(camLimitsMax, Vector2.left * xDist);
        Gizmos.DrawRay(camLimitsMax, Vector2.down * yDist);
    }
#endif
}
