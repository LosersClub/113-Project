using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newEnemy : GroundEnemy {

    public override void Start () {
        base.Start();
        speed = 3; 
        distance = .5f; 
	}

    public override void Move()
    {
        base.Move();
    }
}
