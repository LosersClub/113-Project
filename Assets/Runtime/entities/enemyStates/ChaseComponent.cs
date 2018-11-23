using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private float flipTimer;
    private float flipCooldown
    {
        get
        {
            return UnityEngine.Random.Range(.25f, 1);
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
        Vector2 dir = enemy.GetDirection();

        enemy.Move(speed); 
	}

    void Move(float speed)
    {
        Animator anim = enemy.anim;
        transform.Translate(enemy.GetDirection() * speed * Time.deltaTime);
    }
}
