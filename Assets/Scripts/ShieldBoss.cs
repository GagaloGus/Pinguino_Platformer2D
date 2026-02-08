using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBoss : MonoBehaviour
{
    private Animator animator;
    private CircleCollider2D colliderCircle;
    private GameObject parent;
    public bool spawn, appear, destroy, hit;

    void Start()
    {
        parent = transform.parent.gameObject;
        animator = GetComponent<Animator>();
        colliderCircle = GetComponent<CircleCollider2D>();
        colliderCircle.enabled = false;
    }

    void Update()
    {
        if(spawn && appear)
        {
            colliderCircle.enabled = true;
            animator.SetBool("Spawned", true);
            animator.SetBool("Appear", true);
        }
        if (hit)
        {
            animator.SetBool("Hit", true);
        }
        if (destroy)
        {
            animator.SetBool("Destroy", true);
        }
    }

    void DisableAppear()
    {
        animator.SetBool("Appear", false);
        appear = false;
    }
    void DisableHit()
    {
        animator.SetBool("Hit", false);
        hit = false;
    }
    void DisableDestroy()
    {
        animator.SetBool("Destroy", false);
        destroy = false;
    }
}
