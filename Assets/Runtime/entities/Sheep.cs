using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : GroundEnemy {

	public override void Start () {
		base.Start(); 
		speed = 3f;
		distance = .5f;

        front_sight = 0;
        behind_sight = 0;
        vertical_sight = 0;
    }
	
	public override void Move() {
        base.Move();
	}
}
