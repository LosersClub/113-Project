using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private MeleeDamageDealer meleeAttack;
    private float meleeTimer;
    private float meleeCooldown = 2;
    private bool inMeleeRange
    {
        get
        {
            return enemy.deltaX <= meleeRange && enemy.deltaY <= meleeRange/2;
        }
    }

    public float meleeRange = 1.5f; 

    void Start () {
        enemy = GetComponent<EnemyComponent>();
    }

    void Update () {
        meleeTimer += Time.deltaTime; 
        if (inMeleeRange && meleeTimer >= meleeCooldown)
        {
            Debug.Log("MEEEpH");
            attack(); 
            meleeTimer = 0; 
        }
	}

    void attack()
    {
        enemy.anim.SetTrigger("attack");
        StartCoroutine(WaitForAnimation("Attack"));

    }

    IEnumerator WaitForAnimation(string name)
    {
        if (enemy.anim)
            do
            {
                yield return null;
            } while (!enemy.anim.GetCurrentAnimatorStateInfo(0).IsName(name));

        meleeAttack.HorizontalHit(enemy.facingRight);
    }
}
