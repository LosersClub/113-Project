using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState {

    private GroundEnemy enemy;
    private float patrolTimer;
    private float patrolDuration;

    public void Enter(GroundEnemy enemy)
    {
        patrolDuration = UnityEngine.Random.Range(1, 10); 
        this.enemy = enemy;
    }

    public void Execute()
    {
        Patrol();

        enemy.Move();

        if (enemy.Target != null && enemy.InMeleeRange) enemy.ChangeState(new MeleeState());
        else if (enemy.Target != null) enemy.Move(); 
    }

    public void Exit()
    {
    }

    public void OnTriggerEnter(Collider2D other)
    {
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            enemy.ChangeState(new IdleState());
        }
    }
}
