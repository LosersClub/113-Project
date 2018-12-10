using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirChaseComponent : MonoBehaviour {
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
    [SerializeField]
    private float stopDistance = 2f;

    void Start () {
        enemy = GetComponent<EnemyComponent>();
	}

    void Update()
    {
        if (enemy.inAction) return;

        else
        {
            flipTimer += Time.deltaTime;
            if (flipTimer >= flipCooldown)
            {
                enemy.LookAtTarget();
                flipTimer = 0;
            }

            int dirX = enemy.FacingRight ? 1 : -1;
            int dirY = enemy.PlayerDeltaY+1.5 > 0 ? 1 : -1;
            float velocityX = dirX * speed;
            float velocityY = dirY * speed * 0.5f;

            if (this.WithinStopDistance())
            {
                velocityX = 0;
            }

            enemy.SetVelocityX(velocityX);
            enemy.SetVelocityY(velocityY);
        }
    }

    public bool WithinStopDistance()
    {
        return Mathf.Abs(enemy.PlayerDeltaX) <= stopDistance;
    }
}
