using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void PlaySfx2D(AudioClip clip)
    {
        AudioManager.instance.PlaySFX2D(clip);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void KaboomSelf(float sizeInc = 1)
    {
        AudioManager.instance.StopAllSFX();
        GameManager.instance.CreateExplosion(transform.position, Vector2.one * sizeInc, true);
        Destroy(gameObject);
    }
}
