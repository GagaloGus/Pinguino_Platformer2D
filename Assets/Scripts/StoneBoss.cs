using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBoss : MonoBehaviour
{
    private Animator animator;
    private CapsuleCollider2D collider;
    public bool appear, destroy, hit, visible;
    public float healthBase = 100, respawnTime = 20f;
    private float health;
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        collider.enabled = false;
        visible = false;
        health = healthBase;
    }
    void Update()
    {
        if(health <= 0)
        {
            destroy = true;
        }
        if (appear)
        {
            collider.enabled = true;
            visible = true;
            animator.SetBool("canAppear", true);
        }
        if (hit)
        {
            animator.SetBool("getHit", true);
        }
        if (destroy)
        {
            animator.SetBool("canDestroy", true);
            visible = false;
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        health = healthBase;
        appear = true;
    }

    void DisableAppear()
    {
        animator.SetBool("canAppear", false);
        appear = false;
    }
    void DisableHit()
    {
        animator.SetBool("getHit", false);
        hit = false;
    }
    void DisableDestroy()
    {
        animator.SetBool("canDestroy", false);
        destroy = false;
    }
}
