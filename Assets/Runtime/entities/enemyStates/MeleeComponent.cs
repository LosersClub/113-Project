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
            return Mathf.Abs(enemy.PlayerDeltaX) <= meleeRange &&
                Mathf.Abs(enemy.PlayerDeltaY) <= meleeRange;
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
        enemy.SetSpeed(0);
        enemy.Anim.SetTrigger("attack");
        StartCoroutine(WaitForAnimation("Attack"));

    }

    IEnumerator WaitForAnimation(string name)
    {
        if (enemy.Anim)
            do
            {
                yield return null;
            } while (!enemy.Anim.GetCurrentAnimatorStateInfo(0).IsName(name));

        meleeAttack.HorizontalHit(enemy.FacingRight);
        enemy.inAction = false; 
    }

}
