using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private MeleeDamageDealer meleeAttack;
    private float meleeTimer;

    private bool inMeleeRange
    {
        get
        {
            return Mathf.Abs(enemy.PlayerDeltaX) <= meleeRange &&
                Mathf.Abs(enemy.PlayerDeltaY) <= meleeRange;
        }
    }

    public float meleeRange = 2f;
    public float meleeCooldown = 3f;

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
        enemy.SetVelocityX(0);
        enemy.SetVelocityY(0); 
        enemy.Anim.SetTrigger("attack");
        StartCoroutine(WaitForAnimations("Attack", "Run"));

    }

    IEnumerator WaitForAnimations(string action, string idle)
    {
        if (enemy.Anim)
            do
            {
                yield return null;
            } while (!enemy.Anim.GetCurrentAnimatorStateInfo(0).IsName(action));
        meleeAttack.HorizontalHit(enemy.FacingRight);

        if (enemy.Anim)
            do
            {
                yield return null;
            } while (!enemy.Anim.GetCurrentAnimatorStateInfo(0).IsName(idle));
        enemy.inAction = false; 
    }

}
