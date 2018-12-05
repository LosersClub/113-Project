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
    [SerializeField]
    private float speed = 3f;

    // Stop chasing if within this distance of player. If 0, always chase:
    [SerializeField]
    private float stopDistance = 0f;

    void Start () {
        enemy = GetComponent<EnemyComponent>();
	}
	
	void Update () {
        if (enemy.inAction) return; 

        if(this.WithinStopDistance()) {
            enemy.SetSpeed(0);
        }
        else {
            flipTimer += Time.deltaTime;
            if (flipTimer >= flipCooldown) {
                enemy.LookAtTarget();
                flipTimer = 0;
            }

            enemy.SetSpeed(speed);
        }
    }

    public bool WithinStopDistance() {
        return this.enemy.PlayerDistance <= stopDistance;
    }
}
