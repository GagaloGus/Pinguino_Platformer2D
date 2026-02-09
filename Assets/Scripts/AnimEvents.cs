using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public void canIdle()
    {
        GetComponentInParent<Boss>().spawned = true;
    }

    public void shieldSpawn()
    {
        GetComponentInChildren<ShieldBoss>().spawn = true;
        GetComponentInChildren<ShieldBoss>().appear = true;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void HitBoxAct()
    {
        GetComponentInParent<Boss>().hitBoxsetter = true;
    }

    public void HitBoxDesct()
    {
        GetComponentInParent<Boss>().hitBoxsetter = false;
        GetComponentInParent<Boss>().animator.SetBool("canAttack", false);
    }
    public void DisableHit()
    {
        GetComponent<Animator>().SetBool("getHit", false);
        GetComponentInParent<Boss>().spawned = true;
    }
}
