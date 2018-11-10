using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : IEnemyState {

    private GroundEnemy enemy;
    private float attackTimer;
    private float attackCooldown = 3;
    private bool canAttack = true; 

    public void Enter(GroundEnemy enemy)
    {
        this.enemy = enemy; 
    }

    public void Execute()
    {
        Attack();
        if (!enemy.InMeleeRange)
            enemy.ChangeState(new PatrolState());  
    }

    public void Exit()
    {
    }

    public void OnTriggerEnter(Collider2D other)
    {
    }

    private void Attack()
    {
        attackTimer += Time.deltaTime; 

        if (attackTimer >= attackCooldown)
        {
            canAttack = true;
            attackTimer = 0; 
        }

        if (canAttack)
        {
            canAttack = false;
            enemy.anim.SetTrigger("attack"); 
        }
    }

}
