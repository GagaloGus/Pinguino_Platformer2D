using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : MonoBehaviour
{
    public GameObject sprite, hitBox, shield, player, seen, lost;

    [Header("Enemy Movement")]
    public float speed = 5f;
    public float dir = -1;

    [Header("Boss stats")]
    public int health;
    public int healthBase;

    [Header("Enemy Field of View")]
    public float angle = 30.0f;
    public float rayRange = 10.0f;
    public float coneDirection = 180;

    [Header("Detection Settings")]
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public float detectionRadius = 2f;
    public float raycastDetectionDown = 0.5f;

    [Header("Time for search Player")]
    public float timeLeft = 5f;

    [Header("Actions Check")]
    public bool isGrounded = false;

    protected Rigidbody2D rb;
    protected bool wasInside = false, isWaiting = false;
    protected Vector2 lastLoc;

    [Header("Boss Options")]
    public Animator animator;
    public bool spawned,hitBoxsetter;
    public bool canCheck, canBeHit = false, canMove = true;
    public float attackDis = 4f;
    public UnityEvent OnDeathEvent;

    [Header("Boss Stones")]
    public GameObject[] bossStones;
    void Start()
    {
        player = PlayerController.instance.gameObject;
        rb = GetComponent<Rigidbody2D>();
        lost = transform.GetChild(1).gameObject;
        seen = transform.GetChild(2).gameObject;
        hitBox = transform.GetChild(3).gameObject;
        sprite = transform.GetChild(4).gameObject;
        shield = sprite.transform.GetChild(0).gameObject;
        animator = sprite.GetComponent<Animator>();
        spawned = false;
        hitBoxsetter = false;
        bossStones = GameObject.FindGameObjectsWithTag("StoneBoss");
        canCheck = true;
        health = healthBase;
    }
    public void Update()
    {
        if (canCheck && spawned)
        {
            CheckStones();
        }

        if (player.activeSelf && health > 0)
        {
            if(hitBoxsetter == true)
            {
                hitBox.SetActive(true);
            }
            else
            {
                hitBox.SetActive(false);
            }

            if (IsTargetInCone(player.transform) && spawned && canMove)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);

                if (dist > attackDis && animator.GetBool("canAttack") == false)
                {
                    Move(player.transform.position);
                    seen.SetActive(true);
                    lost.SetActive(false);
                }
                else if (animator.GetBool("getHit"))
                {
                    spawned = false;
                    animator.SetBool("canWalk", false);
                    animator.SetBool("canAttack", false);
                }
                else
                {
                    animator.SetBool("canWalk", false);
                    animator.SetBool("canAttack", true);
                }
            }

            if (IsTargetInCone(player.transform) == false && wasInside == true && spawned == true)
            {
                // LLama el método para que este objeto siga siguiendo al objetivo durante un tiempo (Como buscandolo)
                Search();
            }
        }
        else
        {
            animator.SetBool("canWalk", false);
        }

        if (health <= 0)
        {
            animator.SetBool("death", true);
            animator.SetBool("spawn", false);
            seen.SetActive(false);
            lost.SetActive(false);
            shield.SetActive(false);
            sprite.GetComponent<CapsuleCollider2D>().enabled = false;
            rb.gravityScale = 0;
            for (int i = 0; i < bossStones.Length; i++)
            {
                bossStones[i].transform.GetChild(0).gameObject.GetComponent<StoneBoss>().destroy = true;
                bossStones[i].transform.GetChild(0).gameObject.GetComponent<StoneBoss>().enabled = false;   
            }

            OnDeathEvent.Invoke();
        }
    }

    public void Move(Vector2 loc)
    {
        // Calcula si el objetivo se encuentra a su derecha
        if (transform.position.x - loc.x < 0)
        {
            // Cambia su dirección de movimiento hacia la derecha
            dir = 1f;
            sprite.GetComponent<SpriteRenderer>().flipX = true;
            // Cambia la ubicación del detector de obstaculos a la derecha
            hitBox.transform.position = new Vector2(transform.position.x + 2.75f, transform.position.y);
            // Cambia la rotacion del campo de vision para que mire a su derecha
            coneDirection = 0f;
        }

        // Calcula si el objetivo se encuentra a su izquierda
        if (transform.position.x - loc.x > 0)
        {
            // Cambia su dirección de movimiento hacia la izquierda
            dir = -1f;
            sprite.GetComponent<SpriteRenderer>().flipX = false;
            // Cambia la ubicación del detector de obstaculos a la izquierda
            hitBox.transform.position = new Vector2(transform.position.x - 2.75f, transform.position.y);
            // Cambia la rotacion del campo de vision para que mire a su izquierda
            coneDirection = 180f;
        }
        // Una vez visto hacia que direccion moverse, procede a acelerar
        Vector2 move = new Vector2(dir, 0f) * speed * Time.deltaTime;
        transform.Translate(move);
        animator.SetBool("canWalk", true);
    }

    public void Search()
    {
        // Solo se mueve si no ha llegado a la última posición conocida
        if (Mathf.Abs(transform.position.x - lastLoc.x) > 0.5f)
        {
            Move(lastLoc);
            seen.SetActive(false);
            lost.SetActive(true);
        }
        else
        {
            animator.SetBool("canWalk", false);
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            wasInside = false;
            timeLeft = 5f;
            lost.SetActive(false);
            animator.SetBool("canWalk", false);
        }
    }

    public bool IsTargetInCone(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position);
        float dist = dirToTarget.magnitude;

        // Detección por proximidad (Círculo 360º)
        if (dist <= detectionRadius)
        {
            wasInside = true;
            timeLeft = 5f;
            lastLoc = player.transform.position;
            if (!spawned)
            {
                animator.SetBool("spawn", true);
            }
            Debug.Log("Visto");
            return true;
        }

        // Detección por Cono
        Vector3 baseDirection = Quaternion.Euler(0, 0, coneDirection) * sprite.transform.right;
        float angleToTarget = Vector3.Angle(baseDirection, dirToTarget);

        if (angleToTarget < angle / 2.0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget.normalized, rayRange, obstructionMask | targetMask);
            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & targetMask) != 0)
            {
                wasInside = true;
                timeLeft = 5f;
                lastLoc = player.transform.position;
                return true;
            }
        }
        Collider2D hitS = Physics2D.OverlapCircle(transform.position, detectionRadius, targetMask);

        if (hitS != null)
        {
            if (!spawned)
            {
                animator.SetBool("spawn", true);
            }
            if (dir == 1f && spawned)
            {
                coneDirection = 180;
                sprite.GetComponent<SpriteRenderer>().flipX = false;
                hitBox.transform.position = new Vector2(transform.position.x - 2.75f, transform.position.y);
                dir = -1f;
            }
            else if (dir == -1f && spawned)
            {
                coneDirection = 0;
                sprite.GetComponent<SpriteRenderer>().flipX = true;
                hitBox.transform.position = new Vector2(transform.position.x + 2.75f, transform.position.y);
                dir = 1f;
            }
        }

        if (dist > rayRange)
        {
            return false;
        }
        // Si no se cumple nada de lo anterior devuelve false
        return false;
    }

    public void CheckStones()
    {
        canCheck = false;
        bool yaSeHizo = false;
        int count = 0;
        for (int i = 0; i < bossStones.Length; i++)
        {
            if (bossStones[i].GetComponentInChildren<StoneBoss>().visible)
            {
                count++;
            }
        }
        canBeHit = false;
        if (count == 0 && !yaSeHizo)
        {
            shield.GetComponent<ShieldBoss>().destroy = true;
            canBeHit = true;
            for (int i = 0; i < bossStones.Length; i++)
            {
                bossStones[i].transform.GetChild(0).gameObject.GetComponent<StoneBoss>().StartCoroutine("Respawn");
            }
            yaSeHizo = true;
        }
        else
        {
            canCheck = true;
        }
    }
    void GetDamage(int dmg)
    {
        health -= dmg;
        canMove = false;
        if (health <= 0)
        {
            print("ay");
            AudioManager.instance.StopAmbientMusic();
            AudioManager.instance.PlaySFX2D(MusicLibrary.instance.enemy_kill_sfx);
            AudioManager.instance.PlayRandomSFX2D(MusicLibrary.instance.enemy_death_sfxs);
            GameManager.instance.CreateExplosion(transform, false);
        }
        else
        {
            print("mamon");
            AudioManager.instance.PlaySFX2D(MusicLibrary.instance.enemy_ow_sfx);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackBox") && canBeHit && health > 0)
        {
            if (PlayerController.instance.isSliding)
            {
                animator.SetBool("getHit", true);
                StartCoroutine("hitCooldown");
                PlayerController.instance.onChargeOnEnemy(health);
                GetDamage(PlayerController.instance.dmgChargeAtk);
            }
            else
            {
                animator.SetBool("getHit", true);
                StartCoroutine("hitCooldown");
                GetDamage(PlayerController.instance.dmgMeleeAtk);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBullet bullet) && canBeHit && health > 0)
        {
            animator.SetBool("getHit", true);
            StartCoroutine("hitCooldown");

            GetDamage(bullet.damage);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator hitCooldown()
    {
        sprite.GetComponent<SpriteRenderer>().color = new Color32(255, 153, 153, 255);
        canBeHit = false;
        yield return new WaitForSeconds(2f);
        canBeHit = true;
        sprite.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }

    protected void OnDrawGizmos()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y - 1.8f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(enemyPoS, Vector2.down * raycastDetectionDown);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Quaternion upRayRotation = Quaternion.AngleAxis(-(angle / 2.0f) + coneDirection, Vector3.forward);
        Quaternion downRayRotation = Quaternion.AngleAxis((angle / 2.0f) + coneDirection, Vector3.forward);

        Vector3 upRayDirection = upRayRotation * transform.right * rayRange;
        Vector3 downRayDirection = downRayRotation * transform.right * rayRange;

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(transform.position, upRayDirection);
        Gizmos.DrawRay(transform.position, downRayDirection);
        Gizmos.DrawLine(transform.position + downRayDirection, transform.position + upRayDirection);
    }
}
