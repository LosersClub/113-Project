using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState {

    void Execute();
    void Enter(GroundEnemy enemy);
    void Exit();
    void OnTriggerEnter(Collider2D other); 
}
