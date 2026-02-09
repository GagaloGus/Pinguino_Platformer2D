using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Options")]
    public GameObject sprite, hitBox, shield, player;
    public Animator animator;
    public bool spawned,hitBoxsetter;
    public bool canCheck, canBeHit = false;
    public float attackDis = 4f;
    [HideInInspector] public int healthBase;

    [Header("Boss Stones")]
    public GameObject[] bossStones;
    void Start()
    {
        player = PlayerController.instance.gameObject;
        rb = GetComponent<Rigidbody2D>();
        objDetector = transform.GetChild(0).gameObject;
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
    protected override void Update()
    {
        //Debug.Log(bossStones[0].GetComponentInChildren<StoneBoss>().visible);

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

            if (IsTargetInCone(player.transform) && spawned)
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

            if (IsTargetInCone(player.transform) == false && canPatrol && PatrolPoints.Count > 0)
            {
                Patrol();
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
            shield.GetComponent<ShieldBoss>().destroy = true;
            for (int i = 0; i < bossStones.Length; i++)
            {
                bossStones[i].transform.GetChild(0).gameObject.GetComponent<StoneBoss>().destroy = true;
            }
        }
    }

    override protected void Move(Vector2 loc)
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

    protected void Patrol()
    {
        Vector2 target = PatrolPoints[currentPointIndex];

        if (Mathf.RoundToInt(transform.position.x) != Mathf.RoundToInt(target.x) && isWaiting == false && canJump)
        {
            Move(target);
        }
        else
        {
            StartCoroutine(wait());
            animator.SetBool("canWalk", false);

            currentPointIndex++;

            if (currentPointIndex >= PatrolPoints.Count)
            {
                currentPointIndex = 0;
            }
        }
    }

    protected void Search()
    {
        canPatrol = false;
        // Solo se mueve si no ha llegado a la última posición conocida
        if (Mathf.Abs(transform.position.x - lastLoc.x) > 0.5f && canJump)
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
            canPatrol = true;
            lost.SetActive(false);
            animator.SetBool("canWalk", false);
        }
    }

    override protected bool IsTargetInCone(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position);
        float dist = dirToTarget.magnitude;

        // Detección por proximidad (Círculo 360º)
        if (dist <= detectionRadius)
        {
            canPatrol = false;
            wasInside = true;
            timeLeft = 5f;
            lastLoc = player.transform.position;
            if (!spawned)
            {
                animator.SetBool("spawn", true);
            }
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
                canPatrol = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canBeHit)
        {
            animator.SetBool("getHit", true);
            health -= 100;
            StartCoroutine("hitCooldown");
        }
    }

    IEnumerator hitCooldown()
    {
        canBeHit = false;
        yield return new WaitForSeconds(2f);
        canBeHit = true;
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

        if (PatrolPoints.Count > 0)
        {
            for (int i = 0; i < PatrolPoints.Count; i++)
            {
                Gizmos.color = Color.white;

                Gizmos.DrawWireSphere(PatrolPoints[i], 1);
            }
        }
    }
}
