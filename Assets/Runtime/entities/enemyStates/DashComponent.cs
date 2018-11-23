using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private MeleeDamageDealer meleeAttack;
    private float dashTimer;
    private float dashCooldown = 3f;
    private bool inDashRange
    {
        get
        {
            return Mathf.Abs(enemy.deltaX) <= dashRange && 
                Mathf.Abs(enemy.deltaY) <= dashRange/2;
        }
    }

    private bool dashing = false;

    public float dashRange = 6f;
    public float dashPause = .5f; 
    public float dashSpeed = 15;
    public float dashDistance = 8f;
    public float damageMultiplier = 1; 

    void Start () {
        enemy = GetComponent<EnemyComponent>();
        meleeAttack = GetComponent<MeleeDamageDealer>();
    }

    void Update () {
        if (dashing)
            enemy.Move(dashSpeed); 

        dashTimer += Time.deltaTime; 
        if (inDashRange && dashTimer >= dashCooldown)
        {
            attack(); 
            dashTimer = 0; 
        }
	}

    void attack()
    {
        enemy.canMove = false; 
        StartCoroutine(DashCoroutine()); 
    }

    IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(this.dashPause);
        
        enemy.anim.SetTrigger("attack");
        meleeAttack.HorizontalHit(enemy.facingRight);
        dashing = true;
        yield return new WaitForSeconds(this.dashDistance / this.dashSpeed);
        dashing = false;

        yield return new WaitForSeconds(this.dashPause);
        enemy.canMove = true; 
    }
}
