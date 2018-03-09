using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPumpkin : Monster {


	// Use this for initialization
	void Start () {
		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		base.Update ();	
	}

	void OnCollisionEnter2D(Collision2D other){
		base.OnCollisionEnter2D (other);
		if (other.gameObject.tag == "BigPumpkin") {
			//base.evolveTo (other.gameObject, evolution_tree [0]);
		}
	}
}
