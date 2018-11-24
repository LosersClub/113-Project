using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private MeleeDamageDealer meleeAttack;
    private float meleeTimer;
    private float meleeCooldown = 1.5f;
    private bool inMeleeRange
    {
        get
        {
            return Mathf.Abs(enemy.deltaX) <= meleeRange && 
                Mathf.Abs(enemy.deltaY) <= meleeRange/2;
        }
    }

    public float meleeRange = 2f; 

    void Start () {
        enemy = GetComponent<EnemyComponent>();
        meleeAttack = GetComponent<MeleeDamageDealer>();
    }

    void Update () {
        meleeTimer += Time.deltaTime; 
        if (inMeleeRange && meleeTimer >= meleeCooldown)
        {
            attack(); 
            meleeTimer = 0; 
        }
	}

    void attack()
    {
        if (enemy.inAction) return; 

        enemy.inAction = true; 
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
        enemy.inAction = false; 
    }

}
