using System.Collections;
using System.Collections.Generic;
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
}
