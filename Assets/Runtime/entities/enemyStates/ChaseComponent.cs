using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyComponent))]
public class ChaseComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private float flipTimer;
    private float flipCooldown
    {
        get
        {
            // randomized flip cooldown so enemies dont stack
            return UnityEngine.Random.Range(.20f, 1);
        }
    }
    public float speed = 3f;

    void Start () {
        enemy = GetComponent<EnemyComponent>();
	}
	
	void Update () {
        if (enemy.inAction) return; 
        
        flipTimer += Time.deltaTime;
        if (flipTimer >= flipCooldown)
        {
            enemy.LookAtTarget();
            flipTimer = 0; 
        }
        int dir = enemy.FacingRight ? 1 : -1;

        enemy.Move(speed); 
    }
}
