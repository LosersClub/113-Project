using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private ColliderDamageDealer cdd;
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
        cdd = GetComponent<ColliderDamageDealer>();
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

        dashing = true;
        float temp = cdd.Damage;
        //cdd.Damage = temp * damageMultiplier; 
        yield return new WaitForSeconds(this.dashDistance / this.dashSpeed);
        //cdd.Damage = temp; 
        dashing = false; 

        yield return new WaitForSeconds(this.dashPause);
        enemy.canMove = true; 
    }
}
