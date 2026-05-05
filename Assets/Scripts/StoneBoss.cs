using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBoss : MonoBehaviour
{
    public GameObject Boss;
    private Animator animator;
    private CapsuleCollider2D collider;
    public bool appear, destroy, hit, visible =false;
    public float healthBase = 10, respawnTime = 20f;
    private float health;
    SpriteRenderer Connection;
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        collider.enabled = false;
        health = healthBase;
        Boss = FindAnyObjectByType<Boss>().gameObject;
        Connection = transform.Find("Connect").GetComponent<SpriteRenderer>();
    }
    void Update()
    {

        if (health <= 0)
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
            collider.enabled = false;
        }

        ConnectWithBoss();
    }

    void ConnectWithBoss()
    {
        Vector2 bossVector = Boss.transform.position - transform.position;
        Vector2 centerPoint = new Vector2((Boss.transform.position.x + transform.position.x) / 2, (Boss.transform.position.y + transform.position.y) / 2);

        Connection.transform.up = bossVector;
        Connection.transform.position = centerPoint;
        Connection.size = new Vector2(Connection.size.x, bossVector.magnitude);

    }

    void GetDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            print("ay");
            AudioManager.instance.PlaySFX2D(MusicLibrary.instance.enemy_kill_sfx);
            AudioManager.instance.PlayRandomSFX2D(MusicLibrary.instance.enemy_death_sfxs);
            GameManager.instance.CreateExplosion(transform, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackBox") && health > 0)
        {
            if (PlayerController.instance.isSliding)
            {
                animator.SetBool("getHit", true);
                GetDamage(PlayerController.instance.dmgChargeAtk);
            }
            else
            {
                animator.SetBool("getHit", true);
                GetDamage(PlayerController.instance.dmgMeleeAtk);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBullet bullet) && health > 0)
        {
            animator.SetBool("getHit", true);
            GetDamage(bullet.damage);
            Destroy(collision.gameObject);
        }
    }

    //Eventos de animacion
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        health = healthBase;
        appear = true;
        Boss.GetComponent<Boss>().shield.GetComponent<ShieldBoss>().appear = true;
        Boss.GetComponent<Boss>().canCheck = true;
        Boss.GetComponent<Boss>().canBeHit = false;
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

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (Boss == null)
            Boss = FindAnyObjectByType<Boss>().gameObject;
        if (Connection == null)
            Connection = transform.Find("Connect").GetComponent<SpriteRenderer>();
        ConnectWithBoss();
    }
    private void OnDrawGizmos()
    {
        if(Boss == null)
            Boss = FindAnyObjectByType<Boss>().gameObject;

        Vector2 bossVector = Boss.transform.position - transform.position;
        Vector2 centerPoint = new Vector2((Boss.transform.position.x + transform.position.x) / 2, (Boss.transform.position.y + transform.position.y) / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, bossVector);

        Gizmos.DrawWireSphere(centerPoint, 0.5f);
    }
#endif
}
